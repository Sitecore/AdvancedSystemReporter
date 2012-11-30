using System;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class InvalidName : ASR.Interface.BaseFilter
	{
		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				return !ItemUtil.IsItemNameValid(item.Name);
			}
			return true;
		}
	}
}
