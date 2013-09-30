using LearningSystem.App.ViewModels;
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
            Response.StatusCode = statusCode;
            ErrorModel model = new ErrorModel
            {
                HttpStatusCode = statusCode,
                HttpStatus = Response.Status,
                Exception = exception,
                ControllerName = controllerName,
                ActionName = actionName
            };
            
            return View("Error", model);
        }
        
	}
}