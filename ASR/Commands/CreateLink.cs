using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Collections.Specialized;
using Sitecore.Collections;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Text;
using Sitecore.SecurityModel;

namespace ASR.Commands
{
	public class CreateLink : Command
	{
		private readonly string databasename = "core";


		private readonly string parentid = "{EAEFFD74-055E-453F-9860-47CD385B6580}"; //links
		private readonly string templateid = "{72450C9C-98C4-4117-88B7-573110C7E0C0}";

		private readonly string linktemplate = "<link linktype=\"internal\" url=\"/Applications/Advanced System Reporter\" querystring=\"\" id=\"{605C3AD5-4D6E-4297-98C9-7638BE5A82EA}\" target=\"\" />";

		public override void Execute(CommandContext context)
		{
			if (Current.Context.ReportItem == null)
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("You need to open a report first.");
				return;
			}

			string name = Current.Context.ReportItem.Name;
			string icon = Current.Context.ReportItem.Icon;
			if (string.IsNullOrEmpty(icon))
				icon = "Applications/32x32/folder_view.png";

			Database database = Sitecore.Configuration.Factory.GetDatabase(databasename);
			if (database == null)
			{
				return;
			}

			Item parent = database.GetItem(parentid);
			if (parent == null)
			{
				return;
			}

			string username = Sitecore.Context.User.Name.Replace('\\', '_');

			Item folder = parent.Axes.GetChild(username);

			if (folder == null)
			{
				folder = addItem(username, "Common/Folder", parent);
			}

			if (folder == null)
				folder = parent;

			Item link = addItem(name, templateid, folder);



            //NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("id", new ID(Current.Context.ReportItem.Id).ToString());
            //foreach (var item in Current.Context.ReportItem.Scanners)
            //{
            //    foreach (var p in item.Parameters)
            //    {
            //        nvc.Add(string.Concat(item.Name, "^", p.Name), p.Value);
            //    }
            //}
            //foreach (var item in Current.Context.ReportItem.Filters)
            //{
            //    foreach (var p in item.Parameters)
            //    {
            //        nvc.Add(string.Concat(item.Name, "^", p.Name), p.Value);
            //    }
            //}
            //foreach (var item in Current.Context.ReportItem.Viewers)
            //{
            //    foreach (var p in item.Parameters)
            //    {
            //        nvc.Add(string.Concat(item.Name, "^", p.Name), p.Value);
            //    }
            //}
            
            

			if (link != null)
			{
				using (new SecurityDisabler())
				{
					link.Editing.BeginEdit();
					link["Icon"] = icon;
					link["__icon"] = icon;
					link["application"] = linktemplate;
					link["display name"] = name;
					//link["parameters"] = string.Format("id={0}", new ID(Current.Context.ReportItem.Id));
					//link["parameters"] = Sitecore.StringUtil.NameValuesToString(nvc, "&");
                    link["parameters"] = Current.Context.ReportItem.SerializeParameters();
					link.Editing.EndEdit();
				}

				makeLink(name, icon, link);
			}
		}

		private void makeLink(string name, string icon, Item link)
		{
			ListString desktoplinks = new ListString(Registry.GetString("/Current_User/Desktop/Links"), '|');
			ListString parameters = new ListString('^');
			parameters.Add(databasename);
			parameters.Add(link.ID.ToString());
			parameters.Add(name);
			parameters.Add(icon);
			desktoplinks.Add(parameters.ToString());
			Registry.SetString("/Current_User/Desktop/Links", desktoplinks.ToString());
			Sitecore.Context.ClientPage.ClientResponse.Alert("Link created. It will not appear until Desktop is refreshed");
		}

		private Item addItem(string name, string template, Item parent)
		{
			using (new Sitecore.SecurityModel.SecurityDisabler())
			{
				return ItemUtil.AddFromTemplate(name, template, parent);
			}
		}
		public override CommandState QueryState(CommandContext context)
		{
			if (Current.Context.ReportItem == null)
			{
				return CommandState.Disabled;
			}
			return base.QueryState(context);
		}
	}
}


