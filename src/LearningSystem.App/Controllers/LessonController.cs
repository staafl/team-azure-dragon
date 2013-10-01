using LearningSystem.App.ViewModels;
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
            List<ExerciseViewModel> exercises = new List<ExerciseViewModel>();
            var dbEx = db.Exercises.All("Users").Where(ex => ex.LessonId == lessonId).OrderBy(ex => ex.Order);

            bool isAvailable = true;
            foreach (var excercise in dbEx)
            {
                var exerciseVM = new ExerciseViewModel
                {
                    Name = excercise.Name,
                    Description = excercise.Description,
                    ExerciseId = excercise.LessonId,
                    IsCompleted = excercise.Users.Any(u => u.UserName == User.Identity.Name) ? true : false,
                    IsAvailable = isAvailable
                };

                if (exerciseVM.IsCompleted == false)
                {
                    isAvailable = false;
                }

                exercises.Add(exerciseVM);
            }

            return View(exercises);
        }
	}
}