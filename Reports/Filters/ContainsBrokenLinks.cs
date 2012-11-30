using ASR.Interface;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
    public class ContainsBrokenLinks : BaseFilter
    {
        public bool AllVersions { get; set; }
        public override bool Filter(object element)
        {
            var item = element as Item;
            if (item == null) return true;
            var brokenlinks = item.Links.GetBrokenLinks(AllVersions);
            return (brokenlinks != null && brokenlinks.Length > 0);
        }
    }
}
