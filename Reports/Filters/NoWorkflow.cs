using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	/// <summary>
	/// Filters items which have no workflow or no workflow state but should have according to their template's standard values.
	/// </summary>
	public class NoWorkflow : ASR.Interface.BaseFilter
	{
		/// <summary>
		/// Executes the filter which returns true for items which do not have a workflow or workflow state but 
		/// they should according to their data template's standard values.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{

				if (item["__workflow"] == string.Empty || item["__workflow state"] == string.Empty)
				{
					//check if template has standard values
					Item tsv = item.Template.StandardValues;
					if (tsv != null && tsv["__default workflow"] != string.Empty)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
