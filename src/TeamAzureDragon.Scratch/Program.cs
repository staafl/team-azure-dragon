using LearningSystem.App.AppLogic;
using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;
using TeamAzureDragon.CSharpCompiler;
using System.Reflection;
using System.Data.Entity;
using TeamAzureDragon.Utils.Attributes;

namespace TeamAzureDragon.Scratch
{

    class Program
    {

        static void Main(string[] args)
        {
            //var obj = new { A = "a", B = "b", C = new { A = "a" } };

            //var dict = Misc.SerializeToDictionary(obj);


            CSharpCode();

            //using (var context = new LearningSystem.Data.LearningSystemContext())
            //{
            //    Console.WriteLine(context.Lessons.Count());
            //}
            //using (var context = new LearningSystem.Data.LearningSystemContext())
            //{
            //    var lesson = context.Lessons.Find(2);
            //    var lvm = new LessonViewModel().FillViewModel1(lesson);
            //    var lesson2 = lvm.CreateModel(context);
            //}
        }

        static void CSharpCode()
        {
            string program;
            while ((program = Console.ReadLine()) != null)
            {
                string stdout;
                bool success;
                Console.WriteLine(">> " + ExecutionDirector.RunAndReport(program, out success, null, out stdout, CSharpCodeTemplate.MethodBody));
                if (!string.IsNullOrEmpty(stdout))
                    Console.WriteLine("STDOUT: " + stdout);
            }


            //var handler = AnswerHandlerFactory.GetHandler(AnswerType.CSharpCode, @"0;MethodBody;true;data => data is Array && ((Array)data).Length == 5~", 0);
            //Console.WriteLine(handler.ValidateInput("return 1 + 2;").ErrorContent);
            //Console.WriteLine(handler.ValidateInput("return 1024;").Success);
            //Console.WriteLine(handler.ValidateInput("asdfasdf").ErrorContent);
            //var handler = AnswerHandlerFactory.GetHandler(AnswerType.CSharpCode, @"0;Expression;true;3~3~", 0);

            // Console.WriteLine(handler.ValidateInput("1 + 2"));

            //var template = CSharpCodeTemplate.Class;
            //CSharpAnswerHandler handler = AnswerHandlerFactory.GetHandler(AnswerType.CSharpCode, @"0;Expression;true;3~3~", 0);

            //Console.WriteLine(handler.ValidateInput("1 + 2"));
        }

    }
}
