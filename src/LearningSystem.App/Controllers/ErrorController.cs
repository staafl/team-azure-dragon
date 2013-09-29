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
        public ActionResult Index()
        {
            return View("Error");
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = 404; 
            return View("NotFound");
        }

        public ViewResult NoAccess()
        {
            Response.StatusCode = 403; 
            return View("NoAccess");
        }

        public ActionResult test()
        {
            throw new ArgumentException();
        }
	}
}