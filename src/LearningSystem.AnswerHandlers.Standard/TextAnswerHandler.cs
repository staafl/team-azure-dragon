using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LearningSystem.AnswerHandlers.Reference;
using TeamAzureDragon.Utils;

namespace LearningSystem.AnswerHandlers.Standard
{
    public class TextAnswerHandler : IAnswerHandler
    {
        public static string TypeIdentifier = "Text";

        public static IAnswerHandler GetAnswerHandler(string answerContent, int version = 0)
        {
            bool ignoreCase;
            bool normalize;
            string text;
            if (version == 0)
            {
                ignoreCase = true;
                normalize = true;
                text = answerContent;
            }
            else if (version == 1)
            {
                var match = Regex.Match(answerContent, @"(?ix)^1;(?<ignore>true|false);(?<normalize>true|false);(?<text>.*?);?$");

                if (!match.Success)
                    throw new ArgumentException("failed to match");

                ignoreCase = bool.Parse(match.Groups["ignore"].Value);
                normalize = bool.Parse(match.Groups["normalize"].Value);
                text = match.Groups["text"].Value;
            }
            else
            {
                throw new ArgumentException("version");
            }
            return new TextAnswerHandler { Text = text, IgnoreCase = ignoreCase, NormalizeWhiteSpace = normalize };
        }


        public bool IgnoreCase { get; set; }
        public bool NormalizeWhiteSpace { get; set; }
        public string Text { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            if (this.NormalizeWhiteSpace)
                input = input.NormalizeWhiteSpace();

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
