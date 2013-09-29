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

namespace LearningSystem.App.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork db;
        public HomeController(IUnitOfWork db)
        {
            this.db = db;
        }

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
                Environment.Exit(0);
                throw new ApplicationException("Shouldn't happen");
            }
            throw new HttpException(400, "");
        }

        public ActionResult SaveSkill()
        {
            Response.Expires = -1;
            try
            {
                HttpPostedFileBase file = Request.Files["skills-uploader"];

                ZipFile zipFile = ZipFile.Read(file.InputStream);
                StringBuilder zipContent = new StringBuilder();

                

                foreach (var zipEntry in zipFile.Entries)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    zipEntry.Extract(memoryStream);

                    memoryStream.Position = 0;
                    StreamReader reader = new StreamReader(memoryStream);
                    zipContent.AppendLine(reader.ReadToEnd());
                }

                string s = zipContent.ToString();


                Response.ContentType = "application/json";
                Response.Write("{}");
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }

            return Content("");
        }


    }
}