using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LearningSystem.Models;
using LearningSystem.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using TeamAzureDragon.Utils;
using ValidateAntiForgeryTokenAttribute = TeamAzureDragon.Utils.FakeValidateAntiForgeryTokenAttribute;
using LearningSystem.App.Areas.Administration.ViewModels;

namespace LearningSystem.App.Areas.Administration.Controllers
{
    public class QuestionController : AdminController
    {
        IUoWLearningSystem db;
        public QuestionController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        // GET: /Administration/Skill/
        public ActionResult Index()
        {
            return View(db.Questions.All("Exercise").ToList());
        }

        public ActionResult GetExercises([DataSourceRequest]DataSourceRequest request)
        {
            var viewModelExercises = db.Exercises.All().ToList()
                        .Select(exerciese => Misc.SerializeToDictionary(exerciese,
                                path =>
                                {
                                    if (path == "ExerciseId") return RecursiveSerializationOption.Assign;
                                    if (path == "Name") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

            //DataSourceResult result = viewModelExercises.ToDataSourceResult(request);

            return Json(viewModelExercises, JsonRequestBehavior.AllowGet);
        }

        // GET: /Administration/Skill/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.GetById(id.Value);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            //var viewModelQuestion = db.Questions.All("Exercise").ToList()
            //            .Select(question => Misc.SerializeToDictionary(question,
            //                    path =>
            //                    {
            //                        if (path == "QuestionId") return RecursiveSerializationOption.Assign;
            //                        if (path == "Statement") return RecursiveSerializationOption.Assign;
            //                        if (path == "Order") return RecursiveSerializationOption.Assign;
            //                        if (path == "Points") return RecursiveSerializationOption.Assign;
            //                        if (path == "AnswerType") return RecursiveSerializationOption.Assign;
            //                        if (path == "AnswerContent") return RecursiveSerializationOption.Assign;
            //                        if (path == "AnswerContentVersion") return RecursiveSerializationOption.Assign;
            //                        if (path == "Exercise") return RecursiveSerializationOption.Recurse;
            //                        if (path == "Exercise.ExerciseId") return RecursiveSerializationOption.Assign;
            //                        if (path == "Exercise.Name") return RecursiveSerializationOption.Assign;
            //                        if (path == "ExerciseId") return RecursiveSerializationOption.Assign;
            //                        return RecursiveSerializationOption.Skip;
            //                    })).ToList();

            var viewModelQuestion = db.Questions.All("Exercise").ToList()
                        .Select(question => new QuestionViewModel().FillViewModel(question)).ToList();

            DataSourceResult result = viewModelQuestion.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Administration/Skill/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, QuestionViewModel questionVM)
        {
            Exercise exercise = db.Exercises.GetById(questionVM.Exercise.ExerciseId);
            questionVM.Exercise.LessonId = exercise.LessonId;

            if (ModelState.IsValid)
            {                
                var question = questionVM.FillModel(db.Context);
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { questionVM }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, QuestionViewModel questionVM)
        {
            
            if (ModelState.IsValid)
            {
                Question oldquestion = db.Questions.GetById(questionVM.QuestionId);
                //oldquestion.Statement = questionVM.Statement;
                //oldquestion.Order = questionVM.Order;
                //oldquestion.Points = questionVM.Points;
                //oldquestion.AnswerType = questionVM.AnswerType;
                //oldquestion.AnswerContent = questionVM.AnswerContent;
                //oldquestion.AnswerContentVersion = questionVM.AnswerContentVersion;
                //oldquestion.ExerciseId = questionVM.Exercise.ExerciseId;
                oldquestion = questionVM.FillModel(db.Context, oldquestion);
                db.Questions.Update(oldquestion);
                return RedirectToAction("Index");
            }
            return View(new[] { questionVM }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, QuestionViewModel questionVM)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Delete(questionVM.QuestionId);
            }

            return View(new[] { questionVM }.ToDataSourceResult(request, ModelState));
        }

        public static List<SelectListItem> EnumToDropDownList(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<int>()
                .Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = Enum.GetName(enumType, i),
                }).ToList();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
