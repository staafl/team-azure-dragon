using Ionic.Zip;
using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace LearningSystem.App.AppLogic
{
    public static class XmlParser
    {
        public static void ParseQuestions(Dictionary<int, Question> questions, IEnumerable<ZipEntry> archivedQuestions, XmlDocument document)
        {
            foreach (var question in archivedQuestions)
            {
                MemoryStream memoryStream = new MemoryStream();
                question.Extract(memoryStream);

                memoryStream.Position = 0;
                document.Load(memoryStream);
                var q = document.SelectSingleNode("question");


                int id = int.Parse(q["id"].InnerText);
                int points = int.Parse(q["points"].InnerText);
                int order = int.Parse(q["order"].InnerText);
                string statement = q["statement"].InnerXml;
                string answerType = q["answerType"].InnerText;
                string answerContent = q["answerContent"].InnerText;

                questions.Add(id, new Question()
                {
                    QuestionId = id,
                    Points = points,
                    Order = order,
                    Statement = statement,
                    AnswerType = answerType,
                    AnswerContent = answerContent
                });

                memoryStream.Close();
            }
        }

        public static void ParseExercises(Dictionary<int, Exercise> exercises,
            Dictionary<int, Question> questions,
            IEnumerable<ZipEntry> archivedExercises, XmlDocument document)
        {
            foreach (var archivedEx in archivedExercises)
            {
                MemoryStream memoryStream = new MemoryStream();
                archivedEx.Extract(memoryStream);

                memoryStream.Position = 0;
                document.Load(memoryStream);
                var e = document.SelectSingleNode("exercise");




                int id = int.Parse(e["id"].InnerText);
                string name = e["name"].InnerText;
                int order = int.Parse(e["order"].InnerText);
                string description = e["description"].InnerText;

                var excercise = new Exercise()
                {
                    ExerciseId = id,
                    Name = name,
                    Order = order,
                    Description = description,
                    Questions = new HashSet<Question>()
                };


                XmlNodeList QuestionIds = e["questions"].ChildNodes;

                foreach (XmlNode item in QuestionIds)
                {
                    int questionId = int.Parse(item.InnerText);
                    Question q = questions[questionId];
                    excercise.Questions.Add(q);
                }

                exercises.Add(id, excercise);

                memoryStream.Close();
            }
        }

        public static void ParseLessons(Dictionary<int, Lesson> lessons,
            Dictionary<int, Exercise> exercises,
            IEnumerable<ZipEntry> archivedLessons, XmlDocument document)
        {
            Dictionary<int, List<int>> lessonRequirements = new Dictionary<int, List<int>>();

            foreach (var archivedLesson in archivedLessons)
            {
                MemoryStream memoryStream = new MemoryStream();
                archivedLesson.Extract(memoryStream);

                memoryStream.Position = 0;
                document.Load(memoryStream);
                var l = document.SelectSingleNode("lesson");

                int id = int.Parse(l["id"].InnerText);
                string name = l["name"].InnerText;
                string description = l["description"].InnerText;

                var lesson = new Lesson()
                {
                    LessonId = id,
                    Name = name,
                    Description = description,
                    Exercises = new HashSet<Exercise>(),
                    Requirements = new HashSet<Lesson>()
                };


                XmlNodeList exerciseIds = l["exercises"].ChildNodes;

                foreach (XmlNode item in exerciseIds)
                {
                    int exerciseId = int.Parse(item.InnerText);
                    Exercise e = exercises[exerciseId];
                    lesson.Exercises.Add(e);
                }

                XmlNodeList reqLessonsIds = l["requirements"].ChildNodes;

                if (reqLessonsIds != null)
                {

                    lessonRequirements.Add(lesson.LessonId, new List<int>());
                    foreach (XmlNode item in reqLessonsIds)
                    {
                        int lessonId = int.Parse(item.InnerText);
                        lessonRequirements[lesson.LessonId].Add(lessonId);
                    }
                }

                lessons.Add(id, lesson);

                memoryStream.Close();
            }

            foreach (var reqKey in lessonRequirements.Keys)
            {
                var lesson = lessons[reqKey];

                var requirements = lessonRequirements[reqKey];
                lesson.Requirements = new HashSet<Lesson>();
                foreach (var item in requirements)
                {
                    lesson.Requirements.Add(lessons[item]);
                }
            }
        }

        public static void ParseSkill(Skill skill,
           ZipEntry archivedSkill, XmlDocument document)
        {

            MemoryStream memoryStream = new MemoryStream();
            archivedSkill.Extract(memoryStream);

            memoryStream.Position = 0;
            document.Load(memoryStream);
            var l = document.SelectSingleNode("skill");

            string name = l["name"].InnerText;
            string description = l["description"].InnerText;

            skill.Name = name;
            skill.Description = description;


            memoryStream.Close();
        }
    }
}