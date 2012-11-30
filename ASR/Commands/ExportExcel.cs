namespace ASR.Commands
{
	public class ExportExcel : ExportBaseCommand
	{
		protected override string GetFilePath()
        {
            string tempPath = new Export.HtmlExport(Current.Context.Report, Current.Context.ReportItem).SaveFile("asr", "xls");
           return tempPath;
        }
	}
}
