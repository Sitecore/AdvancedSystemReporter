using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace ASR.Controls
{
    public class RibbonContainer : System.Web.UI.WebControls.WebControl, IHasCommandContext
    {
        public IHasCommandContext CommandContext { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.HasControls())
            {
                Controls.Add(new Ribbon() { ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("Ribbon")});
            }


        }

        public CommandContext GetCommandContext()
        {
            return CommandContext.GetCommandContext();
        }

    }
}
