using LearningSystem.App.AppLogic;
using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;
using Rossie.Engine;

namespace TeamAzureDragon.Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            //var obj = new { A = "a", B = "b", C = new { A = "a" } };

            //var dict = Misc.SerializeToDictionary(obj);

            //var executer = new Rossie.Engine.CodeExecuter();
            //Console.WriteLine(executer.Execute("1 + 2", CSharpCodeTemplate.Expression));

            var handler = AnswerHandlerFactory.GetHandler(AnswerType.CSharpCode, @"0;Expression;true;3~3~", 0);

            Console.WriteLine(handler.ValidateInput("1 + 2"));
        }


    }
}
