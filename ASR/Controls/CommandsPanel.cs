using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace ASR.Controls
{
    class CommandsPanel : RibbonPanel
    {
        public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
        {
            if (Current.Context.ReportItem == null) return;
            foreach (var command in  Current.Context.ReportItem.Commands)
            {
                var click = string.Concat("ASRMainFormCommand:", command.Name);
                var id = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("action");
                RenderSmallButton(output,ribbon,id,Translate.Text(command.Title),command.Icon,string.Empty,click,true,false);
            }
        }
    }
}
