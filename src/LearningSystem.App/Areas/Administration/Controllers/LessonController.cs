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
using ValidateAntiForgeryTokenAttribute = TeamAzureDragon.Utils.FakeValidateAntiForgeryTokenAttribute;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using TeamAzureDragon.Utils;
using LearningSystem.App.Areas.Administration.ViewModels;

namespace LearningSystem.App.Areas.Administration.Controllers
{
    public class LessonController : AdminController
    {
        IUoWLearningSystem db;
        public LessonController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        // GET: /Administration/Skill/
        public ActionResult Index()
        {
            //ViewData["skills"] = db.Skills.All().ToList()
            //            .Select(skill => Misc.SerializeToDictionary(skill,
            //                    path =>
            //                    {
            //                        //if (path == "Skill") return RecursiveSerializationOption.Assign;
            //                        if (path == "SkillId") return RecursiveSerializationOption.Assign;
            //                        if (path == "Name") return RecursiveSerializationOption.Assign;
            //                        return RecursiveSerializationOption.Skip;
            //                    })).ToList();

            //ViewData["requerments"] = db.Lessons.All().ToList()
            //            .Select(lesson => Misc.SerializeToDictionary(lesson,
            //                    path =>
            //                    {
            //                        if (path == "LessonId") return RecursiveSerializationOption.Assign;
            //                        if (path == "SkillId") return RecursiveSerializationOption.Assign;
            //                        if (path == "Name") return RecursiveSerializationOption.Assign;
            //                        return RecursiveSerializationOption.Skip;
            //                    })).ToList(); 

            return View(db.Lessons.All("Skill").ToList());
        }

        // GET: /Administration/Skill/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.GetById(id.Value);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        public ActionResult GetSkills([DataSourceRequest]DataSourceRequest request)
        {
            var viewModelSkills = db.Skills.All().ToList()
                    .Select(skill => new SkillViewModel().FillViewModel(skill)).ToList();

            //DataSourceResult result = viewModelExercises.ToDataSourceResult(request);

            return Json(viewModelSkills, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequirements([DataSourceRequest]DataSourceRequest request)
        {
            var viewModelReqirements = db.Lessons.All().ToList()
                        .Select(requerment => new LessonViewModel().FillViewModel(requerment)).ToList();

            //DataSourceResult result = viewModelExercises.ToDataSourceResult(request);

            return Json(viewModelReqirements, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var viewModelLessons = db.Lessons.All("Skill").ToList()
                .Select(lesson => new LessonViewModel().FillViewModel(lesson)).ToList();

            //for (int i = 0; i < viewModelLessons.Count(); i++)
            //{
            //    if (viewModelLessons[i].Description != null)
            //    {
            //        viewModelLessons[i].Description = viewModelLessons[i].Description.Abbreviate(30);
            //    }
            //}

            DataSourceResult result = viewModelLessons.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Administration/Skill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, LessonViewModel lessonVM)
        {
            if (ModelState.IsValid)
            {
                var lesson = lessonVM.FillModel(db.Context);
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { lessonVM }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, LessonViewModel lessonVM)
        {
            if (ModelState.IsValid)
            {
                var model = db.Lessons.GetById(lessonVM.LessonId);
                lessonVM.FillModel(db.Context, model);
                db.Lessons.Update(model);
                return RedirectToAction("Index");
            }
            return View(new[] { lessonVM }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, LessonViewModel lessonVM)
        {
            if (ModelState.IsValid)
            {
                db.Lessons.Delete(lessonVM.LessonId);
            }

            return View(new[] { lessonVM }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
