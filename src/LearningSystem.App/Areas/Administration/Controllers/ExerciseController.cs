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
using LearningSystem.App.Areas.Administration.Views.ViewModels;

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

            //ViewData["lessons"] = db.Lessons.All().Select(l => new LessonVM()
            //{
            //    LessonId = l.LessonId,
            //    Name = l.Name
            //})
            //.OrderBy(l => l.LessonId);


            //var model = db.Exercises.All("Lesson").Select(e => new ExerciseVM()
            //{
            //    Name = e.Name,
            //    Description = e.Description,
            //    Order = e.Order,
            //    Lesson = new LessonVM()
            //    {
            //        LessonId = e.LessonId,
            //        Name = e.Name
            //    },
            //    LessonName = e.Lesson.Name
            //});

            //return View(model.ToList());
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
            var viewModelExercises = db.Exercises.All("Lesson").ToList()
                        .Select(exercise => Misc.SerializeToDictionary(exercise,
                                path =>
                                {
                                    if (path == "ExerciseId") return RecursiveSerializationOption.Assign;
                                    if (path == "Name") return RecursiveSerializationOption.Assign;
                                    if (path == "Description") return RecursiveSerializationOption.Assign;
                                    if (path == "Order") return RecursiveSerializationOption.Assign;
                                    if (path == "Lesson") return RecursiveSerializationOption.Recurse;
                                    if (path == "Lesson.Name") return RecursiveSerializationOption.Assign;
                                    if (path == "Lesson.LessonId") return RecursiveSerializationOption.Assign;
                                    if (path == "LessonId") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

            for (int i = 0; i < viewModelExercises.Count(); i++)
            {
                viewModelExercises[i]["Description"] = viewModelExercises[i]["Description"].ToString().Abbreviate(30);
            }

            //var viewModelExercises = db.Exercises.All("Lesson").ToList().Select(e => new ExerciseVM()
            //{
            //    Name = e.Name,
            //    Description = e.Description.Abbreviate(30),
            //    Order = e.Order,
            //    Lesson = new LessonVM()
            //    {
            //        LessonId = e.LessonId,
            //        Name = e.Name
            //    },
            //    LessonName = e.Lesson.Name
            //});

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
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                Lesson lesson = db.Lessons.GetById(exercise.Lesson.LessonId);
                exercise.Lesson.SkillId = lesson.SkillId;
                db.Exercises.Add(exercise);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { exercise }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, Exercise exercise)
        {
            Exercise oldexercise = db.Exercises.GetById(exercise.ExerciseId);
            if (ModelState.IsValid)
            {
                oldexercise.Name = exercise.Name;
                oldexercise.Description = exercise.Description;
                oldexercise.Order = exercise.Order;
                oldexercise.LessonId = exercise.Lesson.LessonId;

                db.Exercises.Update(oldexercise);
                return RedirectToAction("Index");
            }
            return View(new[] { oldexercise }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                db.Exercises.Delete(exercise);
            }

            return View(new[] { exercise }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
