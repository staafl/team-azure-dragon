using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LearningSystem.Models;
using LearningSystem.Data;
using LearningSystem.App.ViewModels;
using LearningSystem.App.AppLogic;

namespace LearningSystem.App.Controllers
{
    [Authorize]
    public class LearningSystemControllerBase : Controller
    {
        private IUoWLearningSystem db;

        protected IUoWLearningSystem Db
        {
            get { return db; }
            set { db = value; }
        }

        public ApplicationUser GetCurrentUser()
        {
            var name = User.Identity.Name;
            return db.Users.All().Where(u => u.UserName == name).FirstOrDefault();
        }

        public LearningSystemControllerBase(IUoWLearningSystem db)
        {
            this.db = db;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}