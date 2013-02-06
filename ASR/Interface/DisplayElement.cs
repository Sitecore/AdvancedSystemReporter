using System;
using System.Collections.Generic;

namespace ASR.Interface
{
    public class DisplayElement
    {
        private Dictionary<string,string> columns;                

        /// <summary>
        /// Object returned by the scanner.
        /// </summary>
        public object Element { get; private set; }

        public string Header { get; set; }

        public string Value { get; set; }
        
        public string Icon { get; set; }

        public string ExtraInfo { get; set; }

        internal DisplayElement(object element)
        {
            Element = element;
            columns = new Dictionary<string, string>();
            Icon = "";
            ExtraInfo = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Valid column names for this element</returns>
        public IEnumerable<string> GetColumnNames()
        {
            return columns.Keys;
        }
        
        /// <summary>
        /// Add a new column value to this element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddColumn(string name, string value)
        {
            if (!HasColumn(name))
            {
                columns.Add(name, value);                
            }
        }

        /// <summary>
        /// Checks if a column name has also been added. You cannot repeat column names
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasColumn(string name)
        {
            return columns.ContainsKey(name);
        }

        /// <summary>
        /// Queries the value of a particular column
        /// </summary>
        /// <param name="name">name of the colmn</param>
        /// <returns>null if the column is not defined.</returns>
        public string GetColumnValue(string name)
        {
            if (!columns.ContainsKey(name)) return null;
            return columns[name] as string;
        }
    }
}
