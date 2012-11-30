using System;
using System.Collections.Generic;
using System.Linq;
using ASR.Reports.DisplayItems;

namespace ASR.Reports.Filters
{
	class WorkflowEventsFilter : ASR.Interface.BaseFilter
	{
		public const string TRANSITIONS_PARAMETER = "transitions";

		public override bool Filter(object element)
		{
			if (element is ItemWorkflowEvent)
			{
				ItemWorkflowEvent iwe = element as ItemWorkflowEvent;
				int count = (from i in StateTransitions
							 where i.OldState == iwe.OldState && i.NewState == iwe.NewState
							 select i).Count();
				if (count > 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the state transitions.
		/// </summary>
		/// <value>The state transitions.</value>
		/// <remarks>State transitions should be listed as a comma separated list of workflow state pairs. 
		/// Separator between pair's GUID should be underscore.</remarks>
		List<StateTransition> _stateTransitions = null;
		protected List<StateTransition> StateTransitions
		{
			get
			{
				if (_stateTransitions == null)
				{
					string transitions = base.getParameter(TRANSITIONS_PARAMETER);
					_stateTransitions = new List<StateTransition>();
					foreach (string transitionPair in transitions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						string[] tmp = transitionPair.Split('_');
						_stateTransitions.Add(new StateTransition(tmp[0], tmp[1]));
					}

				}
				return _stateTransitions;
			}
		}

		protected struct StateTransition
		{
			public string OldState;
			public string NewState;

			public StateTransition(string OldState, string NewState)
			{
				this.OldState = OldState;
				this.NewState = NewState;
			}
		}
	}
}
