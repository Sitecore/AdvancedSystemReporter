using System;

namespace ASR.Reports.Logs
{
    public class LogFilter : ASR.Interface.BaseFilter
    {
        public int Age { get; set; }
        public override bool Filter(object element)
        {
            LogItem li = element as LogItem;

            if (li == null || Age < 0)
            {
                return true;
            }

            return DateTime.Now.CompareTo(li.DateTime.AddHours(Age)) < 0;
        }
    }
}
