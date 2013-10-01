using LearningSystem.App.ViewModels;
using LearningSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LearningSystem.App.Controllers
{
    public class MyAchievementsController : Controller
    {
        IUoWLearningSystem db;
        public MyAchievementsController(IUoWLearningSystem db)
        {
            this.db = db;
        }
        //
        // GET: /MyAchievements/
        public ActionResult Index()
        {
            //get skills of the logged user
            //TODO: fix n+1
            var currentSkills = db.Skills.All("Users").Where(s => s.Users.Any(u => u.UserName == User.Identity.Name))
                .Select(s => new SkillViewModel
                {
                    SkillId = s.SkillId,
                    SkillName = s.Name,
                    SkillDescription = s.Description
                });

            return View(currentSkills);
        }
	}
}