using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore;
using Sitecore.Diagnostics;

namespace ASR.Controls
{
    public class DateTimePicker : Sitecore.Web.UI.HtmlControls.DateTimePicker
    {
        public string Format
        {
            get { return base.GetViewStateString("Value.Format"); }
            set
            {
                base.SetViewStateString("Value.Format", value);
            }
        }

        public DateTimePicker():base()
        {
            Format = "MM/dd/yyyy";
        }

        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            output.Write("<div style='display:inline;float:left'>");
            base.DoRender(output);
            output.Write("</div>");

        }
        public override string Value
        {
            get
            {
                var value = base.Value;
                if(DateUtil.IsIsoDate(value))
                {                    
                    return DateUtil.IsoDateToDateTime(value).ToString(Format);
                }
                return value;
            }
            set
            {                
               base.Value = Util.MakeDateReplacements(value);
            }
        }
    }
}
