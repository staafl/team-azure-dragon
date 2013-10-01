using LearningSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearningSystem.App.Controllers
{
    public class LessonController : Controller
    {
        IUoWLearningSystem db;
        public LessonController(IUoWLearningSystem db)
        {
            this.db = db;
        }
        //
        // GET: /Lesson/
        public ActionResult Index(int lessonId)
        {
            return View();
        }
	}
}