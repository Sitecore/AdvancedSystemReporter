using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using ASR.Reports.Items.Exceptions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ASR.Reports.Scanners
{
    class OwnedItemsScanner : DatabaseScanner
    {
        
        public string User { get; set; }
      
        public override ICollection Scan()
        {
            return GetRootItem().Axes.GetDescendants().Where(
                i => i.Security.GetOwner().Equals(User, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}
