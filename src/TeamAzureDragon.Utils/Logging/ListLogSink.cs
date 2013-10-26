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
    public class ListLogSink : ILogSink
    {
        
        readonly Dictionary<string, List<string>> logs = 
             new Dictionary<string, List<string>>();
        
        List<string>  GetLogList(string log) {
            log = log.ToUpper();
            List<string> logList;
            if (!logs.TryGetValue(log, out logList))
            {
                logs[log] = logList = new List<string>();
            }
            return logList;
        }
        public string RetrieveLog(string log) {
            return string.Join("\n", GetLogList(log));
        }
        
        public void Log(string log, string message) {
            GetLogList(log).Add(message);
        }
    }
}