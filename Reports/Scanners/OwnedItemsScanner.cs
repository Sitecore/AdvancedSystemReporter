using System;
using System.Collections;
using System.Linq;

namespace ASR.Reports.Scanners
{
    class OwnedItemsScanner : DatabaseScanner
    {

        public string User { get; set; }

        public override ICollection Scan()
        {
            if (string.IsNullOrEmpty(User))
            {
                return GetRootItem().Axes.GetDescendants().Where(
                i => !string.IsNullOrEmpty(i.Security.GetOwner())).ToList();
            }

            return GetRootItem().Axes.GetDescendants().Where(
                i => i.Security.GetOwner().Equals(User, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}
