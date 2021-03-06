﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LearningSystem.Models;
using TeamAzureDragon.Utils;
using LearningSystem.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ValidateAntiForgeryTokenAttribute = TeamAzureDragon.Utils.FakeValidateAntiForgeryTokenAttribute;
using LearningSystem.App.Areas.Administration.ViewModels;

namespace LearningSystem.App.Areas.Administration.Controllers
{
    public class SkillController : AdminController
    {
        IUoWLearningSystem db;
        public SkillController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        // GET: /Administration/Skill/
        public ActionResult Index()
        {
            return View(db.Skills.All().ToList());
        }

        // GET: /Administration/Skill/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.GetById(id.Value);
            if (skill == null)
            {
                return HttpNotFound();
            }
            return View(skill);
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            // Skill
            //      SkillId
            //      Name            - Name
            //      Description     - Description
            //      Lessons
            //          LessonId
            //          Name        - LessonName
            //          Description
            //          
            //var viewModelSkills = db.Skills.All().ToList()
            //            .Select(skill => Misc.SerializeToDictionary(skill,
            //                    path =>
            //                    {
            //                        if (path == "SkillId") return RecursiveSerializationOption.Assign;
            //                        if (path == "Name") return RecursiveSerializationOption.Assign;
            //                        if (path == "Description") return RecursiveSerializationOption.Assign;
            //                        //if (path == "Lessons") return RecursiveSerializationOption.ForeachRecurse;
            //                        //if (path == "Lessons.Name") return RecursiveSerializationOption.Assign;
            //                        return RecursiveSerializationOption.Skip;
            //                    })).ToList();

            var viewModelSkills = db.Skills.All().ToList()
                        .Select(skill => new SkillViewModel().FillViewModel(skill)).ToList();

            DataSourceResult result = viewModelSkills.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Administration/Skill/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, SkillViewModel skillVM)
        {
            if (ModelState.IsValid)
            {
                var skill = skillVM.FillModel(db.Context);
                db.Skills.Add(skill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { skillVM }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, SkillViewModel skillVM)
        {
            if (ModelState.IsValid)
            {
                var skill = db.Skills.GetById(skillVM.SkillId);
                skillVM.FillModel(db.Context, skill);
                db.Skills.Update(skill);
                return RedirectToAction("Index");
            }
            return View(new[] { skillVM }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, SkillViewModel skillVM)
        {
            if (ModelState.IsValid)
            {
                db.Skills.Delete(skillVM.SkillId);
            }

            return View(new[] { skillVM }.ToDataSourceResult(request, ModelState));
        }

        //public ActionResult Lessons([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    var viewModelLessons = db.Skills.All().ToList()
        //                .Select(skill => Misc.SerializeToDictionary(skill,
        //                    path =>
        //                        {
        //                            if (path == "SkillId") return RecursiveSerializationOption.Assign;
        //                            if (path == "Lessons") return RecursiveSerializationOption.Assign;
        //                            return RecursiveSerializationOption.Skip;
        //                        }));

        //    DataSourceResult result = viewModelLessons.ToDataSourceResult(request);

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
