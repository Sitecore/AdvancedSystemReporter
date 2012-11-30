using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.HtmlControls;

namespace ASR.Controls
{
    public class ASRComboEdit : Listbox
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {

            string select = "<select" /* name=\"lstDropDown_A\" id=\"lstDropDown_A\"*/+ " onKeyDown=\"fnKeyDownHandler(this, event);\" onKeyUp=\"fnKeyUpHandler_A(this, event); return false;\" onKeyPress = \"return fnKeyPressHandler_A(this, event);\"  onChange=\"fnChangeHandler_A(this, event);\"";
            base.SetWidthAndHeightStyle();
            output.Write(string.Concat(select,base.ControlAttributes,">"));
            this.RenderChildren(output);
            output.Write("</select>");
        }
    }
}
