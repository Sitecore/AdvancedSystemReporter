using Sitecore.Web.UI.Sheer;
using ASR.Interface;

namespace ASR
{
    public class ReportRunner
    {
        public void RunCommand(ClientPipelineArgs args)
        {
            //get parameters from the ui
            Sitecore.Context.ClientPage.SendMessage(this, "ASR.MainForm:updateparameters");

            Current.Context.Report = 
              Current.Context.ReportItem.TransformToReport(Current.Context.Report);
            //if (Current.Context.Report == null)
            //{
            //  Current.Context.Report = new Report();
            //}
            //foreach (var sItem in Current.Context.ReportItem.Scanners)
            //{
            //  Current.Context.Report.AddScanner(sItem);
            //}
            //foreach (var vItem in Current.Context.ReportItem.Viewers)
            //{
            //  Current.Context.Report.AddViewer(vItem);
            //}
            //foreach (var fItem in Current.Context.ReportItem.Filters)
            //{
            //  Current.Context.Report.AddFilter(fItem);
            //}

            Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute(
                "Scanning...",
                "Scanning items",
                "",
                 Current.Context.Report.Run,
                "MainForm:runfinished",
                new object[] { });
        }
    }
}
