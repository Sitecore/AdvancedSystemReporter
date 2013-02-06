using ASR.DomainObjects;
using ASR.Interface;
using Sitecore.Diagnostics;

namespace ASR.Reports.Logs
{
	public class LogViewer : ASR.Interface.BaseViewer
	{
		private readonly string ICON_WARN = "Applications/32x32/warning.png";
		private readonly string ICON_ERROR = "Applications/32x32/delete.png";
		private readonly string ICON_INFO = "Applications/32x32/information2.png";
		private readonly string ICON_AUDIT = "Applications/32x32/scroll_view.png";


        public override string[] AvailableColumns
        {
            get { return new string[] {"Type", "Date", "Message", "User", "Verb"}; }
        }

		public override void Display(DisplayElement dElement)
		{
			Debug.ArgumentNotNull(dElement, "element");

			var logElement = dElement.Element as LogItem;

			if (logElement == null)
			{
				return;
			}

			dElement.Icon = GetIcon(logElement);
            var ai = logElement as AuditItem;
		    foreach (var column in Columns)
		    {
		        switch (column.Name)
		        {
                    case "type":
                        dElement.AddColumn(column.Header, logElement.Type.ToString());
		                break;
                    case "date":
                        dElement.AddColumn(column.Header, logElement.DateTime.ToString(GetDateFormat(null)));
		                break;
                    case "message":
                        dElement.AddColumn(column.Header, logElement.Message);
		                break;
                    case "user":
                        if (ai != null)
                        {
                            dElement.AddColumn(column.Header, ai.User);
                        }
		                break;
                    case "verb":
                        if (ai != null)
                        {
                            dElement.AddColumn(column.Header, ai.Verb);
                        }
		                break;
		        }
		    }
			
			if (ai != null)
			{		
				dElement.Value = ai.ItemUri == null ? "" : ai.ItemUri.ToString();
			}
			else
			{
                dElement.Value = dElement.Element.ToString();
			    
			}
		}

		private string GetIcon(LogItem logElement)
		{
			switch (logElement.Type)
			{
				case LogItem.LogType.Audit:
					return ICON_AUDIT;
				case LogItem.LogType.Warning:
					return ICON_WARN;
				case LogItem.LogType.Info:
					return ICON_INFO;
				case LogItem.LogType.Error:
					return ICON_ERROR;
			}
			return string.Empty;
		}
	}
}
