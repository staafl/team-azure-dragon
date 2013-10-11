using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LearningSystem.AnswerHandlers.Reference;
using TeamAzureDragon.Utils;

namespace LearningSystem.App.AppLogic
{
    public static class AnswerHandlerFactory
    {

        static readonly Dictionary<string, Func<string, IAnswerHandler>> handlerGetters = new Dictionary<string, Func<string, IAnswerHandler>>();


        public static void LoadPlugin(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => typeof(IAnswerHandler).IsAssignableFrom(t) && !t.IsAbstract))
            {
                var handlerGetter = type.GetMethod("GetAnswerHandler");
                var identifier = type.GetField("TypeIdentifier");
                var del = handlerGetter.CreateDelegate(typeof(Func<string, IAnswerHandler>));

                RegisterHandler(identifier.GetValue(null) + "", (Func<string, IAnswerHandler>)del);
            }
        }
        public static void RegisterHandler(string identifier, Func<string, IAnswerHandler> handlerGetter)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (handlerGetter == null) throw new ArgumentNullException("handlerGetter");
            handlerGetters[identifier] = handlerGetter;
        }

        static public IAnswerHandler GetHandler(string type, string answerContent, bool throwIfFailed = true)
        {
            Func<string, IAnswerHandler> handlerGetter;
            if (!handlerGetters.TryGetValue(type, out handlerGetter))
            {
                if (throwIfFailed)
                    throw new ArgumentException("Type {0} not recognized".sprintf(type));
                return null;
            }

            return handlerGetter(answerContent);
        }



    }
}