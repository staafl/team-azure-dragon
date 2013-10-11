using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LearningSystem.AnswerHandlers.Reference;
using TeamAzureDragon.Utils;

namespace LearningSystem.AnswerHandlers.Standard
{
    public class ListAnswerHandler : IAnswerHandler
    {
        public int RequiredCount { get; set; }

        public IEnumerable<string> Tests { get; set; }

        public AnswerValidationResult ValidateInput(string input)
        {
            var split = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(Misc.Normalize)
                .Distinct();

            var correct = split.Intersect(
                this.Tests.Select(Misc.Normalize)).Count();
            var wrong = split.Except(
                this.Tests.Select(Misc.Normalize)).Count();

            if (wrong == 0)
            {
                if (correct >= this.RequiredCount)
                {
                    return new AnswerValidationResult
                    {
                        Success = true
                    };
                }
                else
                {
                    return new AnswerValidationResult
                    {
                        Success = false,
                        ErrorContent = String.Format("You only gave {0} answers!", correct)
                    };
                }
            }
            else
            {
                return new AnswerValidationResult
                {
                    Success = false,
                    ErrorContent = String.Format("{0} of your entries were wrong (you got {1} right)!", wrong, correct)
                };
            }

        }

        public string RenderInputHtml()
        {
            return
@"<input type='text' id='answer-input' name='answer-input' class='answer-input-textfield input-code' />";
        }


        public const string TypeIdentifier = "List";

        public static IAnswerHandler GetAnswerHandler(string answerContent)
        {
            var match = Regex.Match(answerContent,
                 @"^(?ix)0;(?<requiredCount>\d+);(?<tests>[^~]+~?)+$");

            if (!match.Success)
                throw new ArgumentException("failed to match");

            var requiredCount = int.Parse(match.Groups["requiredCount"].Value);
            var tests = Misc.GetTildeList(match.Groups["tests"]);

            return new ListAnswerHandler
            {
                RequiredCount = requiredCount,
                Tests = tests,
            };

            throw new ArgumentException("version");
        }

    }
}
