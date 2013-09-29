using LearningSystem.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearningSystem.App.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index(Exception exception, int statusCode, string controllerName, string actionName)
        {
            ErrorModel model = new ErrorModel
            {
                HttpStatusCode = statusCode,
                Exception = exception,
                ControllerName = controllerName,
                ActionName = actionName
            };
            Response.StatusCode = statusCode;
            
            return View("Error", model);
            //return View("Error");
        }
        
        public ActionResult test()
        {
            throw new ArgumentException();
        }
	}
}