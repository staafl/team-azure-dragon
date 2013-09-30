using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LearningSystem.App.AppLogic
{
    public enum CSharpCodeTemplate
    {
        Expression,
        WholeProgram,
        Class,
        Method
    }
    public class CSharpAnswerHandler : IAnswerHandler
    {
        public CSharpCodeTemplate CodeTemplate { get; set; }

        public IEnumerable<string> Tests { get; set; }

        public bool NormalizeLines { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            throw new NotImplementedException();
        }

        public string RenderInputHtml()
        {
            // todo: duplication
            return
@"<input type='text' name='answer-input' class='answer-input-textfield input-code' />";
        }
    }
}
