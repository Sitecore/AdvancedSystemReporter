using System.Globalization;
using Sitecore.Data.Items;
using ASR.Reports.Scanners;
using ASR.Interface;

namespace ASR.Reports.Viewers
{
    public class WorkflowEventViewer : BaseViewer
    {
        #region properties
        public Sitecore.Data.Database Database
        {
            get
            {
                return Sitecore.Context.ContentDatabase;
            }
        }

        public override string[] AvailableColumns
        {
            get
            {
                return new string[]
                    {
                        "Name",
                        "Date",
                        "User",
                        "OldState",
                        "NewState",
                        "Text",
                        "Paths",
                        "Language",
                        "Version"                    
                    };
            }
        } 
        #endregion
        public override void Display(DisplayElement dElement)
        {

            WorkflowEventCustom wec = dElement.Element as WorkflowEventCustom;

            if (wec == null) return;


            dElement.Value = wec.Item.Uri.ToString();
            dElement.Icon = wec.Item.Appearance.Icon;
            foreach (var column in Columns)
            {
                switch (column.Name)
                {
                    case "name":
                        dElement.AddColumn(column.Header, wec.Item.DisplayName);
                        break;
                    case "date":
                        dElement.AddColumn(column.Header, wec.WorkflowEvent.Date.ToString(GetDateFormat(null)));
                        break;
                    case "user":
                        dElement.AddColumn(column.Header, wec.WorkflowEvent.User);
                        break;
                    case "oldstate":
                        dElement.AddColumn(column.Header, getStateName(wec.WorkflowEvent.OldState));
                        break;
                    case "newstate":
                        dElement.AddColumn(column.Header, getStateName(wec.WorkflowEvent.NewState));
                        break;
                    case "text":
                        dElement.AddColumn(column.Header, wec.WorkflowEvent.Text);
                        break;
                    case "paths":
                        dElement.AddColumn(column.Header, wec.Item.Paths.Path);
                        break;
                    case "language":
                        dElement.AddColumn(column.Header, wec.Item.Language.GetDisplayName());
                        break;
                    case "version":
                        dElement.AddColumn(column.Header, wec.Item.Version.Number.ToString(CultureInfo.InvariantCulture));
                        break;
                      
                }
            }
            
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
