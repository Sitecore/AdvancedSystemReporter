using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    
    public class ValueItem:BaseItem
    {
        public ValueItem(Item innerItem) : base(innerItem)
        {
        }
        
        public string Value
        {
            get { return InnerItem["value"]; }
        }
    }
}
