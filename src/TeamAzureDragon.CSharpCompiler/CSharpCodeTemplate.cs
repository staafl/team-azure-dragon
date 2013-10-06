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
    public enum CSharpCodeTemplate
    {
        Expression,
        /// <summary>Not Implemented.</summary>
        WholeProgram,
        /// <summary>Not Implemented. Entire program.</summary>
        Class,
        /// <summary>Not Implemented. Entire method + signature.</summary>
        Method,
        /// <summary>Not Implemented. Several Methods.</summary>
        ClassBody,
        /// <summary>Bunch of statements terminated with return.</summary>
        MethodBody,
        /// <summary>Not Implemented. Bunch of statements.</summary>
        Statements
    }
}
