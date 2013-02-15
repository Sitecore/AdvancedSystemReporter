using System;
using Sitecore.Data.Items;
using Sitecore.Tasks;
using System.Net.Mail;
using Sitecore.Data.Fields;
using ASR.DomainObjects;
using Sitecore;
using ASR.Interface;

namespace ASR.Commands
{
	class ReportEmailTask
	{
		public void Process(Item[] items, Sitecore.Tasks.CommandItem commandItem, ScheduleItem schedule)
		{
			CheckboxField active = commandItem.InnerItem.Fields["active"];
			if (active.Checked)
			{
				//prepare email message
				string from = commandItem["from"];
				string to = commandItem["to"];
				string subject = commandItem["subject"];

				MailMessage message = new MailMessage(from, to)
				{
					Subject = subject,
				};

				//attach reports in excel format
				MultilistField reportReferences = commandItem.InnerItem.Fields["reports"];
				foreach (Item item in reportReferences.GetItems())
				{
					ReportItem reportItem = null;
					Report report = null;
					try
					{
						reportItem = new ReportItem(item);
						report = new Report();
						foreach (var sItem in reportItem.Scanners)
						{
							report.AddScanner(sItem);
						}
						foreach (var vItem in reportItem.Viewers)
						{
							report.AddViewer(vItem);
						}
						foreach (var fItem in reportItem.Filters)
						{
							report.AddFilter(fItem);
						}
						report.Run();

						//attach to mail message
						string tempPath = new ASR.Export.HtmlExport(report, reportItem).SaveFile("Automated report " + reportItem.Name, "xls");
						Attachment newAttachment = new Attachment(tempPath);
						message.Attachments.Add(newAttachment);

					}
					catch (Exception ex)
					{
						message.Body += String.Format("An error occured while running '{0}'\n{1}\n\n", reportItem.Name, ex.ToString());
					}
				}

				MainUtil.SendMail(message);
			}
		}				
	}
}
