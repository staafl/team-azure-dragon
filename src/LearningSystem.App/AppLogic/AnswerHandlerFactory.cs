using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using TeamAzureDragon.Utils;
using TeamAzureDragon.CSharpCompiler;
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
    public static class AnswerHandlerFactory
    {
        static public IAnswerHandler GetHandler(AnswerType type, string answerContent, int version = 0)
        {
            // TODO: arg validation
            switch (type)
            {
                case AnswerType.None:
                    return new NoneAnswerHandler(); // throw new ArgumentException("'type' cannot be None.");
                case AnswerType.Custom:
                case AnswerType.CSharpCode:
                    return GetCSharpAnswerHandler(answerContent);
                case AnswerType.Text:
                    return GetTextAnswerHandler(answerContent);
                case AnswerType.ApproximateText:
                case AnswerType.Multiple:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unsupported type: " + type);
            }
        }

        static readonly string options = String.Join("|", (CSharpCodeTemplate[])Enum.GetValues(typeof(CSharpCodeTemplate)));

        static IAnswerHandler GetCSharpAnswerHandler(string answerContent, int version = 0)
        {
            if (version == 0)
            {
                var match = Regex.Match(answerContent,
                    @"^(?ix)0;(?<template>" + options + @");(?<normalize>true|false);(?<tests>[^~]+~)+$");

                if (!match.Success)
                    throw new ArgumentException("failed to match");

                var template = (CSharpCodeTemplate)Enum.Parse(typeof(CSharpCodeTemplate), match.Groups["template"].Value);
                var normalize = bool.Parse(match.Groups["normalize"].Value);
                var tests = match.Groups["tests"].Captures.Cast<Capture>().Select(c => c.Value.Substring(0, c.Value.Length - 1));

                return new CSharpAnswerHandler
                {
                    CodeTemplate = template,
                    NormalizeLines = normalize,
                    Tests = tests
                };
            }

            throw new ArgumentException("version");
        }


        static IAnswerHandler GetTextAnswerHandler(string data, int version = 0)
        {
            bool ignoreCase;
            bool normalize;
            string text;
            if (version == 0)
            {
                ignoreCase = true;
                normalize = true;
                text = data;
            }
            else if (version == 1)
            {
                var match = Regex.Match(data, @"(?ix)^1;(?<ignore>true|false);(?<normalize>true|false);(?<text>.*?);?$");

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

    }
}