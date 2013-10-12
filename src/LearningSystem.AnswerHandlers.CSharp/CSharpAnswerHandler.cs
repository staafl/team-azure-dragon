using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LearningSystem.AnswerHandlers.Reference;
using TeamAzureDragon.CSharpCompiler;
using TeamAzureDragon.Utils;
namespace LearningSystem.AnswerHandlers.CSharp
{
    public enum CSharpCodeValidation
    {
        Result,
        StdOut
    }

    public class CSharpAnswerHandler : IAnswerHandler
    {
        public CSharpCodeTemplate CodeTemplate { get; set; }

        public IEnumerable<string> Tests { get; set; }

        public bool NormalizeLines { get; set; }

        public CSharpCodeValidation Validation { get; set; }

        // reserved
        // public Func<object, bool> Predicate { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            bool ranOk;
            string stdout;
            var inputs = this.Inputs;
            if (!inputs.Any())
                inputs = new[] { "" };
            foreach (var stdin in inputs)
            {
                var result = ExecutionDirector.RunAndReport(input, out ranOk, stdin, out stdout, this.CodeTemplate, timeoutSeconds: 6, memoryCapMb: 10);
                var toValidate = this.Validation == CSharpCodeValidation.Result ? result : stdout;

                if (ranOk)
                {
                    if (this.CodeTemplate == CSharpCodeTemplate.WholeProgram)
                    {
                        return new AnswerValidationResult { Success = false, ErrorContent = "Result: " + result + "<br/>Program output: " + stdout };
                    }
                    if (toValidate == null || toValidate.ToString() != Tests.FirstOrDefault())
                        return new AnswerValidationResult { Success = false, ErrorContent = "Wrong answer!" };

                }
                else
                    return new AnswerValidationResult { Success = false, ErrorContent = result + "" };
            }
            return new AnswerValidationResult { Success = true };

        }

        public string RenderInputHtml()
        {
            // todo: duplication
            if (this.CodeTemplate == CSharpCodeTemplate.WholeProgram ||
                this.CodeTemplate == CSharpCodeTemplate.Statements)
            {
                return @"<textarea id='answer-input' name='answer-input' class='answer-input-textfield input-code'></textarea>";

            }

            return
@"<input type='text' id='answer-input' name='answer-input' class='answer-input-textfield input-code' />";
        }


        public IEnumerable<string> Inputs { get; set; }

        public const string TypeIdentifier = "CSharp";

        public static IAnswerHandler GetAnswerHandler(string answerContent)
        {
            var match = Regex.Match(answerContent,
                @"^(?ix)0;" +
                @"(?<template>" + Misc.EnumOptions<CSharpCodeTemplate>() + @");" +
                @"(?<normalize>true|false);" +
                @"(?<validation>" + Misc.EnumOptions<CSharpCodeValidation>() + @");" +
                @"(?<tests>[^;~]+~)+" +
                @"(;(?<inputs>[^~]+~)+)?" +
                @"\s*$", RegexOptions.ExplicitCapture);

            // todo: [;~] is problematic

            if (!match.Success)
                throw new ArgumentException("failed to match");

            var template = match.ParseEnum<CSharpCodeTemplate>("template");
            var validation = match.ParseEnum<CSharpCodeValidation>("template");
            var normalize = bool.Parse(match.Groups["normalize"].Value);
            var tests = match.Groups["tests"].GetTildeList();
            var inputs = match.Groups["tests"].GetTildeList();

            return new CSharpAnswerHandler
            {
                CodeTemplate = template,
                NormalizeLines = normalize,
                Tests = tests,
                Validation = validation,
                Inputs = inputs
            };
        }

    }
}
