using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LearningSystem.AnswerHandlers.Reference;
using TeamAzureDragon.Utils;

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

        public static string TypeIdentifier = "Multiple";

        public static IAnswerHandler GetAnswerHandler(string answerContent, int version = 0)
        {
            if (version == 0)
            {
                var match = Regex.Match(answerContent, @"^(?ix)0;(?<tests>[^~]+~[^~]+~?)+$");

                if (!match.Success)
                    throw new ArgumentException("failed to match");

                var tests = Misc.GetTildeList(match.Groups["tests"]).Select(
                    s =>
                    {
                        var split = s.Split('~');
                        return Tuple.Create(split[0], split[1] == "1" ? true : false);
                    }).ToList();

                return new MultipleAnswerHandler
                {
                    Tests = tests
                };
            }

            throw new ArgumentException("version");
        }
    }
}
