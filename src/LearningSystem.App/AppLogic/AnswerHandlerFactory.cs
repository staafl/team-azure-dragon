using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using TeamAzureDragon.Utils;

namespace LearningSystem.App.AppLogic
{
    public static class AnswerHandlerFactory
    {
        static public IAnswerHandler GetHandler(AnswerType type, string answerContent)
        {
            // TODO: arg validation
            switch (type)
            {
                case AnswerType.None:
                    throw new ArgumentException("'type' cannot be None.");
                case AnswerType.Custom:
                case AnswerType.CSharpCode:
                    return GetSCharpAnswerHandler(answerContent);
                case AnswerType.Text:
                    return GetTextAnswerHandler(answerContent);
                case AnswerType.ApproximateText:
                case AnswerType.MultipleChoise:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unsupported type: " + type);
            }
        }

        static IAnswerHandler GetSCharpAnswerHandler(string answerContent)
        {
            return new CSharpAnswerHandler();
        }


        static IAnswerHandler GetTextAnswerHandler(string data)
        {
            return Misc.ParseVersioned<TextAnswerHandler>(data);
        }

    }
}