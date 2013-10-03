using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rossie.Engine;
namespace LearningSystem.App.AppLogic
{
    public class CSharpAnswerHandler : IAnswerHandler
    {
        public CSharpCodeTemplate CodeTemplate { get; set; }

        public IEnumerable<string> Tests { get; set; }

        public bool NormalizeLines { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            var executer = new Rossie.Engine.CodeExecuter();

            bool ranOk;
            var result = executer.Execute(input, out ranOk, this.CodeTemplate);
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
            return
@"<input type='text' name='answer-input' class='answer-input-textfield input-code' />";
        }
    }
}
