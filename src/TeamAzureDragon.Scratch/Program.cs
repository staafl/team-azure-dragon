﻿using LearningSystem.App.AppLogic;
using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;
using Rossie.Engine;
using System.Reflection;
using System.Data.Entity;
using TeamAzureDragon.Utils.Attributes;

namespace TeamAzureDragon.Scratch
{

    public class LessonViewModel : IViewModel<Lesson>
    {
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ModelNavigationId("Skill")]
        [ModelPropertyPath("Skill.SkillId")]
        public int SkillId { get; set; }

        [ModelPropertyPath("Skill.Name")]
        public string SkillName { get; set; }

        [ModelNavigationId("Requirements")]
        [ModelPropertyPath("Requirements.LessonId")]
        public ICollection<int> RequirementsId { get; set; }

        [ModelPropertyPath("Requirements.Name")]
        public ICollection<string> RequirementsName { get; set; }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            //var obj = new { A = "a", B = "b", C = new { A = "a" } };

            //var dict = Misc.SerializeToDictionary(obj);

            //var executer = new Rossie.Engine.CodeExecuter();
            //Console.WriteLine(executer.Execute("1 + 2", CSharpCodeTemplate.Expression));

            // var handler = AnswerHandlerFactory.GetHandler(AnswerType.CSharpCode, @"0;Expression;true;3~3~", 0);

            // Console.WriteLine(handler.ValidateInput("1 + 2"));

            using (var context = new LearningSystem.Data.LearningSystemContext())
            {
                Console.WriteLine(context.Lessons.Count());
            }
            using (var context = new LearningSystem.Data.LearningSystemContext())
            {
                var lesson = context.Lessons.Find(2);
                var lvm = new LessonViewModel().FillViewModel(lesson);
                var lesson2 = lvm.CreateModel(context);
            }
        }


    }
}
