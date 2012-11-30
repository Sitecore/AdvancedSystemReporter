using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using Sitecore.Text;
using Sitecore;
using System.Collections.Specialized;

namespace ASR.Controls
{
    public enum ItemInfo { ID = 0, Name = 1, FullPath = 2 };
    
	public class ItemSelector : Sitecore.Web.UI.HtmlControls.Control
	{
		protected Edit e;
		protected Button b;

		public ItemSelector()
			: base()
		{
			e = new Edit();
			e.ReadOnly = true;
			e.Width = new System.Web.UI.WebControls.Unit(60, System.Web.UI.WebControls.UnitType.Percentage);

			Literal l = new Literal();
			l.Text = "&nbsp;&nbsp;";
			b = new Button();
			b.Header = "select...";

			this.Controls.Add(e);
			this.Controls.Add(l);
			this.Controls.Add(b);
		}

        public ItemInfo DisplayValueType
        {
            get { return (ItemInfo)base.GetViewStateInt("DisplayValueType"); }
            set
            {
                base.SetViewStateInt("DisplayValueType", (int)value);
            }
        }

        public ItemInfo ValueType
        {
            get { return (ItemInfo)base.GetViewStateInt("ValueType"); }
            set
            {
                base.SetViewStateInt("ValueType", (int)value);
            }
        }

        public string Root
        {
            get { return base.GetViewStateString("Root"); }
            set
            {
                base.SetViewStateString("Root", value);
            }
        }

        public string Folder
        {
            get { return base.GetViewStateString("Folder"); }
            set
            {
                base.SetViewStateString("Folder", value);
            }
        }

        public string Filter
        {
            get { return base.GetViewStateString("Filter"); }
            set
            {
                base.SetViewStateString("Filter", value);
            }
        }

		public override string Value
		{
			get
			{
				return base.GetViewStateString("Value");
			}
			set
			{
				if (value != this.Value)
				{
					e.Value = ResultDisplay(value);
					string resultToStore = ResultValue(value);
					base.SetViewStateString("Value", resultToStore);
					SheerResponse.SetAttribute(e.ID, "value", resultToStore);
					base.Attributes["value"] = resultToStore;
				}
			}
		}

		public string Click
		{
			get
			{
				return b.Click;
			}
			set
			{
				b.Click = value;
			}
		}

		public void Clicked(ClientPipelineArgs args)
		{
			if (!args.IsPostBack)
			{
                NameValueCollection nvc = new NameValueCollection();
                if(!string.IsNullOrEmpty(Root)) nvc.Add("ro", Root);
                if(!string.IsNullOrEmpty(Folder)) nvc.Add("fo", Folder);
                if(!string.IsNullOrEmpty(Filter)) nvc.Add("flt", Filter);
				ItemSelectorDialog.Show(nvc);
                args.WaitForPostBack();
			}
			else
			{
				if (args.Result != null && args.Result != "undefined")
				{

					Value = args.Result;

					Sitecore.Context.ClientPage.ClientResponse.Refresh(this.Parent);
				}
			}
		}

        protected virtual string TransformResult(string p, ItemInfo info)
        {
            Item item = Sitecore.Context.ContentDatabase.GetItem(p);
            if (item != null)
            {
                switch (info)
                {
                    case ItemInfo.Name:
                        return item.Name;
                    case ItemInfo.FullPath:
                        return item.Paths.FullPath;
                    case ItemInfo.ID:
                        return item.ID.ToString();
                }
            }
            return p;
        }

		protected virtual string ResultDisplay(string p)
		{
            return TransformResult(p, DisplayValueType);
		}

		protected virtual string ResultValue(string p)
		{
            return TransformResult(p, ValueType);
		}
	}
}
