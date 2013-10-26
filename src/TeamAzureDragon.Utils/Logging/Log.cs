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
    public static class Log
    {
        static int timeStampId = 0;
        public static string GetTimeStamp(DateTime dateTime, bool squareBrackets = false, bool seconds = true, bool ms = true, bool id = false, bool threadId = false)
        {
            timeStampId += 1;
            timeStampId %= 100000;
            return (squareBrackets ? "[" : "")
                + dateTime.ToString("yyyy-MM-dd HH:mm" + (seconds ? ":ss" : "") + (ms ? ".fff" : "")) 
                + (id ? "/" + timeStampId.ToString() : "")
                + (threadId ? "//" + Thread.CurrentThread.ManagedThreadId.ToString() : "")
                + (squareBrackets ? "]" : "");

        }
        
        static readonly object locker = new object();
        
        public static void Initialize(ILogSink logSink) {
            Log.LogSink = logSink;
            Log.Trace("Logging started, using " + logSink.GetType().Name);
        }
        public static ILogSink LogSink { get; set; }
        
        static readonly string NL = Environment.NewLine;
        
        public static string Trace(string message, string thread = null)
        {
            var timeStamp = GetTimeStamp(DateTime.Now, true, true, true, true, true);
            thread = thread ?? timeStamp;
            lock(locker) {
                LogSink.Log("trace",
                    // [2012-10-01 22:15:24.455/19978] (2012-10-01 22:15:24.455/19978)
                    timeStamp + " (" + thread + ")" + NL + 
                    message);
            }
            return thread;

        }
        public static string Exception(Exception exception, string thread = null, string place = null)
        {
            var timeStamp = Log.GetTimeStamp(DateTime.Now, true, true, true, true, true);
            thread = thread ?? timeStamp;
            lock(locker) {
                if (exception == null) {
                    return Log.Trace("Null exception received!", thread);
                }
                LogSink.Log("exception",
                    // [2012-10-01 22:15:24.455/19978] (2012-10-01 22:15:24.455/19978:AppDomain Handler)
                    timeStamp + " (" + thread + ":" + (place ?? "unknown") + ") " + NL + 
                    exception);
                Log.Trace("Exception: " + exception.Message, thread);
            }
            return thread;
        }
    }
}
