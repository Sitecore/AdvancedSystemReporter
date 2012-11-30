using System;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class ItemsWithResettableVersions : ASR.Interface.BaseFilter
	{
		public override bool Filter(object element)
		{
		    var item = element as Item;
            if (item == null) return true;

            return ((item.Versions.Count == 1) && (item.Version.Number > 1));
		}
	}
}
