
namespace ASR.Reports.Logs
{
	public class TypeFilter : ASR.Interface.BaseFilter
	{
		public override bool Filter(object element)
		{
			LogItem logElement = element as LogItem;
			if (logElement == null)
			{
				return false;
			}
			return logElement.Type == LogItem.LogType.Audit;
		}
	}
}
