using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace TeamAzureDragon.CSharpCompiler.ProxyExecuter
{

    /// <summary>
    /// Todo:
    /// * capture console output
    /// * pipe console input
    /// * timeout
    /// * memory cap
    /// </summary>
    public sealed class ProxyExecuterClass : MarshalByRefObject
    {
        public ProxyExecuterClass()
        {
        }

        public void Run(byte[] compiledAssembly, string stdin, bool getResult, out object result, out Exception exception, out string stdout, out bool timedOut, out bool memoryCapHit, int? timeOutSeconds = 6, int? memoryCapMb = 15)
        {
            Exception tempException = null;

            result = null;
            exception = null;
            timedOut = false;
            memoryCapHit = false;

            exception = null;
            result = null;
            stdout = null;

            StringWriter stdoutWriter = null;
            TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers.SafeSetStdInOut(stdin, out stdoutWriter);

            var assembly = Assembly.Load(compiledAssembly);


            var scriptThread = new Thread(() =>
            {
                try
                {
                    assembly.EntryPoint.Invoke(null, new object[] { });
                }
                catch (TargetInvocationException ex)
                {
                    tempException = ex.InnerException;
                }
            });

            scriptThread.Start();
            var sw = Stopwatch.StartNew();

            while (true)
            {
                // todo: resolution?
                if (scriptThread.Join(TimeSpan.FromMilliseconds(100)))
                    break;

                var frameCount = TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers.GetStackTraceDepth(scriptThread);
                if (frameCount > 500)
                {
                    TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers.AbortThread(scriptThread);
                    exception = new StackOverflowException();
                    break;
                }

                // todo: check sandbox memory consumption

                if (sw.ElapsedMilliseconds > timeOutSeconds * 1000)
                {

                    TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers.AbortThread(scriptThread);
                    timedOut = true;
                    break;
                }
            }

            if (exception == null)
                exception = tempException;

            if (stdoutWriter != null)
            {
                if (stdoutWriter.GetStringBuilder().Length > 100000)
                {
                    exception = exception ?? new OutOfMemoryException();
                }
                else
                {
                    stdout = stdoutWriter.ToString();
                }
            }

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
}

/*

}*/