using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Controls
{
    public class ToolMenuButton:Sitecore.Web.UI.HtmlControls.Toolbutton
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            base.Icon = this.Icon;
            base.DoRender(output);
        }
        public override void LoadFromItem(Sitecore.Data.Items.Item item)
        {
            base.LoadFromItem(item);
            this.Icon = base.Icon;
        }
        //public void LoadFromItem(Sitecore.Data.Items.Item item)
        //{
        //    loadFromitem(item);
        //    this.Icon = base.Icon;
        //}
        private void loadFromitem(Sitecore.Data.Items.Item item)
        {
            if (item != null)
            {
                base.Icon = item["Icon"];
                this.Header = item["Header"];
                if (item["ID"].Length > 0)
                {
                    this.ID = item["ID"];
                }
                this.Action = item["Action"];
                base.Click = (item["Click"].Length > 0) ? item["Click"] : base.Click;
                this.ContextMenu = (item["Context menu"].Length > 0) ? item["Context menu"] : this.ContextMenu;
                this.Disabled = item["Disabled"] == "1";
                this.Visible = item["Hidden"] != "1";
                this.ToolTip = item["Tool tip"];
                this.DataSource = item["Datasource"];
                base.KeyCode = item["KeyCode"];
                if (this.DataSource.Length == 0)
                {
                    this.DataSource = item.Paths.LongID;
                }
            }

        }
        public new string Icon
        {
            get
            {
                return base.GetViewStateString("icon");
            }
            set
            {
                if (value != Icon)
                {
                    base.SetViewStateString("icon",value);
                }
            }
        }
    }
}
