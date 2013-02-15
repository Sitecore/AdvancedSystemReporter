using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Web.UI.Sheer;

namespace ASR.Commands
{
    public class Save : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Start");
        }

        public void Start(ClientPipelineArgs args)
        {

            if (args.IsPostBack)
            {
                Database database =
                    Sitecore.Configuration.Factory.GetDatabase(
                Settings.Instance.ConfigurationDatabase);
                Assert.IsNotNull(database, "configuration database is null");

                Item report = database.GetItem(Current.Context.ReportItem.ID);

                Assert.IsNotNull(report, "can't find report item");

                Sitecore.Context.ClientPage.SendMessage(this, "ASR.MainForm:updateparameters");

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    Item newItem = ItemUtil.AddFromTemplate(args.Result, "System/ASR/Saved Report", report);
                    using (new EditContext(newItem))
                    {
                        newItem["parameters"] = Current.Context.ReportItem.SerializeParameters("^", "&");
                        newItem[Sitecore.FieldIDs.Owner] = Sitecore.Context.User.Name;
                    }
                }
            }
            else
            {
                Sitecore.Context.ClientPage.ClientResponse.Input("Enter the name of the new blog entry:",
                   "report name", Sitecore.Configuration.Settings.ItemNameValidation, "'$Input' is not a valid name.", 
                   Sitecore.Configuration.Settings.MaxItemNameLength);
                args.WaitForPostBack(true);
            }
        }
    }
}
