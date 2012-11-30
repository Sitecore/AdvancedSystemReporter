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

		public override void Display(DisplayElement _dElement)
		{
			Debug.ArgumentNotNull(_dElement, "element");

			LogItem logElement = _dElement.Element as LogItem;

			if (logElement == null)
			{
				return;
			}

			_dElement.Value = _dElement.Element.ToString();
			_dElement.Icon = getIcon(logElement);

			_dElement.AddColumn("Type", logElement.Type.ToString());
			_dElement.AddColumn("Date", logElement.DateTime.ToShortDateString());
			_dElement.AddColumn("Time", logElement.DateTime.ToShortTimeString());

			AuditItem ai = logElement as AuditItem;
			if (ai != null)
			{
				_dElement.AddColumn("User", ai.User);
				_dElement.AddColumn("Verb", ai.Verb);

				_dElement.Value = ai.ItemUri == null ? "" : ai.ItemUri.ToString();
			}

			_dElement.AddColumn("Message", logElement.Message);
		}

		private string getIcon(LogItem logElement)
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
