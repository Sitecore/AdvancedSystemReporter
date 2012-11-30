using System;
using ASR.Interface;
using ASR.Reports.DisplayItems;
using Sitecore.Data.Items;
using Version = Sitecore.Data.Version;
using System.Linq;

namespace ASR.Reports.Filters
{
	class CreatedBetween : BaseFilter
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

	    

	    /// <summary>
	    /// Whether to use the first version
	    /// </summary>
	    /// <value>Use first version.</value>
        public bool UseFirstVersion { get; set; }
        

		public override bool Filter(object element)
		{
			Item item = null;
			if (element is Item)
			{
				item = element as Item;
			}
			else if (element is ItemWorkflowEvent)
			{
				item = (element as ItemWorkflowEvent).Item;
			}
			if (item != null)
			{
                if (UseFirstVersion)
                {
                    var versions = item.Versions.GetVersionNumbers();
                    var minVersion = versions.Min(v => v.Number);
                    item = item.Database.GetItem(item.ID, item.Language, new Version(minVersion)); 
                }
				DateTime dateCreated = item.Statistics.Created;
				if (FromDate <= dateCreated && dateCreated < ToDate)
				{
					return true;
				}
			}
			return false;
		}
	}
}
