using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;

namespace ASR.Export
{
    public class CsvExport
    {
        public char Separator
        {
            get { return ','; }
        }

        public string Save(string prefix, string extension)
        {
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
           
            string tempPath =
                Sitecore.IO.FileUtil.GetWorkFilename(Sitecore.Configuration.Settings.TempFolderPath, prefix, extension);
            
            HashSet<string> headers = new HashSet<string>();
            IEnumerable<DisplayElement> results = Current.Context.Report.GetResultElements();
            foreach (var dElement in results)
            {
                foreach (var header in dElement.GetColumnNames())
                {
                    headers.Add(header);
                }
            }
            writeHeader(oStringWriter,headers);
            writeValues(oStringWriter, headers, results);

            System.IO.File.WriteAllText(tempPath, oStringWriter.ToString());
           
            return tempPath;
        }

        private void writeHeader(System.IO.StringWriter writer,IEnumerable<string> headers)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var header in headers)
            {
                sb.AppendFormat("{0}{1}",Separator,header);                
            }
            writer.WriteLine(sb.ToString().Substring(1));
        }

        private void writeValues(System.IO.StringWriter writer, IEnumerable<string> headers, IEnumerable<DisplayElement> results)
        {
            foreach (var row in results)
            {
                IEnumerator<string> enumerator = headers.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    writer.Write(row.GetColumnValue(enumerator.Current));
                    while (enumerator.MoveNext())
                    {
                        writer.Write(Separator);
                        writer.Write(row.GetColumnValue(enumerator.Current));
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
