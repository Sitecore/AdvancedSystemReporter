using Sitecore.Data.Items;

namespace ASR.Reports.DisplayItems
{
	public class MediaUsageItem
	{
		public Item Item { get; private set; }
		public Item Media { get; private set; }

		public MediaUsageItem(Item item, Item media)
		{
			Item = item;
			Media = media;
		}
	}
}
