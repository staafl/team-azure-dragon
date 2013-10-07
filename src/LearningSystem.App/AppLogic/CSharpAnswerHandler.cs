using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TeamAzureDragon.CSharpCompiler;
namespace LearningSystem.App.AppLogic
{
    public class CSharpAnswerHandler : IAnswerHandler
    {
        public CSharpCodeTemplate CodeTemplate { get; set; }

        public IEnumerable<string> Tests { get; set; }

        public bool NormalizeLines { get; set; }

        public Func<object, bool> Predicate { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            bool ranOk;
            var result = ExecutionDirector.RunAndReport(input, out ranOk, this.CodeTemplate);
            if (ranOk)
            {
                if (result == null || result.ToString() != Tests.FirstOrDefault())
                    return new AnswerValidationResult { Success = false, ErrorContent = "Wrong answer!" };

                return new AnswerValidationResult { Success = true };
            }
            else
                return new AnswerValidationResult { Success = false, ErrorContent = result + "" };
        }

        public string RenderInputHtml()
        {
            // todo: duplication
            if (this.CodeTemplate == CSharpCodeTemplate.WholeProgram)
            {
                return @"<textarea id='answer-input' name='answer-input' class='answer-input-textfield input-code' />";

            }

            return
@"<input type='text' id='answer-input' name='answer-input' class='answer-input-textfield input-code' />";
        }
    }
}
