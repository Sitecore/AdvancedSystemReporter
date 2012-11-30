using System;
using Sitecore.Diagnostics;
using System.Collections.Specialized;
using Sitecore.Shell.Framework.Commands;
using Sitecore;
using Sitecore.Data;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using Sitecore.Text;

namespace ASR.Commands
{
    [Serializable]
    public class SetOwner : Command
    {
        // Methods
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length > 0)
            {
                StringCollection items = new StringCollection();
                foreach (var element in context.Items)
                {
                    items.Add(element.Uri.ToString(ItemUriFormat.Uri));
                }

                NameValueCollection parameters = new NameValueCollection();
                parameters["uris"] = StringUtil.StringCollectionToString(items, "|");
                Sitecore.Context.ClientPage.Start(this, "Run", parameters);
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");


            string[] uris = StringUtil.Split(args.Parameters["uris"], '|', false);
            if (args.IsPostBack)
            {
                if (args.HasResult)
                {
                    string result = args.Result;
                    if (result == "-")
                    {
                        result = string.Empty;
                    }

                    foreach (var uri in uris)
                    {
                        if (ItemUri.IsItemUri(uri))
                        {
                            Item item = Database.GetItem(ItemUri.Parse(uri));
                            item.Editing.BeginEdit();
                            item[FieldIDs.Owner] = result;
                            item.Editing.EndEdit();
                            Log.Audit(this, "Set owner: {0}", new string[] { AuditFormatter.FormatItem(item) });
                        }
                    }
                }
            }
            else
            {
                if (ItemUri.IsItemUri(uris[0]))
                {
                    ItemUri uri = ItemUri.Parse(uris[0]);
                    UrlString str6 = new UrlString("/sitecore/shell/~/xaml/Sitecore.Shell.Applications.Security.SetOwner.aspx");

                    str6.Append("id", uri.Path);
                    str6.Append("la", uri.Language.ToString());
                    str6.Append("vs", uri.Version.ToString());
                    str6.Append("db", uri.DatabaseName);
                    SheerResponse.ShowModalDialog(str6.ToString(), "450", "180", string.Empty, true);
                    args.WaitForPostBack();
                }
            }

        }
    }


}
