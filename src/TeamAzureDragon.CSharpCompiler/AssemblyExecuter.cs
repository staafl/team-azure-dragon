using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System.Security;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace TeamAzureDragon.CSharpCompiler
{
    public class AssemblyExecuter
    {
        /// <summary>
        /// Todo:
        /// * capture console output
        /// * pipe console input
        /// * timeout
        /// * memory cap
        /// </summary>
        public sealed class ProxyExecuter : MarshalByRefObject
        {
            public ProxyExecuter()
            {
            }

            public void Run(byte[] compiledAssembly, EventWaitHandle wh, string stdin, bool getResult, out object result, out Exception exception, out string stdout)
            {
                exception = null;
                result = null;
                stdout = null;

                StringWriter stdoutWriter = null;
                TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers.SafeSetStdInOut(stdin, out stdoutWriter);

                var assembly = Assembly.Load(compiledAssembly);

                wh.Set();

                try
                {
                    assembly.EntryPoint.Invoke(null, new object[] { });
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (stdoutWriter != null)
                    stdout = stdoutWriter.ToString();

                if (getResult)
                {
                    var type = assembly.GetType("EntryPoint");
                    if (type != null)
                    {
                        var resultProp = type.GetProperty("Result");
                        if (resultProp != null)
                            result = resultProp.GetValue(null, null);
                    }
                }
            }
        }

        static AppDomain CreateSandbox()
        {
            var e = new Evidence();
            e.AddHostEvidence(new Zone(SecurityZone.Internet));

            var ps = SecurityManager.GetStandardSandbox(e);
            var security = new SecurityPermission(SecurityPermissionFlag.Execution);

            ps.AddPermission(security);

            var setup = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
            return AppDomain.CreateDomain("Sandbox" + DateTime.Now, null, setup, ps, typeof(TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers).Assembly.Evidence.GetHostEvidence<StrongName>());
        }

        public static void ExecuteAssembly(
            byte[] assemblyIL,
            string stdin,
            out object result,
            out Exception exception,
            out string stdout,
            out bool timedOut,
            out bool memoryCapHit,
            int? timeoutSeconds = 6,
            int? memoryCapMb = 15)
        {
            result = null;
            exception = null;
            timedOut = false;
            memoryCapHit = false;

            // sandbox appdomain
            var sandbox = CreateSandbox();

            try
            {
                // add references
                foreach (var asm in Helpers.GetStandardReferences(false, false))
                    sandbox.Load(asm);

                // create proxy in sandbox appdomain
                var proxy = (ProxyExecuter)Activator.CreateInstance(sandbox, typeof(ProxyExecuter).Assembly.FullName, typeof(ProxyExecuter).FullName).Unwrap();

                // run computation in sandboxed appdomain, on a separate thread, and wait for it to signal the start of computation
                Exception tempException = null;
                string tempStdout = null;
                object tempResult = null;

                Thread scriptThread = null;
                using (var wh = new EventWaitHandle(false, EventResetMode.AutoReset))
                {
                    scriptThread = new Thread(() =>
                   {
                       proxy.Run(assemblyIL, wh, stdin, true, out tempResult, out tempException, out tempStdout);
                   });

                    scriptThread.Start();
                    wh.WaitOne();
                }

                var sw = Stopwatch.StartNew();


                while (true)
                {
                    // resolution of 100 ms
                    if (scriptThread.Join(100))
                        break;

                    // todo: check sandbox memory consumption

                    if (sw.ElapsedMilliseconds > timeoutSeconds * 1000)
                    {
                        scriptThread.Abort();
                        timedOut = true;
                        break;
                    }
                }

                stdout = tempStdout;
                result = tempResult;
                exception = tempException;
            }
            finally
            {
                // cleanup
                AppDomain.Unload(sandbox);
            }

        }

    }
}
