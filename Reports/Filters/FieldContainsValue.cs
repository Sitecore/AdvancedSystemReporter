using ASR.Reports.DisplayItems;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	/// <summary>
	/// Filters items based on field values they contains.
	/// </summary>
	class FieldContainsValue : ASR.Interface.BaseFilter
	{

        public string FieldName { get; set; }

        public string Value { get; set; }

		public override bool Filter(object element)
		{
			Item item = null;
			switch (element.GetType().FullName)
			{
				case "ASR.Reports.DisplayItems.ItemWorkflowEvent":
					item = element is ItemWorkflowEvent ? (element as ItemWorkflowEvent).Item : null;
					break;
				case "ASR.Reports.DisplayItems.MediaUsageItem":
					item = element is MediaUsageItem ? (element as MediaUsageItem).Item : null;
					break;
				default:
					item = element as Item;
					break;
			}

			if (item != null)
			{
				Field field = item.Fields[FieldName];
				if (field != null && field.HasValue && field.Value == Value)
				{
					return true;
				}
			}
			return false;
		}
	}
}
