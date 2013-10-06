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
    public static class ExecutionDirector
    {
        public static object RunAndReport(string code, CSharpCodeTemplate template, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            bool success;
            return RunAndReport(code, out success, template, timeoutSeconds, memoryCapMb);
        }
        public static object RunAndReport(string code, out bool success, CSharpCodeTemplate template, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {

            success = false;
            IEnumerable<Diagnostic> compileErrors;
            // CSharpCodeTemplate template,

            // prepare code
            string program = CSharpCodeBuilder.ToProgramSource(code, template);

            // parse
            var syntaxTree = SyntaxTree.ParseText(program);

            // todo: validate using Helpers.WalkSyntaxNode

            var asm = Compiler.CompileToAssembly(syntaxTree, out compileErrors);

            if (compileErrors.Any())
            {
                var errors = compileErrors.Select(x => x.Info.GetMessage().Replace("______()", "<Factory>()").ToString()).ToArray();

                return "Compilation errors: " + string.Join(", ", errors);
            }

            return RunAndReport(asm, out success, timeoutSeconds, memoryCapMb);

        }
        public static object RunAndReport(byte[] assemblyIL, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            bool success;
            return RunAndReport(assemblyIL, out success, timeoutSeconds, memoryCapMb);
        }
        public static object RunAndReport(byte[] assemblyIL, out bool success, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            object result;
            Exception exception;
            bool timedOut, memoryCapHit;
            string stdout;
            string stdin = null;

            success = false;

            AssemblyExecuter.ExecuteAssembly(assemblyIL, stdin, out result, out exception, out stdout, out timedOut, out memoryCapHit, timeoutSeconds, memoryCapMb);

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
