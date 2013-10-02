using System.Collections.Generic;
using Roslyn.Compilers.CSharp;
using System.Linq;
namespace Rossie.Engine
{
    public static class Validator
    {
        public static bool Validate(string code)
        {
            var syntax = Syntax.ParseStatement(code);

            return ScanSyntax(syntax.ChildNodes());
        }

        public static bool ScanSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                if (syntaxNode.ChildNodes().Any())
                {
                    if (!ScanSyntax(syntaxNode.ChildNodes())) return false;
                }
                if (syntaxNode is QualifiedNameSyntax || syntaxNode is InvocationExpressionSyntax && ((InvocationExpressionSyntax)syntaxNode).Expression is MemberAccessExpressionSyntax) return false;
            }

            return true;
        }
    }
}
