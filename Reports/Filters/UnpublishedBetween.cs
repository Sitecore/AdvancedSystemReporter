using System;
using Sitecore.Common;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	public class UnpublishedBetween : ASR.Interface.BaseFilter
	{

	    /// <summary>
	    /// Gets from date.
	    /// </summary>
	    /// <value>From date.</value>
        public DateTime FromDate { get; set; }

	    /// <summary>
	    /// Gets to date.
	    /// </summary>
	    /// <value>To date.</value>
        public DateTime ToDate { get; set; }

		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				DateTimeRange visibleRange = GetRange(item);
				if (visibleRange.To.InRange(FromDate, ToDate))
				{
					return true;
				}
			}
			return false;
		}

		public static DateTimeRange GetRange(Item item)
		{
			DateTime from = item.Publishing.ValidFrom;
			DateTime to = item.Publishing.ValidTo;

			if (item.Publishing.HideVersion || item.Publishing.NeverPublish)
			{
				return DateTimeRange.Empty;
			}

			Item[] laterVersions = item.Versions.GetLaterVersions();
			for (int i = laterVersions.Length - 1; i >= 0; i--)
			{
				if (!laterVersions[i].Publishing.HideVersion && laterVersions[i].Publishing.ValidFrom < to)
				{
					to = laterVersions[i].Publishing.ValidFrom;
				}
			}

			if (item.Publishing.PublishDate > from)
			{
				from = item.Publishing.PublishDate;
			}
			if (item.Publishing.UnpublishDate < to)
			{
				to = item.Publishing.UnpublishDate;
			}

			return new DateTimeRange(from, to);
		}
	}
}
