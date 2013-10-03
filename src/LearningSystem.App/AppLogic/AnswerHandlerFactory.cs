using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using TeamAzureDragon.Utils;
using Rossie.Engine;
namespace LearningSystem.App.AppLogic
{
    public static class AnswerHandlerFactory
    {
        static public IAnswerHandler GetHandler(AnswerType type, string answerContent, int version = 0)
        {
            // TODO: arg validation
            switch (type)
            {
                case AnswerType.None:
                    throw new ArgumentException("'type' cannot be None.");
                case AnswerType.Custom:
                case AnswerType.CSharpCode:
                    return GetCSharpAnswerHandler(answerContent);
                case AnswerType.Text:
                    return GetTextAnswerHandler(answerContent);
                case AnswerType.ApproximateText:
                case AnswerType.MultipleChoise:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unsupported type: " + type);
            }
        }

        static IAnswerHandler GetCSharpAnswerHandler(string answerContent, int version = 0)
        {
            if (version == 0)
            {
                var match = Regex.Match(answerContent,
                    @"^(?ix)0;(Expression|WholeProgram|Class|Method);(true|false);(?<tests>[^~]+~)+");

                if (!match.Success)
                    throw new ApplicationException("failed to match");

                var template = (CSharpCodeTemplate)Enum.Parse(typeof(CSharpCodeTemplate), match.Groups[0].Value);
                var normalize = bool.Parse(match.Groups[1].Value);
                var tests = match.Groups["tests"].Captures.Cast<Capture>().Select(c => c.Value);

                return new CSharpAnswerHandler
                {
                    CodeTemplate = template,
                    NormalizeLines = normalize,
                    Tests = tests
                };
            }
            throw new ApplicationException();
        }


        static IAnswerHandler GetTextAnswerHandler(string data, int version = 0)
        {
            return new TextAnswerHandler { Text = data, IgnoreCase = true, NormalizeWhiteSpace = true };
        }

    }
}