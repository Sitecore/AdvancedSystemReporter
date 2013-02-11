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
