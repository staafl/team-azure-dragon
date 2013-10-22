using LearningSystem.App.Controllers;
using LearningSystem.Data;
using LearningSystem.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using TeamAzureDragon.Utils;
using LearningSystem.App.AppLogic;
using LearningSystem.AnswerHandlers.CSharp;
using LearningSystem.AnswerHandlers.Standard;

namespace LearningSystem.App
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/fwlink/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //VersionedString.CurrentVersion = 1;
            //VersionedString.DefaultVersion = 1;

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

            AnswerHandlerFactory.LoadPlugin(typeof(CSharpAnswerHandler).Assembly);
            AnswerHandlerFactory.LoadPlugin(typeof(ListAnswerHandler).Assembly);
            
            StartKeepAliveThread();
        }
        
        public static class Settings
        {
            static readonly XmlDocument settings = new XmlDocument();

            public static void Initialize()
            {
                settings.Load(AppDomain.CurrentDomain.BaseDirectory + "\\settings.xml");
            }

            public static string RootUrl {
                get {
                    return settings.SelectSingleNode("/settings/rootUrl").InnerXml.Trim();
                }
            }
        }
        
        static void StartKeepAliveThread() {
            new Thread(() =>
            {
                try {
                    // Log.Trace("Keep-alive thread started.");
                    while(true)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(15));
                        // var thread = Log.Trace("Keep-alive thread awoke.");
                        using (var client = new System.Net.WebClient())
                        {
                            var url = Settings.RootUrl + "/";
                            // Log.Trace("Attempting to download {0}".Fmt(url), thread);
                            client.DownloadString(url);
                            // Log.Trace("Success!", thread);
                        }
                    }
                }
                catch(Exception ex) {
                    // Log.Exception(ex, null, "keep-alive thread catch.");
                    throw;
                }
            }).Start();
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            // TODO: isn't there a better approach?

            Exception exception = Server.GetLastError();
            Server.ClearError();

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("exception", exception);  
                      
            if (exception.GetType() == typeof(HttpException))
            {
                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());
            }
            else
            {
                routeData.Values.Add("statusCode", 500);
            }

            // get controller and action name
            var httpContext = ((MvcApplication)sender).Context;
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    routeData.Values.Add("controllerName", currentRouteData.Values["controller"].ToString());
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    routeData.Values.Add("actionName", currentRouteData.Values["action"].ToString());
                }
            }

            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            Response.End();
        }
    }
}
