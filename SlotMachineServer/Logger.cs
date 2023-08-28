using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotMachineServer
{
    public static class Logger
    {
        private static string log_output = "";
        public static LoggingLevel loggingLevel = LoggingLevel.LiteEventsAndErrors;
        public static int LogHistroySize = 60;
        public static List<LogData> logevents = new List<LogData>();

        public static void Log(string Data, LoggingLevel type)//called async...
        {
            if (logevents.Count > LogHistroySize) {
                logevents.RemoveRange(0, logevents.Count - LogHistroySize);
            }
            if(loggingLevel == LoggingLevel.LiteEventsAndErrors)
            {
                switch (type)
                {
                    case LoggingLevel.None:
                        break;
                    case LoggingLevel.All:
                        logevents.Add(new LogData(Data, DateTime.Now, type));
                        break;
                    case LoggingLevel.Events:
                        logevents.Add(new LogData(Data, DateTime.Now, type));
                        break;
                    case LoggingLevel.LiteEvents:
                        logevents.Add(new LogData(Data, DateTime.Now, type));
                        break;
                    case LoggingLevel.LiteEventsAndErrors:
                        logevents.Add(new LogData(Data, DateTime.Now, type));
                        break;
                    case LoggingLevel.Errors:
                        logevents.Add(new LogData(Data, DateTime.Now, type));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if(type == loggingLevel)
                {
                    logevents.Add(new LogData(Data, DateTime.Now, type));
                }
            }
        }
        public static string GetLog()
        {
            TryAgain:
            log_output = "";
            try
            {
                foreach(LogData logItem in logevents)
                {
                    log_output += logItem.data;
                }
            }
            catch (Exception)
            {
                goto TryAgain;
            }
            return log_output;
        }
        public class LogData
        {
            public string data;
            LoggingLevel level;
            public LogData(string data, DateTime time, LoggingLevel type)
            {
                this.data = data.Insert(0,"\n" + time.ToString() + "|");
                level = type;
            }
        }
    }
    public enum LoggingLevel
    {
        None, All, Events, LiteEvents, LiteEventsAndErrors, Errors
    }

}
