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
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException;
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
}