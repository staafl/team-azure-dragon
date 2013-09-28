using LearningSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearningSystem.App.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public string Status()
        {
            return "System online";
        }

        [HttpPut]
        public string WipeDatabase(string sure)
        {
            if (sure.ToLower() == "yes")
            {
                string[] tables =
                {
"dbo.AspNetUserRoles",
"dbo.AspNetTokens",
"dbo.AspNetUserClaims",
"dbo.AspNetUserLogins",
"dbo.AspNetUserManagement",
"dbo.AspNetUserSecrets",
"dbo.AspNetUsers",
"dbo.AspNetRoles",
                };
                using (var context = new LearningSystemContext())

                    foreach (var tableName in new string[0])
                    {
                        context.Database.ExecuteSqlCommand(string.Format("DELETE FROM {0}", tableName));
                    }
                return "Database successfully cleared.";
            }
            throw new HttpException(400, "");
        }

        [HttpPost]
        public void Kill(string sure)
        {
            if (sure.ToLower() == "yes")
            {
                Environment.Exit();
                throw new ApplicationException("Shouldn't happen");
            }
            throw new HttpException(400, "");
        }
    }
}