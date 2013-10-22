using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAzureDragon.WebUtils
{
    public class LogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Log.Exception(filterContext.Exception, null, "LogAttribute.OnActionExecuted");
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
