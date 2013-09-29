using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamAzureDragon.Utils;

namespace LearningSystem.App
{
    public class ApiController : System.Web.Http.ApiController
    {
        IUnitOfWork db;
        public ApiController(IUnitOfWork db)
        {
            this.db = db;
        }

        public string Status()
        {
            return "System online";
        }

        [HttpPost]
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
        //// GET 
        // api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}