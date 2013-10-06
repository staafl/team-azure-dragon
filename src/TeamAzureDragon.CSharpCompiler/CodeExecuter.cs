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
    public class CodeExecuter
    {
        /// <summary>
        /// Todo:
        /// * capture console output
        /// * pipe console input
        /// * timeout
        /// * memory cap
        /// </summary>
        public sealed class ByteCodeLoader : MarshalByRefObject
        {
            public ByteCodeLoader()
            {
            }

            public void Run(byte[] compiledAssembly, out object result, out Exception ex)
            {
                var assembly = Assembly.Load(compiledAssembly);
                assembly.EntryPoint.Invoke(null, new object[] { });
                var type = assembly.GetType("EntryPoint");
                result = type.GetProperty("Result").GetValue(null, null);
                ex = (Exception)type.GetProperty("Exception").GetValue(null, null);
            }
        }

        static AppDomain CreateSandbox()
        {
            var e = new Evidence();
            e.AddHostEvidence(new Zone(SecurityZone.Internet));

            var ps = SecurityManager.GetStandardSandbox(e);
            var security = new SecurityPermission(SecurityPermissionFlag.Execution); // SecurityPermissionFlag.Assert

            ps.AddPermission(security);

            var setup = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
            return AppDomain.CreateDomain("Sandbox" + DateTime.Now, null, setup, ps);
        }

        public void ExecuteAssembly(
            byte[] assemblyIL,
            out object result,
            out Exception exception,
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
                sandbox.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                sandbox.Load("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

                // compile

                // get proxy loader
                var loader = (ByteCodeLoader)Activator.CreateInstance(sandbox, typeof(ByteCodeLoader).Assembly.FullName, typeof(ByteCodeLoader).FullName).Unwrap();

                // run computation in sandboxed appdomain, on a separate thread
                Exception tempException = null;
                object tempResult = null;

                var scriptThread = new Thread(() =>
                {
                    loader.Run(assemblyIL, out tempResult, out tempException);
                });

                scriptThread.Start();

                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (scriptThread.Join(1000))
                        break;

                    // todo: check sandbox memory consumption

                    if (sw.ElapsedMilliseconds > timeoutSeconds * 1000)
                    {
                        scriptThread.Abort();
                        timedOut = true;
                        break;
                    }
                }

                result = tempResult;
                exception = tempException;
            }
            finally
            {
                // cleanup
                AppDomain.Unload(sandbox);
            }

        }

        public object RunAndReport(string code, CSharpCodeTemplate template, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            bool success;
            return RunAndReport(code, out success, template, timeoutSeconds, memoryCapMb);
        }

        public object RunAndReport(string code, out bool success, CSharpCodeTemplate template, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {

            success = false;
            IEnumerable<Diagnostic> compileErrors;

            var asm = Compiler.CompileToAssembly(code, template, out compileErrors);

            if (compileErrors.Any())
            {
                var errors = compileErrors.Select(x => x.Info.GetMessage().Replace("______()", "<Factory>()").ToString()).ToArray();

                return "Compilation errors: " + string.Join(", ", errors);
            }

            return RunAndReport(asm, out success, timeoutSeconds, memoryCapMb);

        }

        public object RunAndReport(byte[] assemblyIL, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            bool success;
            return RunAndReport(assemblyIL, out success, timeoutSeconds, memoryCapMb);
        }

        public object RunAndReport(byte[] assemblyIL, out bool success, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            object result;
            Exception exception;
            bool timedOut, memoryCapHit;

            success = false;

            ExecuteAssembly(assemblyIL, out result, out exception, out timedOut, out memoryCapHit, timeoutSeconds, memoryCapMb);

            if (exception != null)
            {
                return "Unhandled exception: " + exception.Message;
            }

            if (timedOut)
            {
                return "Program timed out!";
            }

            if (memoryCapHit)
            {
                return "Program hit memory limit!";
            }

            success = true;

            return result;
        }
    }
}
