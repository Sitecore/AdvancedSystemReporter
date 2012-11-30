using System;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.DisplayItems
{
	public class ItemWorkflowEvent : WorkflowEvent
	{
		public ItemWorkflowEvent(string oldState, string newState, string text, string user, DateTime date)
			: base(oldState, newState, text, user, date) { }

		public Item Item { get; set; }
	}
}
