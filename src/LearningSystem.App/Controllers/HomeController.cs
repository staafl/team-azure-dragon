using LearningSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using Ionic.Zip;
using System.Text;
using TeamAzureDragon.Utils;
using LearningSystem.Models;
using System.Xml;
using LearningSystem.App.AppLogic;
using System.Reflection;
using LearningSystem.App.ViewModels;

namespace LearningSystem.App.Controllers
{
    public class HomeController : Controller
    {
        IUoWLearningSystem db;
        public HomeController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {

            var assembly = Assembly.GetExecutingAssembly();

            var nameTokens = assembly.FullName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var name = string.Join(",", nameTokens[0], nameTokens[1]);

            ViewBag.Name = name;

            return View();
        }


        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string searchBox, int toSkip = 0)
        {
            var pageSize = 10;

            var user = db.Users.All("Skills").SingleOrDefault(x => x.UserName == User.Identity.Name);
            var skills = db.Skills.All("Users");

            if (searchBox != string.Empty)
            {
                skills = skills.Where(x => x.Name.ToLower().Contains(searchBox.ToLower()));
            }

            if (user != null)
            {
                skills = skills
                    .OrderByDescending(s => s.Users.Any(u => u.Id == user.Id))
                    .ThenBy(s => s.Name)
                    .Skip(toSkip * pageSize)
                    .Take(pageSize);
            }
            else
            {
                skills = skills
                    .OrderBy(s => s.Name)
                    .Skip(toSkip * pageSize)
                    .Take(pageSize);
            }

            var toCollection = skills.Select(x => new SkillViewModel
            {
                SkillId = x.SkillId,
                SkillDescription = x.Description,
                SkillName = x.Name
            }).ToList();

            if (user != null)
            {
                foreach (var item in toCollection)
                {
                    item.OwnedByUser = user.Skills.Any(x => x.SkillId == item.SkillId);
                }
            }

            return PartialView("_SearchResults", toCollection);
        }

        public ActionResult AutoComplete(string text)
        {
            var skills = db.Skills.All().Where(s => s.Name.StartsWith(text))
                .Take(10)
                .Select(s => new AutoCompleteViewModel
                {
                    SkillName = s.Name
                });

            return Json(skills, JsonRequestBehavior.AllowGet);
        }
    }
}