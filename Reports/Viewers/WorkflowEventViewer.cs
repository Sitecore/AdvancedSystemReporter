using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using ASR.Reports.Scanners;
using ASR.Interface;

namespace ASR.Reports.Viewers
{
    public class WorkflowEventViewer : BaseViewer
    {
        public Sitecore.Data.Database Database
        {
            get
            {
                return Sitecore.Context.ContentDatabase;
            }
        }
        public override void Display(DisplayElement dElement)
        {

            WorkflowEventCustom wec = dElement.Element as WorkflowEventCustom;

            if (wec == null) return;


            dElement.Value = wec.Item.Uri.ToString();
            dElement.Icon = wec.Item.Appearance.Icon;
            dElement.AddColumn("Name", wec.Item.DisplayName);
            dElement.AddColumn("Date", wec.WorkflowEvent.Date.ToString("dd/MM/yyyy HH:mm:ss"));
            dElement.AddColumn("User", wec.WorkflowEvent.User);
            dElement.AddColumn("OldState", getStateName(wec.WorkflowEvent.OldState));
            dElement.AddColumn("NewState", getStateName(wec.WorkflowEvent.NewState));
            dElement.AddColumn("Text", wec.WorkflowEvent.Text);

        }
        private string getStateName(string stateID)
        {
            Item state = Database.GetItem(stateID);
            if (state != null)
            {
                return state.Name;
            }
            return string.Empty;
        }
    }
}
