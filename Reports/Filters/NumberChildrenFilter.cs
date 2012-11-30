using Sitecore.Data.Items;

namespace ASR.Reports.Items
{
    public class NumberChildrenFilter:NumberFilter
    {
        
        public override bool Filter(object element)
        {
            Item itemElement = element as Item;
            if (itemElement != null)
            {
                return itemElement.Children.Count > this.Number;
            }
            return false;
        }
    }
}
