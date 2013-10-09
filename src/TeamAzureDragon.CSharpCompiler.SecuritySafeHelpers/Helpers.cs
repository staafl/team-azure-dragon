using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
[assembly: AllowPartiallyTrustedCallers]
namespace TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers
{

    [SecuritySafeCritical]
    public static class Helpers
    {
        [SecuritySafeCritical]
        static public int GetStackTraceDepth(Thread thread)
        {
            if (thread.ThreadState != System.Threading.ThreadState.Running)
                return 0;

            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlThread).Assert();

            try
            {
                // todo: better granularity? 
#pragma warning disable // we know what we're doing
                thread.Suspend();
#pragma warning restore

#pragma warning disable
                var trace = new StackTrace(thread, true);
#pragma warning restore
                return trace.FrameCount;
            }
            finally
            {
                try
                {
#pragma warning disable
                    thread.Resume();
#pragma warning restore
                }
                finally
                {
                    CodeAccessPermission.RevertAll();
                }
            }
        }
        [SecuritySafeCritical]
        public static void AbortThread(Thread thread)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlThread).Assert();
            try
            {
                thread.Abort();
            }
            finally
            {
                CodeAccessPermission.RevertAll();
            }
        }
        [SecuritySafeCritical]
        public static void SafeSetStdInOut(string stdin, out StringWriter stdoutWriter)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
            try
            {
                if (stdin != null)
                    Console.SetIn(new StringReader(stdin));
                stdoutWriter = new StringWriter();
                Console.SetOut(stdoutWriter);
            }
            finally
            {
                CodeAccessPermission.RevertAll();
            }
        }
    }
}
