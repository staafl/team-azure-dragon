using System;
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

namespace LearningSystem.App.Areas.Administration.Controllers
{
    public class SkillController : Controller
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
            var viewModelSkills = db.Skills.All().ToList();

            //foreach (var item in viewModelSkills)
            //{
            //    if (item.Description != null && item.Description.Length > 20)
            //    {
            //        item.Description = item.Description.Substring(0, 20);
            //    }
            //}

            DataSourceResult result = viewModelSkills.ToDataSourceResult(request,
                s => new { Skill = Ids.SkillId });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Administration/Skill/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, Skill skill)
        {
            if (ModelState.IsValid)
            {
                db.Skills.Add(skill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(new[] { skill }.ToDataSourceResult(request, ModelState));
        }

        // POST: /Administration/Skill/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([DataSourceRequest]DataSourceRequest request, Skill skill)
        {
            if (ModelState.IsValid)
            {
                db.Skills.Update(skill);
                return RedirectToAction("Index");
            }
            return View(new[] { skill }.ToDataSourceResult(request, ModelState));
        }

        // GET: /Administration/Skill/Delete/5
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, Skill skill)
        {
            if (ModelState.IsValid)
            {
                db.Skills.Delete(skill);
            }

            return View(new[] { skill }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
