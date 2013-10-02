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

        public ActionResult Index(int exId)
        {
            var question = Db.Questions.All("Users")
                .Where(q => q.ExerciseId == exId)
                .OrderBy(q => q.Order)
                .Skip(1)
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

            if (question == null)
            {
                //logic
            }

            var handler = AnswerHandlerFactory.GetHandler(question.AnswerType, question.AnswerContent);
            question.InputHtml = handler.RenderInputHtml();

            ViewBag.ExId = exId;
            return View(question);
        }

        public ActionResult GetQuestion(int exId, int toSkip = 0)
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

            if (question == null)
            {
                //logic
            }

            var handler = AnswerHandlerFactory.GetHandler(question.AnswerType, question.AnswerContent);
            question.InputHtml = handler.RenderInputHtml();

            return PartialView("_Question", question);
        }
	}
}