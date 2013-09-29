using LearningSystem.Data;
using LearningSystem.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TeamAzureDragon.Utils;

namespace LearningSystem.App
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/fwlink/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            VersionedString.CurrentVersion = 1;
            VersionedString.DefaultVersion = 1;

            Database.SetInitializer<LearningSystemContext>(new MigrateDatabaseToLatestVersion<LearningSystemContext, Configuration>());

            using (var context = new LearningSystemContext())
            {
                Console.WriteLine(context.Users.Count());
                //new DefaultInitializer().InitializeDatabaseWithSetInitializer(context);
            }

            AreaRegistration.RegisterAllAreas();


            // keep this first to ensure WebAPI routing
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


        }
    }
}
