using Sitecore.Data.Items;

namespace ASR.Reports.Items
{
	class MultipleVersions : NumberFilter
	{
		public override bool Filter(object element)
		{
			Item itemElement = element as Item;
			if (itemElement != null)
			{
				return itemElement.Versions.Count >= this.Number;
			}
			return false;
		}
	}
}
