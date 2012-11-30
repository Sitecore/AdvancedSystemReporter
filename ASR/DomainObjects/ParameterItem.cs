using CorePoint.DomainObjects;
using CorePoint.DomainObjects.SC;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using Sitecore.Web.UI.HtmlControls;

namespace ASR.DomainObjects
{
	[Template("System/ASR/Parameter")]
	public class ParameterItem : StandardTemplate
	{
		[Field("title")]
		public string Title
		{
			get;
			set;
		}

		[Field("type")]
		public string Type
		{
			get;
			set;
		}

		[Field("default value")]
		private string DefaultValue { get; set; }

		public string Value
		{
			get
			{
				string _replacedValue = DefaultValue;
				if (!string.IsNullOrEmpty(_replacedValue))
				{
					_replacedValue = _replacedValue.Replace("$sc_lastyear", DateTime.Today.AddYears(-1).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_lastweek", DateTime.Today.AddDays(-7).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_lastmonth", DateTime.Today.AddMonths(-1).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_yesterday", DateTime.Today.AddDays(-1).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_today", DateTime.Today.ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_now", DateTime.Now.ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_currentuser", Sitecore.Context.User == null ? string.Empty : Sitecore.Context.User.Name);
				}
				return _replacedValue;
			}
			set
			{
				DefaultValue = value;
			}
		}

        [Field("parameters")]
        public string _parameters { get; set; }

        private NameValueCollection _params;
        public NameValueCollection Parameters
        {
            get
            {
                if (_params == null)
                {
                    //_params = Sitecore.StringUtil.ParseNameValueCollection(_parameters, '|', '=');
                    _params = new NameValueCollection();
                    string[] substrings = _parameters.Split('|');
                    foreach (var st in substrings)
                    {
                        int i = st.IndexOf('=');
                        if (i > 0)
                        {
                            _params.Add(st.Substring(0, i), st.Substring(i+1));
                        }
                    }
                }
                return _params;
            }
        }

		public IEnumerable<ValueItem> PossibleValues()
		{
			return this.Director.GetChildObjects<ValueItem>(this.Id);
		}

        public Control MakeControl()
        {
            Control input = null;
            if (this.Type == "Text")
            {
                input = new Edit();
                input.ID = Control.GetUniqueID("input");
            }
            else if (this.Type == "Dropdown")
            {
                Combobox c = new Combobox();
                foreach (var value in this.PossibleValues())
                {
                    ListItem li = new ListItem();
                    li.Header = value.Name;
                    li.Value = value.Value;
                    c.Controls.Add(li);
                }
                input = c;
                input.ID = Control.GetUniqueID("input");
            }
            else if (this.Type == "Item Selector")
            {
                ASR.Controls.ItemSelector iSelect = new ASR.Controls.ItemSelector();
                input = iSelect;
                input.ID = Control.GetUniqueID("input");
                iSelect.Click = string.Concat("itemselector", ":", input.ID);
                if (this.Parameters["root"] != null) iSelect.Root = this.Parameters["root"];
                if (this.Parameters["folder"] != null) iSelect.Folder = this.Parameters["folder"];
                if (this.Parameters["displayresult"] != null) iSelect.DisplayValueType = (ASR.Controls.ItemInfo)Enum.Parse(typeof(ASR.Controls.ItemInfo), this.Parameters["displayresult"].ToString());
                if (this.Parameters["valueresult"] != null) iSelect.ValueType = (ASR.Controls.ItemInfo)Enum.Parse(typeof(ASR.Controls.ItemInfo), this.Parameters["valueresult"].ToString());
                if (this.Parameters["filter"] != null) iSelect.Filter = this.Parameters["filter"];
            }
            else if (this.Type == "User Selector")
            {
                ASR.Controls.UserSelector uSelect = new ASR.Controls.UserSelector();
                input = uSelect;
                input.ID = Control.GetUniqueID("input");
                uSelect.Click = string.Concat("itemselector", ":", input.ID);
                if (this.Parameters["filter"] != null) uSelect.Filter = this.Parameters["filter"];
            }
            else if (this.Type == "Date picker")
            {
                var dtPicker = new ASR.Controls.DateTimePicker();
                dtPicker.Style.Add("float", "left");
                dtPicker.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("input");
                dtPicker.ShowTime = false;
                dtPicker.Click = "datepicker" + ":" + dtPicker.ID;
                dtPicker.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "inline");
                dtPicker.Style.Add(System.Web.UI.HtmlTextWriterStyle.VerticalAlign, "middle");
                if (this.Parameters["Format"] != null) dtPicker.Format = this.Parameters["Format"];
                input = dtPicker;
            }
            //input.ID = Control.GetUniqueID("input");
            input.Value = this.Value;
            return input;
        }

	}
}
