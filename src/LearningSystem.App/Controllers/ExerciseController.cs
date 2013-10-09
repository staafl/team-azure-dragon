using LearningSystem.App.AppLogic;
using LearningSystem.App.ViewModels;
using LearningSystem.Data;
using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeamAzureDragon.Utils;

namespace LearningSystem.App.Controllers
{
    public class ExerciseController : LearningSystemControllerBase
    {
        public ExerciseController(IUoWLearningSystem db) : base(db) { }

        private int CountQuestions(int exId)
        {
            int count = Db.Exercises.GetById(exId).Questions.Count;
            return count;
        }

        private QuestionViewModel GetCurrentQuestion(int exId, int toSkip)
        {
            var question = Db.Questions.All("Users")
                .Where(q => q.ExerciseId == exId)
                .OrderBy(q => q.Order)
                .Skip(toSkip)
                .Take(1)
                .Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    Statement = q.Statement,
                    AnswerType = q.AnswerType,
                    AnswerContent = q.AnswerContent,
                    ExerciseId = q.ExerciseId
                })
                .FirstOrDefault();

            return question;
        }

        public ActionResult Index(int exId)
        {
            var question = GetCurrentQuestion(exId, 0);

            if (question == null)
            {
                return View("_MissingQuestions");
            }

            var handler = AnswerHandlerFactory.GetHandler(question.AnswerType, question.AnswerContent);
            question.InputHtml = handler.RenderInputHtml();

            ViewBag.LessonId = Db.Exercises.GetById(exId).LessonId;
            ViewBag.Lesson = Db.Exercises.GetById(exId);
            ViewBag.CurrentQuestionOrder = 0;
            ViewBag.ExId = exId;
            var questionsCount = this.CountQuestions(exId);
            ViewBag.QuestionsCount = questionsCount;

            var passableErrors = questionsCount / 4;
            if (passableErrors == 0)
	        {
		        passableErrors = 1;
	        }
            ViewBag.PassableErrors = passableErrors;
            return View(question);
        }

        public ActionResult GetQuestion(int exId, int toSkip)
        {
            var question = GetCurrentQuestion(exId, toSkip);

            if (question == null)
            {
                return HttpNotFound();
            }

            var handler = AnswerHandlerFactory.GetHandler(question.AnswerType, question.AnswerContent);
            question.InputHtml = handler.RenderInputHtml();

            return PartialView("_Question", question);
        }

        public ActionResult FinishExercise(int exId)
        {
            //var exercise = Db.Exercises.GetById(exId);

            //var user = Db.Users.All().SingleOrDefault(u => u.UserName == User.Identity.Name);
            //exercise.Users.Add(user);
            //Db.SaveChanges();

            ViewBag.ExId = exId;

            return PartialView("_FinishExercise");
        }

        public ActionResult FailedExercise(int exId)
        {
            ViewBag.ExId = exId;

            return PartialView("_FailedExercise");
        }

        public ActionResult BackToLesson(int exId)
        {
            var exercise = Db.Exercises.GetById(exId);
            var lessonId = exercise.LessonId;
            //TODO check if lesson is finished in the redirected action
            return RedirectToAction("Index", "Lesson", new { lessonId = lessonId });
        }

        [System.Web.Mvc.HttpPostAttribute]
        public ActionResult HandleQuestionInput(int questionId, string input, bool passExercise)
        {
            var question = Db.Questions.GetById(questionId);
            if (question == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var handler = AnswerHandlerFactory.GetHandler(question.AnswerType, question.AnswerContent);

            var result = handler.ValidateInput(input);

            var dict = Misc.SerializeToDictionary(result);

            /* {
             *  success (bool),
             *  errorContent (string),
             *  exerciseFinished (bool)
             *  lessonFinished (bool),
             *  skillFinished (bool)
             *  }
             */

            if (result.Success || passExercise)
            {
                var exercise = question.Exercise;
                if (question.Order == exercise.Questions.Max(q => q.Order))
                {
                    var user = this.GetCurrentUser();

                    dict["exerciseFinished"] = true;
                    user.Exercises.Add(exercise);

                    var lesson = exercise.Lesson;
                    if (exercise.Order == lesson.Exercises.Max(e => e.Order))
                    {
                        dict["lessonFinished"] = true;
                        user.Lessons.Add(lesson);
                        if (!lesson.Skill.Lessons.Select(l => l.LessonId).Except(user.Lessons.Select(l => l.LessonId)).Any())
                        {
                            dict["skillFinished"] = true;
                        }
                    }

                    Db.SaveChanges();
                }
            }

            return new JsonResult { Data = dict };
        }
    }
}