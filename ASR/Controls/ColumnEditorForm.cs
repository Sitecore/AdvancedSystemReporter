using System;
using System.IO;
using ASR.DomainObjects;
using ASR.Interface;
using Sitecore.Data;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web;
using System.Xml;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Resources;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.Sheer;
using Sitecore;
using System.Collections;

namespace ASR.Controls
{
	class ColumnEditorForm : DialogForm
	{
		protected ASRComboEdit ColumnName;
        
		protected Edit ColumnHeader;
		protected Scrollbox Columns;

		/// <summary>
		/// Gets or sets the index of the selected column.
		/// </summary>
		/// <value>The index of the selected column.</value>
		public int SelectedIndex
		{
		    get { return MainUtil.GetInt(Sitecore.Context.ClientPage.ServerProperties["SelectedIndex"], -1); }
		    set
			{
				Sitecore.Context.ClientPage.ServerProperties["SelectedIndex"] = value;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Sitecore.Context.ClientPage.IsEvent)
			{
			    PopulateAvailableColumns();

			    Refresh();
				SelectedIndex = -1;
                ColumnHeader.Value =
                    Sitecore.Context.Request.QueryString["itemtype"] ?? "<no name>";
			}
		}

	    private void PopulateAvailableColumns()
	    {
	        var handle = UrlHandle.Get();

	        var itemUri = ItemUri.Parse(handle["id"]);

	        var item = Sitecore.Data.Database.GetItem(itemUri);
	        if (item == null) return;
	        
            try
            {

                var referenceItem = new ReferenceItem(item);
                var viewer = BaseViewer.Create(referenceItem.FullType, string.Empty);
                if (viewer == null) return;

                
                foreach (var availableColumn in viewer.AvailableColumns)
                {
                    ColumnName.Controls.Add(new ListItem { Header = availableColumn, Value = availableColumn.ToLowerInvariant()});
                }
            }
            catch (FileNotFoundException)
            {

                //todo
                
            }
           
	        
	    }


	    protected override void OnOK(object sender, EventArgs args)
		{
			Sitecore.Context.ClientPage.ClientResponse.SetDialogValue("yes");
			base.OnOK(sender, args);
		}

		private void Refresh()
		{
			Columns.Controls.Clear();
			Controls = new ArrayList();
			RenderColumns();
			SheerResponse.SetOuterHtml("Columns", Columns);
		}

       
		private void RenderColumns()
		{
			//LayoutDefinition.Parse(WebUtil.GetSessionString("SC_DEVICEEDITOR"));
			string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
			if (!String.IsNullOrEmpty(xml))
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				XmlNodeList columnNodes = doc.DocumentElement.SelectNodes("Column");
				for (int i = 0; i < columnNodes.Count; i++)
				{
					XmlControl webControl = Resource.GetWebControl("Column") as XmlControl;
					Assert.IsNotNull(webControl, typeof(XmlControl));
					Columns.Controls.Add(webControl);
					string uniqueID = Control.GetUniqueID("C");
					webControl["Click"] = "OnColumnClick(\"" + i + "\")";
					webControl["DblClick"] = "device:edit";
					if (i == SelectedIndex)
					{
						webControl["Background"] = "#EDB078";
					}
					webControl["ID"] = uniqueID;
					webControl["Header"] = columnNodes[i].InnerText;
					webControl["Name"] = columnNodes[i].Attributes["name"].Value;

					Controls.Add(uniqueID);
				}
			}
		}

		protected void OnColumnClick(string index)
		{
			Assert.ArgumentNotNull(index, "index");
			//deselect current
			if (SelectedIndex >= 0)
			{
				SheerResponse.SetStyle(StringUtil.GetString(Controls[SelectedIndex]), "background", string.Empty);
			}
			//select current
			SelectedIndex = MainUtil.GetInt(index, -1);
			if (SelectedIndex >= 0)
			{
				SheerResponse.SetStyle(StringUtil.GetString(Controls[SelectedIndex]), "background", "#EDB078");
			}
		}

		public ArrayList Controls
		{
			get
			{
				return (ArrayList)Sitecore.Context.ClientPage.ServerProperties["Controls"];
			}
			set
			{
				Assert.ArgumentNotNull(value, "value");
				Sitecore.Context.ClientPage.ServerProperties["Controls"] = value;
			}
		}

		protected void MoveUp()
		{
			if (SelectedIndex > 0)
			{
				string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
				if (!String.IsNullOrEmpty(xml))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xml);

					XmlNode columnNode = doc.DocumentElement.ChildNodes[SelectedIndex];
					XmlNode previousSibling = columnNode.PreviousSibling;
					doc.DocumentElement.RemoveChild(columnNode);
					doc.DocumentElement.InsertBefore(columnNode, previousSibling);

					SelectedIndex--;

					WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
					Refresh();
				}
			}
		}

		protected void MoveDown()
		{
			if (SelectedIndex < (Controls.Count - 1))
			{
				string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
				if (!String.IsNullOrEmpty(xml))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xml);

					XmlNode columnNode = doc.DocumentElement.ChildNodes[SelectedIndex];
					XmlNode nextSibling = columnNode.NextSibling;
					doc.DocumentElement.RemoveChild(columnNode);
					doc.DocumentElement.InsertAfter(columnNode, nextSibling);

					SelectedIndex++;

					WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
					Refresh();
				}
			}
		}

		protected void Remove()
		{
			if (SelectedIndex > -1)
			{
				string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
				if (!String.IsNullOrEmpty(xml))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xml);

					XmlNode columnNode = doc.DocumentElement.ChildNodes[SelectedIndex];
					doc.DocumentElement.RemoveChild(columnNode);

					SelectedIndex = -1;

					WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
					Refresh();
				}
			}
		}

		protected void Add()
		{
			string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
			if (!String.IsNullOrEmpty(xml))
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);

				XmlElement columnNode = doc.CreateElement("Column");
				XmlAttribute nameAttribute = doc.CreateAttribute("name");
				nameAttribute.Value = ColumnName.Value;
				columnNode.Attributes.Append(nameAttribute);
				columnNode.InnerText = ColumnHeader.Value;
				doc.DocumentElement.AppendChild(columnNode);

                
                ColumnHeader.Value = "<no name>";

				WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
				Refresh();
			}
		}
	}
}
