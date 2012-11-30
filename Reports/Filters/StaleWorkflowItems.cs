using ASR.Interface;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.Filters
{
    class StaleWorkflowItems : BaseFilter
    {
        protected int Age { get; set; }
        
        public override bool Filter(object element)
        {
            Debug.ArgumentNotNull(element, "element");
            Item item = element as Item;
            if (item == null) return true;

            IWorkflow wf = Sitecore.Context.Workflow.GetWorkflow(item);
            if (wf == null) return false;
            WorkflowState state = wf.GetState(item);
            if (state == null || state.FinalState) return false;

            WorkflowEvent[] wevents = wf.GetHistory(item);
            if (wevents == null || wevents.Length == 0) return false;
            return (System.DateTime.Now - wevents[wevents.Length - 1].Date).Days > Age;
                        
        }
    }
}
