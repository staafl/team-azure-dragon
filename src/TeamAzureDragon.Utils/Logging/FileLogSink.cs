using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Threading;

namespace TeamAzureDragon.Utils.Log
{
    public class FileLogSink : ILogSink
    {
        public string FilePath { get; set; }
        
        string GetLogPath(string log) {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, log + ".log");
        }
        
        public string RetrieveLog(string log) {
            return File.ReadAllText(GetLogPath(log));
        }
        
        public void Log(string log, string message) {
            File.AppendAllText(GetLogPath(log), message + Environment.NewLine + Environment.NewLine);
        }
    }
}
