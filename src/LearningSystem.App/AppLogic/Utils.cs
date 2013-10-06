using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LearningSystem.App.AppLogic
{
    public class Utils
    {
        // http://stackoverflow.com/questions/5853294/how-do-i-get-the-site-root-url
        public static string GetSiteRootUrl()
        {
            string protocol;

            if (HttpContext.Current.Request.IsSecureConnection)
                protocol = "https";
            else
                protocol = "http";

            StringBuilder uri = new StringBuilder(protocol + "://");

            string hostname = HttpContext.Current.Request.Url.Host;

            uri.Append(hostname);

            int port = HttpContext.Current.Request.Url.Port;

            if (port != 80 && port != 443)
            {
                uri.Append(":");
                uri.Append(port.ToString());
            }

            return uri.ToString();
        }
    }
}