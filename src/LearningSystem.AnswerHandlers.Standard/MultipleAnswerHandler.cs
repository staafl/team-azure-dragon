using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LearningSystem.AnswerHandlers.Reference;

namespace LearningSystem.AnswerHandlers.Standard
{
    public class MultipleAnswerHandler : IAnswerHandler
    {
        public IEnumerable<Tuple<string, bool>> Tests { get; set; }

        public string RenderInputHtml()
        {
            var sb = new StringBuilder();
            foreach (var t in Tests)
            {
                sb.AppendFormat(@"<div><label><input type='checkbox' name='answer-input[]' value='{0}'/>&nbsp;{0}</label></div>", t.Item1); // TODO: escape
            }
            return sb.ToString();
        }

        public AnswerValidationResult ValidateInput(string input)
        {
            return new AnswerValidationResult { Success = true };
        }
    }
}
