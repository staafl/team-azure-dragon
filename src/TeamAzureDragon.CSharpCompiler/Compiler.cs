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
    // http://blog.filipekberg.se/2011/12/08/hosted-execution-of-smaller-code-snippets-with-roslyn/
    public static class Compiler
    {
        // todo: cache?
        public static byte[] CompileToAssembly(
            string code,
            CSharpCodeTemplate template,
            out IEnumerable<Diagnostic> compileErrors)
        {
            compileErrors = new Diagnostic[0];

            string program = CSharpCodeBuilder.MakeProgram(code, template);

            // parse
            var syntaxTrees = new[] { SyntaxTree.ParseText(program) };

            // add references

            var references = new[] { 
                MetadataReference.CreateAssemblyReference(typeof(object).Assembly.FullName), // mscorlib
                MetadataReference.CreateAssemblyReference("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"), 
                MetadataReference.CreateAssemblyReference("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
            };


            // compile

            var options = new CompilationOptions(outputKind: OutputKind.ConsoleApplication);

            var compilation = Compilation.Create("DUMMY_NAME", options: options,
                                        syntaxTrees: syntaxTrees,
                                        references: references);

            // emit

            byte[] compiledAssembly;
            using (var output = new MemoryStream())
            {
                var emitResult = compilation.Emit(output);

                if (!emitResult.Success)
                {
                    compileErrors = emitResult.Diagnostics;
                    return null;
                }

                compiledAssembly = output.ToArray();
            }

            if (compiledAssembly.Length == 0) throw new ApplicationException();

            return compiledAssembly;

            
        }

    }
}
