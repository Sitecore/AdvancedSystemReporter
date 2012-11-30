using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class ItemsOfType : ASR.Interface.BaseFilter
	{
		public string TemplateID { get; set; }

	
		public override bool Filter(object element)
		{
		    var item = element as Item;
            if (item == null) return true;

            string[] templateIDs = TemplateID.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

		    return templateIDs.Any(templateID => item.TemplateID.ToString() == templateID);
		}
	}
}
