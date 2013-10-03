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

        [Authorize]
        [HttpPost]
        public ActionResult Search(SearchViewModel text)
        {
            var skills = db.Skills.All().Where(x => x.Name.ToLower().Contains(text.Query.ToLower()));

            var user = db.Users.All().Single(x => x.UserName == User.Identity.Name);



            var toCollection = skills.Select(x => new SkillViewModel
            {
                SkillId = x.SkillId,
                SkillDescription = x.Description,
                SkillName = x.Name
            }).ToList();

            foreach (var item in toCollection)
            {
                item.OwnedByUser = user.Skills.Any(x => x.SkillId == item.SkillId);
            }

            return PartialView("_SearchResults", toCollection);
        }


    }
}