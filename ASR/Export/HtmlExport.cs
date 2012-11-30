using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ASR.DomainObjects;
using Sitecore;
using Sitecore.Security.Accounts;
using ASR.Interface;

namespace ASR.Export
{
	public class HtmlExport
	{
		protected Report report;
		protected ReportItem reportItem;

	  public HtmlExport(Report report, ReportItem reportItem)
		{
			this.report = report;
			this.reportItem = reportItem;
		}

		public string SaveFile(string prefix, string extension)
		{
			var oStringWriter = new System.IO.StringWriter();
			var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);

			var headers = new HashSet<string>();
			var results = report.GetResultElements();

			foreach (var dElement in results)
			{
				foreach (var header in dElement.GetColumnNames())
				{
					headers.Add(header);
				}
			}

			printReport(oHtmlTextWriter, headers, results);


			var tempPath =
				Sitecore.IO.FileUtil.GetWorkFilename(Sitecore.Configuration.Settings.TempFolderPath, prefix, extension);

			System.IO.File.WriteAllText(tempPath, oStringWriter.ToString());
			return tempPath;

		}

		private void printReportHeader(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
			//report name
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Th);
			writer.Write("Report name:");
			writer.RenderEndTag();//th

			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.Write(this.reportItem.Name);
			writer.RenderEndTag();//td
			writer.RenderEndTag();//tr

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Th);
			writer.Write("Run by:");
			writer.RenderEndTag();//th

			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			User currentUser = Sitecore.Context.User;
			writer.Write(String.IsNullOrEmpty(currentUser.Profile.FullName) ? currentUser.Name : String.Format("{0} ({1})", currentUser.Profile.FullName, currentUser.Name));
			writer.RenderEndTag();//td
			writer.RenderEndTag();//tr

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Th);
			writer.Write("Exported:");
			writer.RenderEndTag();//th

			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.Write(DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
			writer.RenderEndTag();//td
			writer.RenderEndTag();//tr

			//print report parameters
			foreach (var scanner in this.reportItem.Scanners)
			{
				addParameters(writer, scanner);
			}

			foreach (var refItem in this.reportItem.Filters)
			{
				addParameters(writer, refItem);
			}
			foreach (var viewer in this.reportItem.Viewers)
			{
				addParameters(writer, viewer);
			}

			writer.RenderEndTag();//table
		}

    private Sitecore.Data.Database Db
    {
      get
      {
        return Sitecore.Context.ContentDatabase ?? Sitecore.Configuration.Factory.GetDatabase("master");
      }
    }
		/// <summary>
		/// Prints individual parameters of a reference item.
		/// </summary>
		/// <param name="oHtmlTextWriter">The o HTML text writer.</param>
		/// <param name="referenceItem">The reference item.</param>
		private void addParameters(HtmlTextWriter oHtmlTextWriter, ReferenceItem referenceItem)
		{
			if (referenceItem.Parameters.Count() > 0)
			{
				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

				oHtmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
				oHtmlTextWriter.Write(referenceItem.Name);
				oHtmlTextWriter.RenderEndTag();//th
				oHtmlTextWriter.RenderEndTag();//tr
			}

			foreach (var param in referenceItem.Parameters)
			{
				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
				oHtmlTextWriter.Write(param.Title);
				oHtmlTextWriter.RenderEndTag();//td

				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
				var value = param.Value;
				switch (param.Type)
				{
				  case "Dropdown":
				    {
				      var friendlyValue = param.PossibleValues().FirstOrDefault(v => v.Value == value);
				      if (friendlyValue != null)
				      {
				        value = friendlyValue.Name;
				      }
				    }
				    break;
				  case "Item Selector":
				    {
				      var item = this.Db.GetItem(param.Value);
				      if (item != null)
				      {
				        value = item.Paths.FullPath;
				      }
				    }
				    break;
				  case "Date picker":
				    value = DateUtil.FormatIsoDate(param.Value, "dd/MM/yyyy hh:mm");
				    break;
				}

				oHtmlTextWriter.Write(value);
				oHtmlTextWriter.RenderEndTag();//td

				oHtmlTextWriter.RenderEndTag();
			}
		}

		private void printReport(HtmlTextWriter oHtmlTextWriter, IEnumerable<string> headers, IEnumerable<DisplayElement> results)
		{
			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Html);
			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Head);
			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Title);
			oHtmlTextWriter.Write(string.Format("ASR report {0} {1} ", reportItem.Name, DateTime.Now.ToString("dd/MM/yyyy - HH:mm")));
			oHtmlTextWriter.RenderEndTag(); //title
			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Style);
			oHtmlTextWriter.Write("td {padding: 1px 5px 1px 5px}");
			oHtmlTextWriter.RenderEndTag(); //style
			oHtmlTextWriter.RenderEndTag(); //head
			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Body);

			//print parameters
			printReportHeader(oHtmlTextWriter);

			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Table);
			oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
			foreach (var header in headers)
			{
				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
				oHtmlTextWriter.Write(header);
				oHtmlTextWriter.RenderEndTag(); //th
			}
			oHtmlTextWriter.RenderEndTag(); //tr
			foreach (var row in results)
			{
				oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
				foreach (var col in headers)
				{
					oHtmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
					oHtmlTextWriter.Write(row.GetColumnValue(col) ?? "&nbsp;");
					oHtmlTextWriter.RenderEndTag(); //td
				}
				oHtmlTextWriter.RenderEndTag(); //tr
			}
			oHtmlTextWriter.RenderEndTag(); //table

			oHtmlTextWriter.RenderEndTag(); // body
			oHtmlTextWriter.RenderEndTag(); //html
		}
	}
}
