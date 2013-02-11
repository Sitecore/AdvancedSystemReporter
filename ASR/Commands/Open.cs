using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using CorePoint.DomainObjects.SC;
using ASR.DomainObjects;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace ASR.Commands
{
    class Open : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Start");
        }

        public void Start(ClientPipelineArgs args)
        {

            if (!args.IsPostBack)
            {
                Util.ShowItemBrowser(
                    "Select the report",
                    "Select the report",
                    "Database/32x32/view_h.png",
                    "Select",
                    Current.Context.Settings.ReportsFolder,
                    Current.Context.ReportItem == null ? Current.Context.Settings.ReportsFolder : Current.Context.ReportItem.Path,
                    Current.Context.Settings.ConfigurationDatabase);
                args.WaitForPostBack();
            }
            else
            {
                if (!Sitecore.Data.ID.IsID(args.Result))
                {
                    return;
                }
                Database database = Sitecore.Configuration.Factory.GetDatabase(Current.Context.Settings.ConfigurationDatabase);
                Sitecore.Diagnostics.Assert.IsNotNull(database,"no configuration databsae");

                Item item = database.GetItem(args.Result);

                Sitecore.Diagnostics.Assert.IsNotNull(item, "report item cannot be loaded");

                switch(item.Template.Key)
                {
                    case "report":
                    SCDirector director = new SCDirector(Current.Context.Settings.ConfigurationDatabase, "en");

                    ReportItem rItem = director.LoadObjectFromItem<ReportItem>(item);
                    if (rItem != null)
                    {
                        Current.Context.ReportItem = rItem;
                        Current.Context.Report = null;
                        Sitecore.Context.ClientPage.SendMessage(this, "ASR.MainForm:update");                 
                    }
                        break;
                   
                    case "saved report":
                
                    Message m = Message.Parse(this, "ASR.MainForm:openlink");
                    System.Collections.Specialized.NameValueCollection nvc = 
                    Sitecore.StringUtil.ParseNameValueCollection(item["parameters"], '&', '=');

                    m.Arguments.Add(nvc);
                    Sitecore.Context.ClientPage.SendMessage(m);
                    break;
                }

               
            }
        }

    }
}
