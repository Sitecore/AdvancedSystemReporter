using System;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI;
using System.Web.UI.WebControls;
using Sitecore;
using Sitecore.Xml;
using System.Xml;
using Sitecore.Web.UI.Sheer;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Resources;
using Sitecore.Web.UI.HtmlControls;
using Version = Sitecore.Data.Version;

namespace ASR.Controls
{
	public class ColumnsField : Sitecore.Web.UI.HtmlControls.Control, IContentField, IMessageHandler
	{
        #region properties

        public string ItemID
        {
            get { return StringUtil.GetString(ViewState["ItemID"]); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                ViewState["ItemID"] = value;
            }
        }
        public string ItemVersion
        {
            get { return StringUtil.GetString(ViewState["ItemVersion"]); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                ViewState["ItemVersion"] = value;
            }
        }
        public string ItemLanguage
        {
            get { return StringUtil.GetString(ViewState["ItemLanguage"]); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                ViewState["ItemLanguage"] = value;
            }
        }
        public override string Value
        {
            get
            {
                return StringUtil.GetString(ViewState["Value"]);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                ViewState["Value"] = value;
            }
        } 
        #endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Sitecore.Context.ClientPage.IsEvent)
			{
				Refresh();
			}
		}

		private void BuildControl(Control parent)
		{
			GridPanel columnsGrid = new GridPanel();
			parent.Controls.Add(columnsGrid);
			columnsGrid.RenderAs = RenderAs.Literal;
			columnsGrid.Width = Unit.Parse("100%");
			columnsGrid.Attributes["CellSpacing"] = "2";
			columnsGrid.Attributes["id"] = ID;

			if (!string.IsNullOrEmpty(Value))
			{
				XmlDocument xml = XmlUtil.LoadXml(Value);
				if (xml.OuterXml.Length > 0)
				{
					XmlNodeList columnNodes = xml.DocumentElement.SelectNodes("Column");
					for (int i = 0; i < columnNodes.Count; i++)
					{
						XmlControl webControl = Resource.GetWebControl("Column") as XmlControl;
						Assert.IsNotNull(webControl, typeof(XmlControl));
						Controls.Add(webControl);
						string uniqueID = Control.GetUniqueID("C");
						webControl["ID"] = uniqueID;
						webControl["Header"] = columnNodes[i].InnerText;
						webControl["Name"] = columnNodes[i].Attributes["name"].Value;

						columnsGrid.Controls.Add(webControl);
					}
				}
			}
		}

		#region IContentField Members

		public string GetValue()
		{
			return GetDocument().OuterXml;
		}

		public void SetValue(string value)
		{
			Assert.ArgumentNotNull(value, "value");
			base.ServerProperties["Value"] = value;
		}
		#endregion

		#region IMessageHandler Members

		void IMessageHandler.HandleMessage(Message message)
		{
			Assert.ArgumentNotNull(message, "message");

			if (message["id"] == ID && message.Name == "columns:edit")
			{
				Sitecore.Context.ClientPage.Start(this, "Edit");
			}
		}
		#endregion

		private void Refresh()
		{
			Controls.Clear();
			BuildControl(this);
			if (Sitecore.Context.ClientPage.IsEvent)
			{
				Sitecore.Context.ClientPage.ClientResponse.SetOuterHtml(ID, this);
			}
		}

		protected void Edit(ClientPipelineArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			if (Enabled)
			{
				if (args.IsPostBack)
				{
					if (((args.Result != null) && (args.Result.Length > 0)) && (args.Result != "undefined"))
					{
						XmlDocument doc = XmlUtil.LoadXml(WebUtil.GetSessionString("ASR_COLUMNEDITOR"));
						WebUtil.SetSessionValue("ASR_COLUMNEDITOR", null);

						Value = doc.OuterXml;
						SetModified();
						Refresh();
					}
				}
				else
				{
					XmlDocument document = GetDocument();
					WebUtil.SetSessionValue("ASR_COLUMNEDITOR", document.OuterXml);
					UrlString urlString = new UrlString(UIUtil.GetUri("control:ColumnEditor"));
					UrlHandle handle = new UrlHandle();
					string value = Value;
					if (value == "__#!$No value$!#__")
					{
						value = string.Empty;
					}                    
					handle["value"] = value;
				    handle["id"] =
				        new ItemUri(Sitecore.Data.ID.Parse(ItemID), Language.Parse(ItemLanguage), new Version(ItemVersion),
				                    Sitecore.Context.ContentDatabase).ToString(ItemUriFormat.Uri);
					handle.Add(urlString);                    
                    SheerResponse.ShowModalDialog(urlString.ToString(), "800px", "500px", string.Empty, true);
					args.WaitForPostBack();
				}
			}
		}

		private void SetModified()
		{
			Sitecore.Context.ClientPage.Modified = true;
		}

		private XmlDocument GetDocument()
		{
			XmlDocument document = new XmlDocument();
			string xml = StringUtil.GetString(base.ServerProperties["Value"]);
			if (xml.Length > 0)
			{
				document.LoadXml(xml);
				return document;
			}
			document.LoadXml("<Columns/>");
			return document;
		}
	}
}
