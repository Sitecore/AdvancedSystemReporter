using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.Sheer;
using Sitecore;
using Sitecore.Web.UI.HtmlControls;
using ASR.Interface;

namespace ASR.Controls
{
	public class ASRListview : Sitecore.Web.UI.HtmlControls.Listview
	{
		public int CurrentPage
		{
			get { return GetViewStateInt("CurrentPage"); }
			set { SetViewStateInt("CurrentPage", value); }
		}

		public ASRListview()
			: base()
		{

		}
       

		/// <summary>
		/// Executes the click event.
		/// </summary>
		/// <param name="message">The message.</param>
		protected override void DoClick(Message message)
		{
			Assert.ArgumentNotNull(message, "message");
			string source = Sitecore.Context.ClientPage.ClientRequest.Source;
			if (source.StartsWith("SortBy"))
			{
				string columnIndex = source.Substring(source.IndexOf("_") + 1);
                int colindex = MainUtil.GetInt(columnIndex, 0);
                if (colindex > 0)
                {
                    this.SortByColumn(colindex);
                }
                else
                {
                    SelectAll();
                }
                        

			}
			else
			{
				base.DoClick(message);
			}
		}

        private void SelectAll()
        {
            foreach (var item in this.Items)
            {
                item.Selected = !item.Selected;
            }
        }

        

		/// <summary>
		/// Sorts the by column.
		/// </summary>
		/// <param name="index">The index.</param>
		protected void SortByColumn(int index)
		{
			string key = base.ColumnNames.GetKey(index);
			if (this.SortBy == key)
			{
				this.SortAscending = !this.SortAscending;
			}
			else
			{
				this.SortBy = key;
				this.SortAscending = true;
			}
			Func<DisplayElement, string> orderFunc = t => t.GetColumnValue(key);
			IEnumerable<DisplayElement> sorterResultSet = null;
			if (SortAscending)
			{
				sorterResultSet = Current.Context.Report.DisplayElements.OrderBy(orderFunc, SmartComparer.Instance);
			}
			else
			{
				sorterResultSet = Current.Context.Report.DisplayElements.OrderByDescending(orderFunc, SmartComparer.Instance);
			}
			Current.Context.Report.DisplayElements = sorterResultSet.ToList();
			this.Refresh();
		}

		/// <summary>
		/// Refreshes this Listview.
		/// </summary>
		public override void Refresh()
		{
			if (Sitecore.Context.ClientPage.IsEvent)
			{
				this.Controls.Clear();
				Sitecore.Context.ClientPage.ClientResponse.DisableOutput();
				this.Populate();
				Sitecore.Context.ClientPage.ClientResponse.EnableOutput();
				Sitecore.Context.ClientPage.ClientResponse.SetOuterHtml(this.ID, this);
			}
		}

		/// <summary>
		/// Populates this instance.
		/// </summary>
		protected virtual void Populate()
		{
			this.Controls.Clear();
			this.ColumnNames.Clear();
			this.ColumnNames.Add("Icon", "Icon");

			HashSet<string> columnNames = new HashSet<string>();

			int count = ASR.Current.Context.Settings.PageSize;
			int start = (CurrentPage - 1) * count;

			foreach (var result in Current.Context.Report.GetResultElements(start, count))
			{
				ListviewItem lvi = new ListviewItem();
				lvi.ID = Control.GetUniqueID("lvi");
				lvi.Icon = result.Icon;
				lvi.Value = result.Value;
				foreach (var column in result.GetColumnNames())
				{
					columnNames.Add(column);
					lvi.ColumnValues.Add(column, result.GetColumnValue(column));
				}
				this.Controls.Add(lvi);
			}
			foreach (var column in columnNames)
			{
				this.ColumnNames.Add(column, column);
			}
		}

		class SmartComparer : IComparer<string>
		{
			private static SmartComparer _instance;
			public static SmartComparer Instance
			{
				get
				{
					if (_instance == null)
					{
						_instance = new SmartComparer();
					}
					return _instance;
				}
			}
			private SmartComparer() { }

			#region IComparer<string> Members

			public int Compare(String x, String y)
			{
				DateTime dateX;
				DateTime dateY;
				int intX;
				int intY;
				if (DateTime.TryParse(x, out dateX) && DateTime.TryParse(y, out dateY))
				{
					return dateX.CompareTo(dateY);
				}
				else if (int.TryParse(x, out intX) && int.TryParse(y, out intY))
				{
					return intX.CompareTo(intY);
				}
				return x.CompareTo(y);
			}

			#endregion
		}
	}
}
