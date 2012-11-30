using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Export
{
    class XMLExport
    {
        public string Save(string prefix, string extension)
        {
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();

            string tempPath =
                Sitecore.IO.FileUtil.GetWorkFilename(Sitecore.Configuration.Settings.TempFolderPath, prefix, extension);

            
            writeValues(oStringWriter);

            System.IO.File.WriteAllText(tempPath, oStringWriter.ToString());

            return tempPath;
        }

        private void writeValues(System.IO.StringWriter oStringWriter)
        {
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.ConformanceLevel = System.Xml.ConformanceLevel.Auto;
            
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(oStringWriter,settings);
            
            writer.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteStartElement("report");
            writer.WriteAttributeString("date", DateTime.Now.ToString("yyyyMMdd_HHmm"));
            foreach (var row in Current.Context.Report.GetResultElements())
            {
                
                writer.WriteStartElement("result");
                foreach (var col in row.GetColumnNames())
                {
                    writer.WriteStartElement("column");
                    writer.WriteAttributeString("name", col);
                    writer.WriteString(row.GetColumnValue(col));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            
            writer.Flush();
        }
    }
}
