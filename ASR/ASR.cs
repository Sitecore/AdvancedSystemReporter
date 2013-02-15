using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using ASR.Controls;
using ASR.DomainObjects;
using ASR.Interface;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Control = Sitecore.Web.UI.HtmlControls.Control;

namespace ASR.App
{
    public class MainForm : BaseForm, IHasCommandContext
    {
        #region Variables

        protected ContextMenu Cm;

        protected Section ConfigSection;
        protected Literal Description;

        protected Section InformationSection;

        protected ASRListview ItemList;

        protected ListviewHeader LViewHeader;

        protected GridPanel MainPanel;

        protected RibbonContainer MyRibbon;

        protected Literal Status;
        protected Border RibbonBorder;

        #endregion

        #region Public methods

        public CommandContext GetCommandContext()
        {                        
            var itemNotNull = Client.CoreDatabase.GetItem("{EE1860D8-09CB-49FE-9AB5-2F01D2D2D796}"); // /sitecore/content/Applications/Advanced System Reporter/Ribbon            
            var context = new CommandContext {RibbonSourceUri = itemNotNull.Uri};

            return context;
        }

        public override void HandleMessage(Message message)
        {
            if (message.Name == "ASR.MainForm:updateSection")
            {
                var ctl = message.Sender as Sitecore.Web.UI.HtmlControls.Control;
                if (ctl != null)
                {
                    Sitecore.Context.ClientPage.ClientResponse.Refresh(ctl);
                }
                return;
            }
            if (message.Name.StartsWith("ASRMainFormCommand:"))
            {
                string commandname = message.Name.Substring(message.Name.IndexOf(':') + 1);
                var parameters = new NameValueCollection { { "name", commandname } };
                Sitecore.Context.ClientPage.Start(this, "RunCommand", parameters);
                return;
            }
            if (message.Name == "event:click")
            {
                var nvc = message.Sender as NameValueCollection;
                if (nvc != null)
                {
                    string eventname = nvc["__PARAMETERS"];
                    if (!string.IsNullOrEmpty(eventname))
                    {
                        HandleClickEvent(message, eventname);
                        return;
                    }
                }
            }
                        
            base.HandleMessage(message);            
        }

        #endregion

        #region Protected methods

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            MyRibbon.CommandContext = this;
            if (Sitecore.Context.ClientPage.IsEvent) return;

            Current.ClearContext();               

            ItemList.View = "Details";
            ItemList.DblClick = "OnOpen";

            string queryString = WebUtil.GetQueryString();
            if (!string.IsNullOrEmpty(queryString))
            {
                OpenReport(queryString);
            }
        }

        protected void OnOpen()
        {
            if (ItemList.GetSelectedItems().Length <= 0) return;
            var o = ItemList.GetSelectedItems()[0].Value;
            var uri = ItemUri.Parse(o);
            if (uri != null)
            {
                Util.OpenItem(uri);
            }
        }

        [HandleMessage("ASR.MainForm:openlink")]
        protected void OpenReport(Message m)
        {
            OpenReport(m.Arguments);
        }

        #endregion

        #region Private methods

        private void ChangePage(string pageno)
        {
            var start = (int.Parse(pageno) - 1)*Current.Context.Settings.PageSize + 1;
            PopulateItemList(start, Current.Context.Settings.PageSize);
        }     

        private void CreateParameters()
        {
            ConfigSection.Controls.Clear();
            var configsectionvisible = Current.Context.ReportItem.Scanners.Aggregate(false, (current, scanner) => current | MakeControls(scanner));
            configsectionvisible = Current.Context.ReportItem.Filters.Aggregate(configsectionvisible, (current, refItem) => current | MakeControls(refItem));
            configsectionvisible = Current.Context.ReportItem.Viewers.Aggregate(configsectionvisible, (current, viewer) => current | MakeControls(viewer));
            ConfigSection.Visible = configsectionvisible;
        }

        private void HandleClickEvent(Message message, string eventname)
        {
            if (eventname.StartsWith("itemselector"))
            {
                var parts = eventname.Split(':');
                if (parts.Length == 2)
                {
                    var iselector = Sitecore.Context.ClientPage.FindSubControl(parts[1]) as ItemSelector;
                    if (iselector != null)
                    {
                        Sitecore.Context.ClientPage.Start(iselector, "Clicked");
                    }
                }
            }
           
            else if (eventname.StartsWith("changepage"))
            {
                var pageno = eventname.Substring(eventname.IndexOf(':') + 1);
                ChangePage(pageno);
            }
            else
            {
                base.HandleMessage(message);
            }
        }      

        private bool MakeControls(ReferenceItem referenceItem)
        {
            if (!referenceItem.HasParameters)
            {
                return false;                
            }
           
            var panel = new Panel();
            panel.Style.Add("border", "none");
            panel.Style.Add("margin-bottom", "10px");

            var literal = new Literal
                {
                    Text =
                        string.Format("<div style=\"margin-left:10px;margin-top:4px;font-weight:bold\">{0}</div><br/>",
                                      referenceItem.PrettyName)
                };
            panel.ID =
                Control.GetUniqueID(
                    string.Concat("params_", referenceItem.GetType().Name.ToLower(), "_", referenceItem.Name.ToLower(),
                                  "_"));
            panel.Controls.Add(literal);
            foreach (var pi in referenceItem.Parameters)
            {
                var i = new Inline();
                var l = new Label {Header = pi.Title + ":"};

                l.Style.Add("font-weight", "bold");
                l.Style.Add("padding-top", "5px");
                l.Style.Add("margin-right", "10px");
                l.Style.Add("margin-left", "20px");
                l.Style.Add("width", "100px");
                l.Style.Add("text-align", "right");
                l.Style.Add("float", "left");

                var input = pi.MakeControl();
                l.For = input.ID;

                i.Style.Add("display", "block");
                i.Style.Add("margin-top", "5px");
                i.Value = input.ID;
                i.ID = Control.GetUniqueID("params_" + pi.Name + "_");
                i.Controls.Add(l);
                i.Controls.Add(input);
                var lit = new Literal {Text = "<br/>"};
                i.Controls.Add(lit);
                panel.Controls.Add(i);
            }
            ConfigSection.Controls.Add(panel);
            return true;
        }

        private void OpenReport(string qs)
        {
            var rItem = ReportItem.CreateFromParameters(qs);
            StoreContext(rItem);
        }

        private void OpenReport(NameValueCollection nvc)
        {
            var rItem = ReportItem.CreateFromParameters(nvc);
            StoreContext(rItem);
        }

        private void StoreContext(ReportItem rItem)
        {
            if (rItem == null) return;
            Current.Context.ReportItem = rItem;
            Current.Context.Report = null;
            UpdateInterface(null);
        }
     
        private void PopulateItemList(int start, int count)
        {
            ItemList.Controls.Clear();
            ItemList.ColumnNames.Clear();
            ItemList.ColumnNames.Add("Icon", "Icon");

            var columnNames = new HashSet<string>();

            foreach (DisplayElement result in Current.Context.Report.GetResultElements(start - 1, count))
            {
                var lvi = new ListviewItem
                    {
                        ID = Control.GetUniqueID("lvi"),
                        Icon = result.Icon,
                        Value = result.Value
                    };
                foreach (string column in result.GetColumnNames())
                {
                    columnNames.Add(column);
                    lvi.ColumnValues.Add(column, result.GetColumnValue(column));
                }
                ItemList.Controls.Add(lvi);
            }
            foreach (string column in columnNames)
            {
                ItemList.ColumnNames.Add(column, column);
            }

            Status.Text = string.Format("{0} results found.", Current.Context.Report.ResultsCount());            

            var noPages =
                (int) Math.Ceiling((decimal) Current.Context.Report.ResultsCount()/Current.Context.Settings.PageSize);
            ItemList.CurrentPage = (int) Math.Ceiling((decimal) start/Current.Context.Settings.PageSize);

            int startpage = noPages > Current.Context.Settings.MaxNumberPages &&
                            ItemList.CurrentPage > Current.Context.Settings.MaxNumberPages/2
                                ? ItemList.CurrentPage - Current.Context.Settings.MaxNumberPages/2
                                : 1;
            int endpage = Math.Min(startpage + Current.Context.Settings.MaxNumberPages, noPages);
            const string separator = "&nbsp;&nbsp;";
            if (noPages > 0)
            {
                var sb = new StringBuilder("&nbsp;&nbsp; Page ");
                if (startpage > 1)
                {
                    int newpage = Math.Max(1, startpage - Current.Context.Settings.MaxNumberPages);
                    if (newpage > 1)
                    {
                        var b = new LinkButton {Header = "first", Click = "changepage:" + 1};
                        sb.Append(b.RenderAsText());
                    }
                    var lb = new LinkButton {Header = "...", Click = "changepage:" + newpage};
                    sb.Append(lb.RenderAsText());
                }
                for (int i = startpage; i <= endpage; i++)
                {
                    var b = new LinkButton
                        {Header = i.ToString(CultureInfo.InvariantCulture), Selected = i == ItemList.CurrentPage, Click = "changepage:" + i};
                    b.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                    sb.Append(b.RenderAsText());
                    sb.Append(separator);
                }
                if (endpage < noPages)
                {
                    int newpage = Math.Min(noPages, endpage + Current.Context.Settings.MaxNumberPages/2);
                    var b = new LinkButton {Header = "...", Click = "changepage:" + newpage};
                    sb.Append(b.RenderAsText());
                    if (newpage < noPages)
                    {
                        b = new LinkButton {Header = "last", Click = "changepage:" + noPages};
                        sb.Append(b.RenderAsText());
                    }
                }
                Status.Text += sb.ToString();
                Status.Style.Add(HtmlTextWriterStyle.Cursor, "default");
            }

            Sitecore.Context.ClientPage.ClientResponse.Refresh(ItemList);

            Sitecore.Context.ClientPage.ClientResponse.Refresh(Status);
        }       

        private void RunCommand(ClientPipelineArgs args)
        {
            string commandname = args.Parameters["name"];
            var sl = new StringList();
            sl.AddRange(ItemList.SelectedItems.Select(item => item.Value));
            if (sl.Count > 0)
            {
                Current.Context.ReportItem.RunCommand(commandname, sl);
            }
            else
            {
                SheerResponse.Alert("You need to select at least one item");
            }
        }

        [HandleMessage("MainForm:runfinished", false)]
        private void RunFinished(Message message)
        {
            PopulateItemList(1, Current.Context.Settings.PageSize);            
            SheerResponse.Refresh(RibbonBorder);
        }

        [HandleMessage("ASR.MainForm:update", false)]
        private void UpdateInterface(Message message)
        {
            ItemList.ColumnNames.Clear();
            ItemList.Controls.Clear();
            if (Current.Context.ReportItem == null)
            {
                Status.Text = "Open a report first to get started";
                ConfigSection.Visible = false;
                InformationSection.Visible = false;
                return;
            }
            else
            {
                InformationSection.Visible = true;
                Status.Text = "";
            }

            
            Status.Text = "";
            CreateParameters();
            
            ConfigSection.Header = string.Concat("Configure report - ", Current.Context.ReportItem.Name);


            Description.Text = StringUtil.GetString((object)Current.Context.ReportItem.Description, Translate.Text("no description"));
            InformationSection.Collapsed = true;

         
            Sitecore.Context.ClientPage.ClientResponse.Refresh(MainPanel);
            Sitecore.Context.ClientPage.ClientResponse.Refresh(Status);
        }

        [HandleMessage("ASR.MainForm:updateparameters", false)]
        [HandleMessage("asr:linkit")]
        private void UpdateParameters(Message message)
        {
            foreach (Sitecore.Web.UI.HtmlControls.Control ctl in ConfigSection.Controls)
            {
                if (ctl.ID != null && ctl.ID.StartsWith("params_"))
                {
                    string[] splitid = ctl.ID.Split('_');

                    foreach (Sitecore.Web.UI.HtmlControls.Control subCtl in ctl.Controls)
                    {
                        if (subCtl.ID != null && subCtl.ID.StartsWith("params_"))
                        {
                            string tid = subCtl.ID.Substring(7);
                            int indexof = tid.IndexOf('_');
                            tid = tid.Substring(0, indexof);
                            var input = subCtl.FindControl(subCtl.Value) as Sitecore.Web.UI.HtmlControls.Control;
                            if (input != null)
                            {
                                if (splitid[1].StartsWith("scanner"))
                                {
                                    Current.Context.ReportItem.Scanners.First(s => s.Name.ToLower() == splitid[2])
                                           .SetAttributeValue(
                                               tid, input.Value);
                                }
                                else if (splitid[1].StartsWith("viewer"))
                                {
                                    Current.Context.ReportItem.Viewers.First(v => v.Name.ToLower() == splitid[2])
                                           .SetAttributeValue(
                                               tid, input.Value);
                                }
                                else
                                {
                                    Current.Context.ReportItem.Filters.First(f => f.Name.ToLower() == splitid[2])
                                           .SetAttributeValue(
                                               tid, input.Value);
                                }
                            }
                        }
                    }
                }
            }
            if (message.Name == "asr:linkit")
            {
                Sitecore.Context.ClientPage.SendMessage(this, "asr:createlink");
            }
        }       

        #endregion
    }
}