using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearningSystem.App.Models
{
    public class ErrorModel
    {
        public int HttpStatusCode { get; set; }
        public string HttpStatus { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Exception Exception { get; set; }
    }
}