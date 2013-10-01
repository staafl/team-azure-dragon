using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearningSystem.App.Areas.Administration.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        //
        // GET: /Administration/Admin/
	}
}