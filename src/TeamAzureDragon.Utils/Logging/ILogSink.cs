using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Xml;
using System.Threading;

namespace TeamAzureDragon.Utils.Log
{
    public interface ILogSink
    {
        void Log(string log, string message);
        string RetrieveLog(string log);
    }
}