using System;
using ASR.Interface;
using ASR.Reports.DisplayItems;

namespace ASR.Reports.Filters
{
	class WorkflowEventDateFilter : BaseFilter
	{
		internal ItemWorkflowEvent Event
		{
			get;
			set;
		}

		#region Parameters

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
		#endregion

		public override bool Filter(object element)
		{
			if (element is ItemWorkflowEvent)
			{
				Event = element as ItemWorkflowEvent;
				return (FromDate <= Event.Date && Event.Date < ToDate);
			}
			return false;
		}
	}
}
