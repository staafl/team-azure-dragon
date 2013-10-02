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

namespace Rossie.Engine
{
    public sealed class ByteCodeLoader : MarshalByRefObject
    {
        public ByteCodeLoader()
        {
        }

        public object Run(byte[] compiledAssembly)
        {
            var assembly = Assembly.Load(compiledAssembly);
            assembly.EntryPoint.Invoke(null, new object[] { });
            var result = assembly.GetType("EntryPoint").GetProperty("Result").GetValue(null, null);

            return result;
        }
    }

    // http://blog.filipekberg.se/2011/12/08/hosted-execution-of-smaller-code-snippets-with-roslyn/
    public class CodeExecuter
    {
        private static AppDomain CreateSandbox()
        {
            var e = new Evidence();
            e.AddHostEvidence(new Zone(SecurityZone.Internet));

            var ps = SecurityManager.GetStandardSandbox(e);
            var security = new SecurityPermission(SecurityPermissionFlag.Execution);

            ps.AddPermission(security);

            var setup = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
            return AppDomain.CreateDomain("Sandbox", null, setup, ps);
        }
        public object Execute(string code, int timeout = 6000)
        {
            // sandbox appdomain
            var sandbox = CreateSandbox();

            // scaffold code
             string entryPoint =
                "using System.Reflection; public class EntryPoint { public static object Result {get;set;} public static void Main() { Result = Eval(); }  public static object Eval() {" + code + "} }";

            string script = "public static object Eval() {" + code + "}";
            
            // parse
            var syntaxTrees = new[] { SyntaxTree.ParseText(entryPoint), 
                // SyntaxTree.ParseText(script, options: new ParseOptions(kind: SourceCodeKind.Interactive))
            };

            // add references
            var core = sandbox.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            var system = sandbox.Load("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

            var references = new[] { 
                MetadataReference.CreateAssemblyReference(typeof(object).Assembly.FullName),
                MetadataReference.CreateAssemblyReference(core.FullName), 
                MetadataReference.CreateAssemblyReference(system.FullName)
            };

            
            // compile

            var usings = new[] { "System", 
                                    "System.IO", 
                                    "System.Net", 
                                    "System.Linq", 
                                    "System.Text", 
                                    "System.Text.RegularExpressions", 
                                    "System.Collections.Generic" };

            var options = new CompilationOptions(outputKind: OutputKind.ConsoleApplication, usings: usings);

            var compilation = Compilation.Create("foo", options: options,
                                        syntaxTrees: syntaxTrees,
                                        references: references);

            var ep = compilation.GetEntryPoint(default(System.Threading.CancellationToken));

            // emit

            byte[] compiledAssembly;
            using (var output = new MemoryStream())
            {
                var emitResult = compilation.Emit(output);

                if (!emitResult.Success)
                {
                    var errors = emitResult.Diagnostics.Select(x => x.Info.GetMessage().Replace("Eval()", "<Factory>()").ToString()).ToArray();

                    return string.Join(", ", errors);
                }

                compiledAssembly = output.ToArray();
            }

            if (compiledAssembly.Length == 0) return "Incorrect data";

            // get proxy loader
            var loader = (ByteCodeLoader)Activator.CreateInstance(sandbox, typeof(ByteCodeLoader).Assembly.FullName, typeof(ByteCodeLoader).FullName).Unwrap();

            // run computation in sandboxed appdomain, on a separate thread
            Exception hex = null;
            object result = null;
            try
            {
                var scriptThread = new Thread(() =>
                {
                    try
                    {
                        result = loader.Run(compiledAssembly);
                    }
                    catch (Exception ex)
                    {
                        hex = ex;
                        result = ex.Message;
                    }
                });

                scriptThread.Start();

                if (!scriptThread.Join(timeout))
                {
                    scriptThread.Abort();
                    AppDomain.Unload(sandbox);
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }

            // cleanup
            AppDomain.Unload(sandbox);

            if (result == null || string.IsNullOrEmpty(result.ToString())) result = "null";

            return result;
        }
    }
}
