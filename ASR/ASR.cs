namespace ASR.App
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Text;

  using Controls;

  using DomainObjects;

  using Interface;

  using Sitecore;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Web;
  using Sitecore.Web.UI.HtmlControls;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Web.UI.WebControls;

  using Toolmenu=Controls.Toolmenu;

  public class MainForm : BaseForm
  {
    #region Variables

    protected ContextMenu cm;

    protected Section ConfigSection;

    protected ASRListview ItemList;

    protected ListviewHeader LViewHeader;

    protected GridPanel MainPanel;

    protected Toolbar MyToolbar;

    protected Literal Status;

    #endregion

    #region Public methods

    public override void HandleMessage(Message message)
    {
      if (message.Name == "ASR.MainForm:updateSection")
      {
        var ctl = message.Sender as Control;
        if (ctl != null)
        {
          Context.ClientPage.ClientResponse.Refresh(ctl);
        }
      }

      else if (message.Name == "event:click")
      {
        var nvc = message.Sender as NameValueCollection;
        if (nvc != null)
        {
          string eventname = nvc["__PARAMETERS"];
          if (!string.IsNullOrEmpty(eventname))
          {
            this.handleClickEvent(message, eventname);
          }
          else
          {
            base.HandleMessage(message);
          }
        }
        else
        {
          base.HandleMessage(message);
        }
      }
      else
      {
        base.HandleMessage(message);
      }
    }

    #endregion

    #region Protected methods

    protected override void OnLoad(EventArgs args)
    {
      base.OnLoad(args);
      if (!Context.ClientPage.IsEvent)
      {
        Current.ClearContext();
        this.LoadMenuItems();

        this.ItemList.View = "Details";
        this.ItemList.DblClick = "OnOpen";

          var queryString = WebUtil.GetQueryString();
          if(!string.IsNullOrEmpty(queryString))
          {
              openReport(queryString);
          }

      }
    }

    protected void OnOpen()
    {
      if (this.ItemList.GetSelectedItems().Length > 0)
      {
        var o = this.ItemList.GetSelectedItems()[0].Value;
        var uri = ItemUri.Parse(o);
        if (uri != null)
        {
          Util.OpenItem(uri);
        }
      }
    }

    [HandleMessage("ASR.MainForm:openlink")]
    protected void openReport(Message m)
    {
      openReport(m.Arguments);
    }

    #endregion

    #region Private methods

    private void changePage(string pageno)
    {
      int start = (int.Parse(pageno) - 1) * Current.Context.Settings.PageSize + 1;
      this.populateItemList(start, Current.Context.Settings.PageSize);
    }

    private void createActions()
    {
      this.purgeOldActions();

      var t = new Tooldivider { ID = Control.GetUniqueID("action") };
      this.MyToolbar.Controls.Add(t);
      foreach (CommandItem item in Current.Context.ReportItem.Commands)
      {
        var ctl = new ToolButton
          {
            Header = item.Name,
            Icon = item.Icon,
            Click = string.Concat("ASRMainFormcommand:", item.Name),
            ID = Control.GetUniqueID("action")
          };

        this.MyToolbar.Controls.Add(ctl);
      }

      Context.ClientPage.ClientResponse.Refresh(this.MyToolbar);
    }

    private void createParameters()
    {
      this.ConfigSection.Controls.Clear();
      foreach (var scanner in Current.Context.ReportItem.Scanners)
      {
        this.makeControls(scanner);
      }

      foreach (var refItem in Current.Context.ReportItem.Filters)
      {
        this.makeControls(refItem);
      }
      foreach (var viewer in Current.Context.ReportItem.Viewers)
      {
        this.makeControls(viewer);
      }
    }

    private void handleClickEvent(Message message, string eventname)
    {
      if (eventname.StartsWith("itemselector"))
      {
        string[] parts = eventname.Split(':');
        if (parts.Length == 2)
        {
          var iselector = Context.ClientPage.FindSubControl(parts[1]) as ItemSelector;
          if (iselector != null)
          {
            Context.ClientPage.Start(iselector, "Clicked");
          }
        }
      }
      else if (eventname.StartsWith("ASRMainFormcommand:"))
      {
        var commandname = eventname.Substring(eventname.IndexOf(':') + 1);
        var parameters = new NameValueCollection { { "name", commandname } };

        Context.ClientPage.Start(this, "runCommand", parameters);
      }
      else if (eventname.StartsWith("changepage"))
      {
        var pageno = eventname.Substring(eventname.IndexOf(':') + 1);
        this.changePage(pageno);
      }
      else
      {
        base.HandleMessage(message);
      }
    }

    private void LoadMenuItems()
    {
      this.MyToolbar.Controls.Clear();
      this.loadMenuItemsRec(Context.Item.Children["menu"], this.MyToolbar);
    }

    private void loadMenuItemsRec(Item root, System.Web.UI.Control ctl)
    {
      var menuitems = root.Children;
      foreach (Item mItem in menuitems)
      {
        Control child = null;
        switch (mItem.Template.Key)
        {
          case "toolmenu":
            //Toolmenu tm = new Toolmenu();
            var tm = new Toolmenu { ID = Control.GetUniqueID("T") };
            tm.LoadFromItem(mItem);
            this.loadMenuItemsRec(mItem, tm);
            child = tm;
            break;
          case "toolbar divider":
            var td = new Tooldivider();
            child = td;
            break;
          case "toolbutton":
            var tb = new ToolButton();
            //Toolbutton tb = new Toolbutton();     
            tb.LoadFromItem(mItem);
            child = tb;
            break;
          case "toolmenubutton":
            //Toolmenubutton tmb = new Toolmenubutton();
            var tmb = new ToolMenuButton();
            tmb.LoadFromItem(mItem);
            child = tmb;
            break;
          case "menu item":
            var mi = new MenuItem();
            mi.LoadFromItem(mItem);
            child = mi;
            break;
          default:
            break;
        }
        if (child != null)
        {
          ctl.Controls.Add(child);
        }
      }
    }

    private void makeControls(ReferenceItem referenceItem)
    {
      if (!referenceItem.HasParameters)
      {
        return;
      }
      var panel = new Panel();
      panel.Style.Add("border", "none");
      panel.Style.Add("margin-bottom", "10px");

      var literal = new Literal { Text = string.Format("<strong>{0}</strong><br/>", referenceItem.Name) };
      panel.ID =
        Control.GetUniqueID(
          string.Concat("params_", referenceItem.GetType().Name.ToLower(), "_", referenceItem.Name.ToLower(), "_"));
      panel.Controls.Add(literal);
      foreach (var pi in referenceItem.Parameters)
      {
        var i = new Inline();
        var l = new Label { Header = pi.Title + ":" };

        l.Style.Add("font-weight", "bold");
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
        var lit = new Literal { Text = "<br/>" };
        i.Controls.Add(lit);
        panel.Controls.Add(i);
      }
      this.ConfigSection.Controls.Add(panel);
    }

    private void openReport(string qs)
    {
      var rItem = ReportItem.CreateFromParameters(qs);
      this.StoreContext(rItem);
    }
    private void openReport(NameValueCollection nvc)
    {
      var rItem = ReportItem.CreateFromParameters(nvc);
      this.StoreContext(rItem);
    }
    private void StoreContext(ReportItem rItem)
    {
      if(rItem== null) return;
      Current.Context.ReportItem = rItem;
      Current.Context.Report = null;
      this.updateInterface(null);
    }

    //private void openReport(NameValueCollection nvc)
    //{
    //  var id = nvc["id"];
    //  if (string.IsNullOrEmpty(id))
    //  {
    //    return;
    //  }

    //  var director = new SCDirector("master", "en");
    //  if (!director.ObjectExists(id)) return;
    //  var rItem = director.GetObjectByIdentifier<ReportItem>(id);
    //  foreach (string key in nvc.Keys)
    //  {
    //    if (key.Contains("^"))
    //    {
    //      var item_parameter = key.Split('^');
    //      var g = new Guid(item_parameter[0]);

    //      var ri = rItem.FindItem(g);
    //      if (ri != null)
    //      {
    //        ri.SetAttributeValue(item_parameter[1], nvc[key]);
    //      }
    //    }
    //  }
    //  Current.Context.ReportItem = rItem;
    //  Current.Context.Report = null;
    //  this.updateInterface(null);                                   
    //}

    private void populateItemList(int start, int count)
    {
      this.ItemList.Controls.Clear();
      this.ItemList.ColumnNames.Clear();
      this.ItemList.ColumnNames.Add("Icon", "Icon");

      var columnNames = new HashSet<string>();

      foreach (DisplayElement result in Current.Context.Report.GetResultElements(start - 1, count))
      {
        var lvi = new ListviewItem { ID = Control.GetUniqueID("lvi"), Icon = result.Icon, Value = result.Value };
        foreach (var column in result.GetColumnNames())
        {
          columnNames.Add(column);
          lvi.ColumnValues.Add(column, result.GetColumnValue(column));
        }
        this.ItemList.Controls.Add(lvi);
      }
      foreach (var column in columnNames)
      {
        this.ItemList.ColumnNames.Add(column, column);
      }

      this.Status.Text = string.Format("{0} results found.", Current.Context.Report.ResultsCount());

      var noPages =
        (int)Math.Ceiling((decimal)Current.Context.Report.ResultsCount() / Current.Context.Settings.PageSize);
      this.ItemList.CurrentPage = (int)Math.Ceiling((decimal)start / Current.Context.Settings.PageSize);

      var startpage = noPages > Current.Context.Settings.MaxNumberPages &&
                      this.ItemList.CurrentPage > Current.Context.Settings.MaxNumberPages / 2
                        ? this.ItemList.CurrentPage - Current.Context.Settings.MaxNumberPages / 2
                        : 1;
      var endpage = Math.Min(startpage + Current.Context.Settings.MaxNumberPages, noPages);
      if (noPages > 0)
      {
        var sb = new StringBuilder("&nbsp;&nbsp; Page ");
        if (startpage > 1)
        {
          var newpage = Math.Max(1, startpage - Current.Context.Settings.MaxNumberPages);
          if (newpage > 1)
          {
            var b = new LinkButton { Header = "first", Click = "changepage:" + 1 };
            sb.Append(b.RenderAsText());
          }
          var lb = new LinkButton { Header = "...", Click = "changepage:" + newpage };
          sb.Append(lb.RenderAsText());
        }
        for (var i = startpage; i <= endpage; i++)
        {
          var b = new LinkButton
            { Header = i.ToString(), Selected = i == this.ItemList.CurrentPage, Click = "changepage:" + i };
          sb.Append(b.RenderAsText());
        }
        if (endpage < noPages)
        {
          var newpage = Math.Min(noPages, endpage + Current.Context.Settings.MaxNumberPages / 2);
          var b = new LinkButton { Header = "...", Click = "changepage:" + newpage };
          sb.Append(b.RenderAsText());
          if (newpage < noPages)
          {
            b = new LinkButton { Header = "last", Click = "changepage:" + noPages };
            sb.Append(b.RenderAsText());
          }
        }
        this.Status.Text += sb.ToString();
      }

      Context.ClientPage.ClientResponse.Refresh(this.ItemList);

      Context.ClientPage.ClientResponse.Refresh(this.Status);
    }

    private void purgeOldActions()
    {
      var cc = new List<System.Web.UI.Control>();
      foreach (System.Web.UI.Control ctl in this.MyToolbar.Controls)
      {
        if (ctl.ID != null && ctl.ID.StartsWith("action"))
        {
          cc.Add(ctl);
        }
      }
      foreach (System.Web.UI.Control ctl in cc)
      {
        this.MyToolbar.Controls.Remove(ctl);
      }
    }

    private void runCommand(ClientPipelineArgs args)
    {
      string commandname = args.Parameters["name"];
      var sl = new StringList();
      foreach (ListviewItem item in this.ItemList.SelectedItems)
      {
        sl.Add(item.Value);
      }
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
    private void runFinished(Message message)
    {
      this.populateItemList(1, Current.Context.Settings.PageSize);
      SheerResponse.Refresh(this.MyToolbar);
    }

    [HandleMessage("ASR.MainForm:update", false)]
    private void updateInterface(Message message)
    {
      this.ItemList.ColumnNames.Clear();
      this.ItemList.Controls.Clear();
      this.Status.Text = "";
      this.createParameters();
      this.createActions();
      this.ConfigSection.Header = string.Concat("Configure report - ", Current.Context.ReportItem.Name);
      Context.ClientPage.ClientResponse.Refresh(this.MainPanel);
      Context.ClientPage.ClientResponse.Refresh(this.Status);
    }

    [HandleMessage("ASR.MainForm:updateparameters", false)]
    [HandleMessage("asr:linkit")]
    private void updateParameters(Message message)
    {
      foreach (Control ctl in this.ConfigSection.Controls)
      {
        if (ctl.ID != null && ctl.ID.StartsWith("params_"))
        {
          string[] splitid = ctl.ID.Split('_');

          foreach (Control subCtl in ctl.Controls)
          {
            if (subCtl.ID != null && subCtl.ID.StartsWith("params_"))
            {
              string tid = subCtl.ID.Substring(7);
              int indexof = tid.IndexOf('_');
              tid = tid.Substring(0, indexof);
              var input = subCtl.FindControl(subCtl.Value) as Control;
              if (input != null)
              {
                if (splitid[1].StartsWith("scanner"))
                {
                  Current.Context.ReportItem.Scanners.First(s => s.Name.ToLower() == splitid[2]).SetAttributeValue(
                    tid, input.Value);
                }
                else if (splitid[1].StartsWith("viewer"))
                {
                  Current.Context.ReportItem.Viewers.First(v => v.Name.ToLower() == splitid[2]).SetAttributeValue(
                    tid, input.Value);
                }
                else
                {
                  Current.Context.ReportItem.Filters.First(f => f.Name.ToLower() == splitid[2]).SetAttributeValue(
                    tid, input.Value);
                }
              }
            }
          }
        }
      }
      if (message.Name == "asr:linkit")
      {
        Context.ClientPage.SendMessage(this, "asr:createlink");
      }
    }

    [HandleMessage("ASR.MainForm:toolbarupdate", false)]
    private void updateToolbar(ClientPipelineArgs args)
    {
      this.LoadMenuItems();
      Context.ClientPage.ClientResponse.Refresh(this.MyToolbar);
    }

    #endregion
  }
}