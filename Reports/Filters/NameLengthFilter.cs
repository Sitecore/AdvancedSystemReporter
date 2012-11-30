using Sitecore.Data.Items;

namespace ASR.Reports.Items
{
    public class NameLengthFilter:NumberFilter
    {
       
        public override bool Filter(object element)
        {
            Item itemElement = element as Item;
            if (itemElement != null)
            {
                Sitecore.Diagnostics.Assert.ArgumentNotNull(element, "element");
                return itemElement.Name.Length == this.Number;
            }
            return false;
        }
    }
}
