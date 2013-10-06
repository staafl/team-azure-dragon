using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
//[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityCritical]
// [assembly: SecurityRules(SecurityRuleSet.Level1)]

namespace TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers
{
    [SecuritySafeCritical]
    public static class Helpers
    {
        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static void SafeSetStdInOut(string stdin, out StringWriter stdoutWriter)
        {
       //     new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
            try
            {
                if (stdin != null)
                    Console.SetIn(new StringReader(stdin));
                stdoutWriter = new StringWriter();
                Console.SetOut(stdoutWriter);
            }
            finally
            {
        //        CodeAccessPermission.RevertAll();
            }
        }
    }
}
