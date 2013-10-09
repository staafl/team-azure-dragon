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
    public class ExerciseController : AdminController
    {
        IUoWLearningSystem db;
        public ExerciseController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        // GET: /Administration/Skill/
        public ActionResult Index()
        {
            ViewData["lessons"] = db.Lessons.All().ToList()
                        .Select(lesson => Misc.SerializeToDictionary(lesson,
                                path =>
                                {
                                    if (path == "Lesson") return RecursiveSerializationOption.Assign;
                                    if (path == "LessonId") return RecursiveSerializationOption.Assign;
                                    if (path == "Name") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

            return View(db.Exercises.All("Lesson").ToList());            
        }

        // GET: /Administration/Skill/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = db.Exercises.GetById(id.Value);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            //var viewModelExercises = db.Exercises.All("Lesson").ToList()
            //            .Select(exercise => Misc.SerializeToDictionary(exercise,
            //                    path =>
            //                    {
            //                        if (path == "ExerciseId") return RecursiveSerializationOption.Assign;
            //                        if (path == "Name") return RecursiveSerializationOption.Assign;
            //                        if (path == "Description") return RecursiveSerializationOption.Assign;
            //                        if (path == "Order") return RecursiveSerializationOption.Assign;
            //                        if (path == "Lesson") return RecursiveSerializationOption.Recurse;
            //                        if (path == "Lesson.Name") return RecursiveSerializationOption.Assign;
            //                        if (path == "Lesson.LessonId") return RecursiveSerializationOption.Assign;
            //                        if (path == "LessonId") return RecursiveSerializationOption.Assign;
            //                        return RecursiveSerializationOption.Skip;
            //                    })).ToList();

            //for (int i = 0; i < viewModelExercises.Count(); i++)
            //{
            //    viewModelExercises[i]["Description"] = (viewModelExercises[i]["Description"] ?? "").ToString().Abbreviate(30);
            //}

            var viewModelExercises = db.Exercises.All("Lesson").ToList()
                        .Select(exercise => new ExerciseViewModel().FillViewModel(exercise)).ToList();

            DataSourceResult result = viewModelExercises.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Administration/Skill/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, ExerciseViewModel exerciseVM)
        {
            Lesson lesson = db.Lessons.GetById(exerciseVM.Lesson.LessonId);
            exerciseVM.Lesson.SkillId = lesson.SkillId;
            if (ModelState.IsValid)
            {                
                Exercise exercise = exerciseVM.FillModel(db.Context);
                db.Exercises.Add(exercise);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { exerciseVM }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, ExerciseViewModel exerciseVM)
        {
            
            if (ModelState.IsValid)
            {
                Exercise oldexercise = db.Exercises.GetById(exerciseVM.ExerciseId);
                //oldexercise.Name = exerciseVM.Name;
                //oldexercise.Description = exerciseVM.Description;
                //oldexercise.Order = exerciseVM.Order;
                //oldexercise.LessonId = exerciseVM.Lesson.LessonId;
                exerciseVM.FillModel(db.Context, oldexercise);

                db.Exercises.Update(oldexercise);
                return RedirectToAction("Index");
            }
            return View(new[] { exerciseVM }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, ExerciseViewModel exerciseVM)
        {
            if (ModelState.IsValid)
            {
                db.Exercises.Delete(exerciseVM.ExerciseId);
            }

            return View(new[] { exerciseVM }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
