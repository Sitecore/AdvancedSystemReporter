using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
	
	public class ViewerItem : ReferenceItem
	{
	    public ViewerItem(Item i) : base(i)
	    {
	    }


        public string ColumnsXml {
            get { return InnerItem["columns"]; }
        }

	}
}
