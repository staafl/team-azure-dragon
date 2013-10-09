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
using TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers;

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

            var path = AppDomain.CurrentDomain.BaseDirectory;
            // TODO: HACK HACK HACK
            // IIS resolution problem - https://github.com/staafl/team-azure-dragon/issues/36 
            if (!path.ToUpper().Contains("BIN"))
                path += "bin\\";
            var setup = new AppDomainSetup
            {
                ApplicationBase = path
            };
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

            // sandbox appdomain
            var sandbox = CreateSandbox();

            try
            {
                // add references
                foreach (var asm in Helpers.GetStandardReferences(false, true, true))
                    sandbox.Load(asm);

                var proxy = (ProxyExecuterClass)Activator.CreateInstance(sandbox, typeof(ProxyExecuterClass).Assembly.FullName, typeof(ProxyExecuterClass).FullName).Unwrap();


                // run computation in sandboxed appdomain

                proxy.Run(assemblyIL, stdin, true, out result, out exception, out stdout, out timedOut, out memoryCapHit, timeoutSeconds, memoryCapMb);


            }
            finally
            {
                // cleanup
                AppDomain.Unload(sandbox);
            }

        }

    }
}
