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
        /// <summary>
        /// Compiles a piece of code to IL representing a CLR assembly with the specified code and the standard references.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="template"></param>
        /// <param name="compileErrors"></param>
        /// <returns></returns>
        public static byte[] CompileToAssembly(
            SyntaxTree syntaxTree,
            out IEnumerable<Diagnostic> compileErrors)
        {
            compileErrors = new Diagnostic[0];

            // add references
            var references = Helpers.GetStandardReferences(true, false).Select(MetadataReference.CreateAssemblyReference).ToList();

            // prepare compilation
            var options = new CompilationOptions(outputKind: OutputKind.ConsoleApplication);

            var compilation = Compilation.Create("DUMMY_NAME", options: options,
                                        syntaxTrees: new[] { syntaxTree },
                                        references: references);

            // compile and emit

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

            return compiledAssembly;
        }

    }
}
