using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    
    public class FilterItem:ReferenceItem
    {
        public FilterItem(Item innerItem) : base(innerItem)
        {
        }
    }
}
