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
            ViewData["exercises"] = db.Exercises.All().ToList()
                        .Select(exerciese => Misc.SerializeToDictionary(exerciese,
                                path =>
                                {
                                    if (path == "ExerciseId") return RecursiveSerializationOption.Assign;
                                    if (path == "Name") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

            return View(db.Questions.All("Exercise").ToList());
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
            var viewModelQuestion = db.Questions.All("Exercise").ToList()
                        .Select(question => Misc.SerializeToDictionary(question,
                                path =>
                                {
                                    if (path == "QuestionId") return RecursiveSerializationOption.Assign;
                                    if (path == "Statement") return RecursiveSerializationOption.Assign;
                                    if (path == "Oreder") return RecursiveSerializationOption.Assign;
                                    if (path == "Points") return RecursiveSerializationOption.Assign;
                                    if (path == "AnswerType") return RecursiveSerializationOption.Assign;
                                    if (path == "AnswerContent") return RecursiveSerializationOption.Assign;
                                    if (path == "AnswerContentVersion") return RecursiveSerializationOption.Assign;
                                    if (path == "Exercise") return RecursiveSerializationOption.Recurse;
                                    if (path == "Exercise.ExerciseId") return RecursiveSerializationOption.Assign;
                                    if (path == "Exercise.Name") return RecursiveSerializationOption.Assign;
                                    if (path == "ExerciseId") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

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
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { question }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Update(question);
                return RedirectToAction("Index");
            }
            return View(new[] { question }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Delete(question);
            }

            return View(new[] { question }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
