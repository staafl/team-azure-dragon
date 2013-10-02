using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LearningSystem.App.AppLogic
{
    public class TextAnswerHandler : IAnswerHandler
    {
        // version 0:

        public bool IgnoreCase { get; set; }
        public bool NormalizeWhiteSpace { get; set; }
        public string Text { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            if (this.NormalizeWhiteSpace)
                input = Regex.Replace(input, @"\s+", " ").Trim();

            if (String.Compare(input, this.Text, this.IgnoreCase) == 0)
                return new AnswerValidationResult { Success = true };
            else
                return new AnswerValidationResult
                {
                    Success = false,
                    ErrorContent = "<span class='answer-error-content'>Your answer was not correct.</span>"
                };
        }

        public string RenderInputHtml()
        {
            return
@"<input type='text' id='answer-input' name='answer-input' class='answer-input-textfield' />";
        }
    }
}
