using Sitecore;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Text;
using Sitecore.SecurityModel;

namespace ASR.Commands
{
	public class CreateLink : Command
	{
	    private const string Databasename = "core";
	    private const string Parentid = "{EAEFFD74-055E-453F-9860-47CD385B6580}"; //links
	    private const string Templateid = "{72450C9C-98C4-4117-88B7-573110C7E0C0}";
	    private const string Linktemplate = "<link linktype=\"internal\" url=\"/Applications/Advanced System Reporter\" querystring=\"\" id=\"{605C3AD5-4D6E-4297-98C9-7638BE5A82EA}\" target=\"\" />";
	    private const string DefaultIcon = "Applications/32x32/folder_view.png";
	    private const string ConfirmationMessage = "Link created. It will not appear until Desktop is refreshed";

	    public override void Execute(CommandContext context)
		{
			if (Current.Context.ReportItem == null)
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("You need to open a report first.");
				return;
			}
            Sitecore.Context.ClientPage.SendMessage(this, "ASR.MainForm:updateparameters");

			var name = Current.Context.ReportItem.Name;
			var icon = Current.Context.ReportItem.Icon;
			if (string.IsNullOrEmpty(icon))
				icon = DefaultIcon;		

		    var linkItem = AddItem(name, Templateid);

		    if (linkItem == null) return;

		    using (new SecurityDisabler())
		    {
		        linkItem.Editing.BeginEdit();		        
		        linkItem[FieldIDs.Icon] = icon;
                linkItem[FieldIDs.DisplayName] = name;
                linkItem["Icon"] = icon;
		        linkItem["application"] = Linktemplate;		        
		        linkItem["parameters"] = Current.Context.ReportItem.SerializeParameters();
		        linkItem.Editing.EndEdit();
		    }

		    MakeLink(name, icon, linkItem);
		}

		private void MakeLink(string name, string icon, Item link)
		{
			var desktoplinks = new ListString(Registry.GetString("/Current_User/Desktop/Links"), '|');
			var parameters = new ListString('^') {Databasename, link.ID.ToString(), name, icon};
		    desktoplinks.Add(parameters.ToString());
			Registry.SetString("/Current_User/Desktop/Links", desktoplinks.ToString());
            Sitecore.Context.ClientPage.ClientResponse.Alert(Translate.Text(ConfirmationMessage));
		}

		private Item AddItem(string name, string tid)
		{
            var database = Sitecore.Configuration.Factory.GetDatabase(Databasename);

            if (database == null)
            {
                return null;
            }

            var parent = database.GetItem(Parentid);
            if (parent == null)
            {
                return null;
            }

            var username = Sitecore.Context.User.Name.Replace('\\', '_');

		    var folder = (parent.Axes.GetChild(username));
            using (new SecurityDisabler())
		    {
                if (folder == null)		    		    
		        {
		            folder = ItemUtil.AddFromTemplate(username, "Common/Folder", parent) ?? parent;
		        }
		        return folder.Add(name, new TemplateID(new ID(tid)));
		    }
		}

		public override CommandState QueryState(CommandContext context)
		{
		    return Current.Context.ReportItem == null ? CommandState.Disabled : base.QueryState(context);
		}
	}
}


