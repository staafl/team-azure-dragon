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
using ValidateAntiForgeryTokenAttribute = TeamAzureDragon.Utils.ValidateAntiForgeryTokenAttribute;

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
            ViewData["skills"] = db.Skills.All().ToList()
                        .Select(skill => Misc.SerializeToDictionary(skill,
                                path =>
                                {
                                    if (path == "SkillId") return RecursiveSerializationOption.Assign;
                                    if (path == "Name") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

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

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var viewModelLessons = db.Lessons.All("Skill").ToList()
                        .Select(lesson => Misc.SerializeToDictionary(lesson,
                                path =>
                                {
                                    if (path == "LessonId") return RecursiveSerializationOption.Assign;
                                    if (path == "Name") return RecursiveSerializationOption.Assign;
                                    if (path == "Description") return RecursiveSerializationOption.Assign;
                                    if (path == "Skill") return RecursiveSerializationOption.Recurse;
                                    if (path == "Skill.Name") return RecursiveSerializationOption.Assign;
                                    if (path == "SkillId") return RecursiveSerializationOption.Assign;
                                    //if (path == "Requirements") return RecursiveSerializationOption.ForeachRecurse;
                                    //if (path == "Requirements.LessonId") return RecursiveSerializationOption.Assign;
                                    //if (path == "Requirements.Name") return RecursiveSerializationOption.Assign;
                                    return RecursiveSerializationOption.Skip;
                                })).ToList();

            for (int i = 0; i < viewModelLessons.Count(); i++)
            {
                viewModelLessons[i]["Description"] = viewModelLessons[i]["Description"].ToString().Abbreviate(30);
            }

            DataSourceResult result = viewModelLessons.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Administration/Skill/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { lesson }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, Lesson lesson)
        {
            Lesson oldlesson = db.Lessons.GetById(lesson.LessonId);
            if (ModelState.IsValid)
            {
                oldlesson.Name = lesson.Name;
                oldlesson.Description = lesson.Description;
                oldlesson.SkillId = lesson.Skill.SkillId;
                db.Lessons.Update(oldlesson);
                return RedirectToAction("Index");
            }
            return View(new[] { oldlesson }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Lessons.Delete(lesson);
            }

            return View(new[] { lesson }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
