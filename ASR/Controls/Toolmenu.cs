using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Controls
{
    public class Toolmenu: Sitecore.Web.UI.HtmlControls.Toolmenu
    {
        //public Toolmenu()
        //{

        //}

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
                    base.SetViewStateString("icon", value);
                }
            }
        }
    }
}
