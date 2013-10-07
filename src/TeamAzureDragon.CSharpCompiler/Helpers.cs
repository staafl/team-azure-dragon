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
    [SecuritySafeCritical]
    public static class Helpers
    {
        public static IEnumerable<string> GetStandardReferences(bool includeMsCorLib, bool includeSecuritySafeHelpers, bool includeProxyExecuter)
        {
            if (includeMsCorLib)
                yield return typeof(object).Assembly.FullName;

            if (includeSecuritySafeHelpers)
                yield return typeof(TeamAzureDragon.CSharpCompiler.SecuritySafeHelpers.Helpers).Assembly.FullName;

            if (includeProxyExecuter)
                yield return typeof(TeamAzureDragon.CSharpCompiler.Proxy.Class1).Assembly.FullName;


            yield return "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            yield return "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        }

        /// <summary>
        /// Useful for checking code for unwanted features.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="act"></param>
        public static void WalkSyntaxTree(string code, Action<SyntaxNode> act)
        {
            var syntax = Syntax.ParseStatement(code);

            WalkSyntaxTree(syntax, act);
        }

        public static void WalkSyntaxTree(SyntaxNode syntaxNode, Action<SyntaxNode> act)
        {
            act(syntaxNode);
            foreach (var childNode in syntaxNode.ChildNodes())
                WalkSyntaxTree(childNode, act);
        }



        public static MetadataReference GetCompilerAssemblyReference()
        {
            return new MetadataFileReference(typeof(Compiler).Assembly.Location);
        }
    }
}
