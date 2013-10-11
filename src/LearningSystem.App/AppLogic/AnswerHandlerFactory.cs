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


    public static class AnswerHandlerFactory
    {
        static public IAnswerHandler GetHandler(AnswerType type, string answerContent, int version = 0)
        {
            // TODO: arg validation
            switch (type)
            {
                case AnswerType.None:
                    return new NoneAnswerHandler(); // throw new ArgumentException("'type' cannot be None.");
                case AnswerType.CSharpCode:
                    return GetCSharpAnswerHandler(answerContent, version);
                case AnswerType.Text:
                    return GetTextAnswerHandler(answerContent, version);
                case AnswerType.List:
                    return GetListAnswerHandler(answerContent, version);
                case AnswerType.Multiple:
                    return GetMultipleAnswerHandler(answerContent, version);
                case AnswerType.Custom:
                case AnswerType.ApproximateText:
                //throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unsupported type: " + type);
            }
        }


        static IAnswerHandler GetMultipleAnswerHandler(string answerContent, int version = 0)
        {
            if (version == 0)
            {
                if (version == 0)
                {
                    var match = Regex.Match(answerContent, @"^(?ix)0;(?<tests>[^~]+~[^~]+~?)+$");

                    if (!match.Success)
                        throw new ArgumentException("failed to match");

                    var tests = GetTildeList(match.Groups["tests"]).Select(
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


            }
            throw new ArgumentException("version");
        }

        static IAnswerHandler GetListAnswerHandler(string answerContent, int version = 0)
        {
            if (version == 0)
            {
                var match = Regex.Match(answerContent,
                     @"^(?ix)0;(?<requiredCount>\d+);(?<tests>[^~]+~?)+$");

                if (!match.Success)
                    throw new ArgumentException("failed to match");

                var requiredCount = int.Parse(match.Groups["requiredCount"].Value);
                var tests = GetTildeList(match.Groups["tests"]);

                return new ListAnswerHandler
                {
                    RequiredCount = requiredCount,
                    Tests = tests,
                };

            }
            throw new ArgumentException("version");
        }

        static IAnswerHandler GetCSharpAnswerHandler(string answerContent, int version = 0)
        {
            if (version == 0)
            {
                var match = Regex.Match(answerContent,
                    @"^(?ix)0;" +
                    @"(?<template>" + EnumOptions<CSharpCodeTemplate>() + @");" +
                    @"(?<normalize>true|false);" +
                    @"(?<validation>" + EnumOptions<CSharpCodeValidation>() + @");" +
                    @"(?<tests>[^;~]+~)+" +
                    @"(;(?<inputs>[^~]+~)+)?" +
                    @"\s*$", RegexOptions.ExplicitCapture);

                // todo: [;~] is problematic

                if (!match.Success)
                    throw new ArgumentException("failed to match");

                var template = (CSharpCodeTemplate)Enum.Parse(typeof(CSharpCodeTemplate), match.Groups["template"].Value, true);
                var validation = (CSharpCodeValidation)Enum.Parse(typeof(CSharpCodeValidation), match.Groups["validation"].Value, true);
                var normalize = bool.Parse(match.Groups["normalize"].Value);
                var tests = GetTildeList(match.Groups["tests"]);
                var inputs = GetTildeList(match.Groups["tests"]);

                return new CSharpAnswerHandler
                {
                    CodeTemplate = template,
                    NormalizeLines = normalize,
                    Tests = tests,
                    Validation = validation,
                    Inputs = inputs
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



        static IEnumerable<string> GetTildeList(Group group)
        {
            return group.Captures.Cast<Capture>().Select(c => c.Value.EndsWith("~") ? c.Value.Substring(0, c.Value.Length - 1) : c.Value);
        }

        static string EnumOptions<TEnum>()
        {
            return String.Join("|", (TEnum[])Enum.GetValues(typeof(TEnum)));
        }

    }
}