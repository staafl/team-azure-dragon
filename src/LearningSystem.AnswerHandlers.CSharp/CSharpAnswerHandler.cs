using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TeamAzureDragon.CSharpCompiler;
namespace LearningSystem.App.AppLogic
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
            if (this.CodeTemplate == CSharpCodeTemplate.WholeProgram)
            {
                return @"<textarea id='answer-input' name='answer-input' class='answer-input-textfield input-code'></textarea>";

            }

            return
@"<input type='text' id='answer-input' name='answer-input' class='answer-input-textfield input-code' />";
        }


        public IEnumerable<string> Inputs { get; set; }
    }
}
