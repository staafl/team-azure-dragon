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

namespace Rossie.Engine
{
    public enum CSharpCodeTemplate
    {
        Expression,
        WholeProgram,
        Class,
        Method,
        ClassBody,
        MethodBody
    }
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
        public void Execute(
            string code,
            out object result,
            out Exception exception,
            out IEnumerable<Diagnostic> compileErrors,
            out bool timedOut,
            out bool memoryCapHit,
            CSharpCodeTemplate template = CSharpCodeTemplate.Expression,
            int? timeoutSeconds = 6,
            int? memoryCapMb = 15)
        {
            result = null;
            exception = null;
            compileErrors = new Diagnostic[0];
            timedOut = false;
            memoryCapHit = false;

            // sandbox appdomain
            var sandbox = CreateSandbox();

            string entryPoint;

            // scaffold code

            #region scaffold code templates
            // expression
            string entryPointExpression =
@"using System; 
public class EntryPoint 
{ 
    public static object Result {get;set;} 
    public static Exception Exception {get;set;} 

    public static void Main() 
    {
        try
        {
            Result = Eval(); 
        }
        catch (Exception ex)
        {
            Exception = ex;
        }
    }  
    public static object Eval() { return " + code + @"; }
    
}";

            // method body
            string entryPointMethodBody =
@"using System; 
public class EntryPoint 
{ 
    public static object Result {get;set;} 
    public static Exception Exception {get;set;} 

    public static void Main() 
    {
        try
        {
            Result = Eval(); 
        }
        catch (Exception ex)
        {
            Exception = ex;
        }
    }  
    public static object Eval() { " + code + @" }
    
}";

            string entryPointMethod =
@"using System; 
using System.Reflection;
public class EntryPoint 
{ 
    public static object Result {get;set;} 
    public static Exception Exception {get;set;} 

    public static void Main() 
    {
        try
        {
            Result = Eval(); 
        }
        catch (Exception ex)
        {
            Exception = ex;
        }
    }  
    public static object Eval() { return typeof(EntryPoint).GetMethod(%MethodName%).Apply(%Arguments%);  } // apply with reflection

    " + code + @"
    
}";

            // compiling an entire program will require a different structure
            // ...

            #endregion

            switch (template)
            {
                case CSharpCodeTemplate.Expression: entryPoint = entryPointExpression; break;
                case CSharpCodeTemplate.MethodBody: entryPoint = entryPointMethodBody; break;
                case CSharpCodeTemplate.WholeProgram:
                case CSharpCodeTemplate.Class:
                case CSharpCodeTemplate.ClassBody:
                case CSharpCodeTemplate.Method: throw new NotImplementedException();
                default: throw new ArgumentException();

            }
            // parse
            var syntaxTrees = new[] { SyntaxTree.ParseText(entryPoint) };

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
                    compileErrors = emitResult.Diagnostics;
                    return;
                }

                compiledAssembly = output.ToArray();
            }

            if (compiledAssembly.Length == 0) throw new ApplicationException();

            // get proxy loader
            var loader = (ByteCodeLoader)Activator.CreateInstance(sandbox, typeof(ByteCodeLoader).Assembly.FullName, typeof(ByteCodeLoader).FullName).Unwrap();

            // run computation in sandboxed appdomain, on a separate thread
            Exception tempException = null;
            object tempResult = null;

            var scriptThread = new Thread(() =>
            {
                loader.Run(compiledAssembly, out tempResult, out tempException);
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
                }
            }

            result = tempResult;
            exception = tempException;

            // cleanup
            AppDomain.Unload(sandbox);

        }

        public object Execute(string code, CSharpCodeTemplate template = CSharpCodeTemplate.Expression, int? timeoutSeconds = 6, int? memoryCapMb = 15)
        {
            object result;
            Exception exception;
            IEnumerable<Diagnostic> compileErrors;
            bool timedOut, memoryCapHit;

            Execute(code, out result, out exception, out compileErrors, out timedOut, out memoryCapHit, template, timeoutSeconds, memoryCapMb);

            if (compileErrors.Any())
            {
                var errors = compileErrors.Select(x => x.Info.GetMessage().Replace("Eval()", "<Factory>()").ToString()).ToArray();

                return "Compilation errors: " + string.Join(", ", errors);
            }

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


            return result;
        }
    }
}
