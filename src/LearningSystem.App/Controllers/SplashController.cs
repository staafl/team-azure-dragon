using LearningSystem.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LearningSystem.App.Controllers
{
    public class SplashController : Controller
    {
        IUoWLearningSystem db;
        public SplashController(IUoWLearningSystem db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "MyAchievements");
            }

            return View();
        }
	}
}