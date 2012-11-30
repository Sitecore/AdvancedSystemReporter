using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Sitecore.Data.Items;
using Sitecore.Workflows;
using ASR.Reports.DisplayItems;

namespace ASR.Reports.Scanners
{
	class WorkflowEventScanner : DatabaseScanner
	{
		#region Parameters
		public enum Mode { Item = -1, Descendants = 1, Children = 0 };
		private string _scanMode;
		/// <summary>
		/// Gets the scanning deeph.
		/// </summary>
		/// <value>The deep.</value>
		/// <remarks>We can scan Item (-1), Descendants (1) or Children (0).</remarks>
		public Mode ScanMode
		{
			get
			{
				if (string.IsNullOrEmpty(_scanMode))
				{
					_scanMode = getParameter("deep");
				}
				return (Mode)int.Parse(_scanMode);
			}
		}

	    /// <summary>
	    /// Gets a value indicating whether all versions should be scanned.
	    /// </summary>
	    /// <value><c>true</c> if all versions should be scanned; otherwise, <c>false</c>.</value>
        public bool AllVersions
        {
            get; set;
        }

  
		#endregion

		/// <summary>
		/// Scans for workflow events.
		/// </summary>
		/// <returns></returns>
		public override ICollection Scan()
		{
			var results = new List<ItemWorkflowEvent>();
		    var rootitem = GetRootItem();
			Item[] items;
			switch (ScanMode)
			{
				case Mode.Descendants:
					items = rootitem.Axes.GetDescendants();
					break;
				case Mode.Children:
                    items = rootitem.Axes.SelectItems("./*");
					break;
				default:
					items = new Item[] { rootitem };
					break;
			}

			foreach (Item item in items)
			{
				if (AllVersions)
				{
					var versions = item.Versions.GetVersions().OrderBy(i => i.Version.Number);
					foreach (var version in versions)
					{
						AddEvents(results, version);
					}
				}
				else
				{
					AddEvents(results, item);
				}
			}

			return results;
		}

		public IWorkflow GetWorkflow(Item item)
		{
			return item.Database.WorkflowProvider.GetWorkflow(item);
		}

		/// <summary>
		/// Adds the events.
		/// </summary>
		/// <param name="results">The results.</param>
		/// <param name="version">The version.</param>
		private void AddEvents(List<ItemWorkflowEvent> results, Item item)
		{
			IWorkflow workflow = GetWorkflow(item);
			if (workflow != null)
			{
				var events = from wEvent in workflow.GetHistory(item)
							 orderby wEvent.Date ascending
							 select new ItemWorkflowEvent(wEvent.OldState, wEvent.NewState, wEvent.Text, wEvent.User, wEvent.Date)
							 {
								 Item = item
							 };
				results.AddRange(events);
			}
		}
	}
}
