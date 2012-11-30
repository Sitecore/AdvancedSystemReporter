using System;

namespace ASR.Reports.Logs
{
    public class LogItem
    {
        public enum LogType { Info , Error, Audit, Warning };
        
        public LogType Type { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Process { get; private set; }
        public string Message { get; protected set; }


        protected LogItem()
        {

        }

        public static LogItem Make(DateTime date, string pid, string type, string text)
        {
            LogItem logItem;
            if (type == "INFO  AUDIT")
            {
                AuditItem auditItem = new AuditItem();
                auditItem.Initialize(text);
                logItem = auditItem;
            }
            else
            {
                logItem = new LogItem();
            }
            logItem.DateTime = date;
            logItem.Process = pid;
            logItem.Type = parse(type);
            logItem.Message = text;

            return logItem;
        }

        private static LogType parse(string type)
        {
            switch (type)
            {
                case "INFO":
                    return LogType.Info;                    
                case "WARN":
                    return LogType.Warning;                    
                case "ERROR":
                    return LogType.Error;                    
                default:
                    return LogType.Audit;                    
            }
        }

        //public static LogItem MakeType(string line, Sitecore.Collections.StringList Valid_Types)
        //{
        //    object[] types = {
        //                         " INFO  AUDIT ", typeof(AuditItem), LogType.Audit,
        //                         " INFO ",typeof (LogItem), LogType.Info,
        //                         " ERROR ", typeof (LogItem), LogType.Error,
        //                         " WARN ", typeof (LogItem), LogType.Warning
        //                     };
            
        //    if (string.IsNullOrEmpty(line))
        //    {
        //        return null;
        //    }

        //    string subline = line.Length > 200 ? line.Substring(0, 100) : line;
        //    //int indexof;

        //    for (int i = 0; i < types.Length; i += 3)
        //    {
        //        string keyword = (string)types[i];
        //        Type type = (Type)types[i + 1];

        //        int indexof = subline.IndexOf(keyword);
        //        if (indexof > 0)
        //        {
        //            if (Valid_Types == null || Valid_Types.Count == 0 || Valid_Types.Contains(keyword))
        //            {
        //                LogItem li;
        //                try
        //                {                   
        //                    li = (LogItem)type.CreateObject();
        //                    li.Initialize(line, indexof);                            
        //                }
        //                catch(Exception ex)
        //                {
        //                    throw new Exception(line, ex);
        //                }
        //                li.Type = (LogType)types[i + 2];
        //                return li; 
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
               
        //    }
            
        //    return null;
        //}

        internal void SetDate(DateTime filedate)
        {
            DateTime = DateTime.Parse(
                string.Format("{0} {1}:{2}:{3}", filedate.ToShortDateString(),DateTime.Hour, DateTime.Minute, DateTime.Second)
                );
        }
    }
    
}
