using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LearningSystem.AnswerHandlers.Reference;
using TeamAzureDragon.Utils;

namespace LearningSystem.AnswerHandlers.Standard
{
    public class NoneAnswerHandler : IAnswerHandler
    {
        public const string TypeIdentifier = "None";

        public static IAnswerHandler GetAnswerHandler(string answerContent)
        {
            return new NoneAnswerHandler();
        }

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
