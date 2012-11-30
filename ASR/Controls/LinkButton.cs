using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Controls
{
    public class LinkButton: Sitecore.Web.UI.HtmlControls.Button
    {
        public LinkButton()
        {
            
            Style.Add("vertical-align", "bottom");
        }
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            int width = 10 + (Header.Length - 1) * 5;
            
                    Style.Add("width", width.ToString()+"px");
            
            
            if (Selected)
            {                
                HeaderStyle = "font-weight:bold";
            }
            else
            {
                Style.Add("cursor", "hand");
            }

            output.Write("<a " + base.ControlAttributes + ">" + base.GetFormattedHeader());
            this.RenderChildren(output);
            output.Write("</a>");
        }


        public bool Selected
        {
            get
            {
               return  base.GetViewStateBool("selected");
            }
            set
            {
                base.SetViewStateBool("selected",value);
            }
        }
      
    }
}
