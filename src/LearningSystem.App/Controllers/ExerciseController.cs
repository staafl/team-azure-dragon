using LearningSystem.App.AppLogic;
using LearningSystem.Data;
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

        //
        // GET: /Exercise/
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPostAttribute]
        public Dictionary<string, object> HandleQuestionInput(int questionId, string input)
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
             *  }
             */

            if (result.Success)
            {
                var exercise = question.Exercise;
                if (question.Order == exercise.Questions.Max(q => q.Order))
                {
                    dict["exerciseFinished"] = true;
                }
                var user = this.GetCurrentUser();
                user.Exercises.Add(exercise);
            }
            return dict;
        }
    }
}