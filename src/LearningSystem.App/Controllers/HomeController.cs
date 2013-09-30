using LearningSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using Ionic.Zip;
using System.Text;
using TeamAzureDragon.Utils;
using LearningSystem.Models;
using System.Xml;
using LearningSystem.App.AppLogic;

namespace LearningSystem.App.Controllers
{
    public class HomeController : Controller
    {
        IUoWLearningSystem db;
        public HomeController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveSkill()
        {
            Response.Expires = -1;
            HttpPostedFileBase file = Request.Files["skills-uploader"];

            ZipFile zipFile = ZipFile.Read(file.InputStream);
            StringBuilder zipContent = new StringBuilder();


            Dictionary<int, Lesson> lessons = new Dictionary<int, Lesson>();

            Dictionary<int, Exercise> exercises = new Dictionary<int, Exercise>();

            Dictionary<int, Question> questions = new Dictionary<int, Question>();

            var archivedQuestions = zipFile.Entries.Where(x =>
                x.Attributes == FileAttributes.Archive &&
                x.FileName.Contains("questions/"));

            XmlDocument document = new XmlDocument();

            XmlParser.ParseQuestions(questions, archivedQuestions, document);

            var archivedExercises = zipFile.Entries.Where(x =>
                x.Attributes == FileAttributes.Archive &&
                x.FileName.Contains("exercises/"));

            XmlParser.ParseExercises(exercises, questions, archivedExercises, document);

            var archivedLessons = zipFile.Entries.Where(x =>
                x.Attributes == FileAttributes.Archive &&
                x.FileName.Contains("lessons/"));

            XmlParser.ParseLessons(lessons, exercises, archivedLessons, document);

            var archivedSkill = zipFile.Entries.Single(x =>
               x.Attributes == FileAttributes.Archive &&
               x.FileName.Contains("skill"));

            Skill skill = new Skill();

            XmlParser.ParseSkill(skill, archivedSkill, document);

            skill.Lessons = lessons.Values;

            db.Skills.Add(skill);
            db.SaveChanges();

            return Content("");
        }


        


    }
}