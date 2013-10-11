using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamAzureDragon.Utils;

namespace LearningSystem.App.AppLogic
{
    public class NoneAnswerHandler : IAnswerHandler
    {
        public AnswerValidationResult ValidateInput(string input)
        {
            return new AnswerValidationResult { Success = true };
        }

        public string RenderInputHtml()
        {
            return "";
        }
    }
}
