using System;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.Reports.Items
{
    public class LockFilter : ASR.Interface.BaseFilter
    {
        public string Owner { get; set; }
        
        public int Age
        {
            get; set;
        }

        public override bool Filter(object element)
        {
            Item item = element as Item;
            if ( item != null )
            {                
                LockField lField = item.Fields["__lock"];
                if ( lField != null )
                {
                    return checkDate(lField.Date, Age) && checkOwner(lField.Owner, Owner);
                }
            }
            return true;
        }

        private bool checkDate(DateTime lockingdate, int hours)
        {
            if (hours < 0) return true;

            return lockingdate.AddHours(hours).CompareTo(DateTime.Now) < 0;
        }

        private bool checkOwner(string ownername, string parameter)
        {
            if (string.IsNullOrEmpty(parameter)) return true;

            return ownername.Equals(parameter,StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
