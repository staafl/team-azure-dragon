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
using TeamAzureDragon.CSharpCompiler.ProxyExecuter;

namespace TeamAzureDragon.CSharpCompiler
{
    public class AssemblyExecuter
    {
        static AppDomain CreateSandbox()
        {
            var e = new Evidence();
            e.AddHostEvidence(new Zone(SecurityZone.Internet));

            var ps = SecurityManager.GetStandardSandbox(e);
            var security = new SecurityPermission(SecurityPermissionFlag.Execution);

            ps.AddPermission(security);

            var setup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.RelativeSearchPath
            };
            return AppDomain.CreateDomain("Sandbox" + DateTime.Now, null, setup, ps, typeof(TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers).Assembly.Evidence.GetHostEvidence<StrongName>());
        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
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
                foreach (var asm in Helpers.GetStandardReferences(false, true, true))
                    sandbox.Load(asm);
                // create proxy in sandbox appdomain
                // AppDomain.CurrentDomain.Load(typeof(ProxyExecuter).Assembly.GetName());
                // AppDomain.CurrentDomain.Load(typeof(TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers).Assembly.GetName());
                // AppDomain.CurrentDomain.Load(File.ReadAllBytes(typeof(ProxyExecuter).Assembly.Location));
                // AppDomain.CurrentDomain.AssemblyResolve += LoadFromSameFolder;

                ProxyExecuterClass proxy;
                try
                {
                    //new ReflectionPermission(PermissionState.Unrestricted).Assert();

                    //sandbox.AssemblyResolve += LoadFromSameFolder;

                    proxy = (ProxyExecuterClass)Activator.CreateInstance(sandbox, typeof(ProxyExecuterClass).Assembly.FullName, typeof(ProxyExecuterClass).FullName).Unwrap();
                }
                finally
                {
                    // sandbox.AssemblyResolve -= LoadFromSameFolder;

                    //     CodeAccessPermission.RevertAll();
                }

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
