using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Data.Items;
using Sitecore.Configuration;
using Sitecore.Web.UI.Pages;
using Sitecore.Text;
using Sitecore;
using System.Collections.Specialized;

namespace ASR.Controls
{
	class ItemSelectorDialog : DialogForm
	{
		// Fields
		protected Combobox Databases = null;
		protected DataContext DataContext = null;
		protected Listview ItemList = null;
		protected DataTreeview Treeview = null;

		protected override void OnLoad(EventArgs e)
		{
			if (!Sitecore.Context.ClientPage.IsEvent)
			{

				this.DataContext.GetFromQueryString();
                if (this.DataContext.Root.Length == 0)
                {
                    this.DataContext.Root = "{11111111-1111-1111-1111-111111111111}";                    
                }
                if (this.DataContext.Filter.Length == 0)
                {
                    this.DataContext.Filter = "@@virtual=0";
                }
				Item folder = this.DataContext.GetFolder();
				this.BuildDatabases((folder == null) ? string.Empty : folder.Database.Name);
			}
			base.OnLoad(e);
		}

		protected override void OnOK(object sender, EventArgs args)
		{
			Item item = this.Treeview.GetSelectionItem();
			if (item == null)
			{
				SheerResponse.Alert("Select an item.", new string[0]);
			}
			else
			{
				Sitecore.Context.ClientPage.ClientResponse.SetDialogValue(item.Paths.FullPath);
				base.OnOK(sender, args);
			}
		}

		/// <summary>
		/// Builds the databases.
		/// </summary>
		/// <param name="selectedName">Name of the selected.</param>
		private void BuildDatabases(string selectedName)
		{
			if (Sitecore.Context.User.IsAdministrator)
			{
				foreach (string str in Factory.GetDatabaseNames())
				{
					if (!Factory.GetDatabase(str).ReadOnly)
					{
						ListItem child = new ListItem();
						this.Databases.Controls.Add(child);
						child.ID = Control.GetUniqueID("ListItem");
						child.Header = str;
						child.Value = str;
						child.Selected = str == selectedName;
					}
				}
			}
			else
			{
				ListItem child = new ListItem();
				this.Databases.Controls.Add(child);
				child.ID = Control.GetUniqueID("ListItem");
				string databaseName = Sitecore.Context.ContentDatabase.Name;
				child.Header = databaseName;
				child.Value = databaseName;
				child.Selected = databaseName == selectedName;
			}
		}

		public void ChangeDatabase()
		{
			this.DataContext.Parameters = "databasename=" + this.Databases.Value;
			this.Treeview.RefreshRoot();
		}

        public static void Show()
        {
            Show(null);
        }
		public static void Show(NameValueCollection options)
		{
			UrlString url = new UrlString(UIUtil.GetUri("control:ItemSelectorDialog"));
            if (options != null)
            {
                
                foreach (var key in options.AllKeys)
                {
                    url.Parameters.Add(
                        key, System.Web.HttpUtility.UrlEncode(options[key]));
                       
                }                               
            }
			SheerResponse.ShowModalDialog(url.ToString(), true);
		}
	}
}
