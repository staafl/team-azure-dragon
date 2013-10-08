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
    public static class CSharpCodeBuilder
    {
        public static string ToProgramSource(string code, CSharpCodeTemplate template)
        {
            string program;
            // scaffold code

            switch (template)
            {
                case CSharpCodeTemplate.Expression: program = entryPointExpression; break;
                case CSharpCodeTemplate.MethodBody: program = entryPointMethodBody; break;
                case CSharpCodeTemplate.Statements: program = entryPointStatements; break;
                case CSharpCodeTemplate.WholeProgram: program = code; break;
                case CSharpCodeTemplate.Class:
                case CSharpCodeTemplate.ClassBody:
                case CSharpCodeTemplate.Method: throw new NotImplementedException();
                default: throw new ArgumentException();

            }

            if (template != CSharpCodeTemplate.WholeProgram)
            {

                var usings = 
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;";

                program = usings + "\n" + program.Replace("####", code);
            }

            return program;
        }

        #region scaffold code templates
        // expression
        const string entryPointExpression =
@"public class EntryPoint 
{ 
    public static object Result {get;set;} 

    public static void Main() 
    {
        Result = ______(); 
    }  
    public static object ______() { return ####; }
    
}";

        // method body
        const string entryPointMethodBody =
@"public class EntryPoint 
{ 
    public static object Result {get;set;} 

    public static void Main() 
    {
        Result = ______(); 
    }  
    public static object ______() { #### }
    
}";

        const string entryPointStatements =
@"public class EntryPoint 
{ 
    public static void Main() 
    {
        #### 
    }  
}";

        const string entryPointMethod =
@"public class EntryPoint 
{ 
    public static object Result {get;set;} 

    public static void Main() 
    {
        Result = ______(); 
    }  
    public static object ______() { return typeof(EntryPoint).GetMethod(%MethodName%).Apply(%Arguments%);  } // apply with reflection

    ####
    
}";

        // compiling an entire program will require a different structure
        // ...

        #endregion
    }
}
