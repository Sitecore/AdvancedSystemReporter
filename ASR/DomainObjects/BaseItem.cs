using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    public abstract class BaseItem: CustomItem
    {
        protected BaseItem(Item innerItem) : base(innerItem)
        {
        }

        public string Path {
            get { return InnerItem.Paths.FullPath; }
        }

        protected IEnumerable<Item> GetMultilistField(string name)
        {
            MultilistField field = InnerItem.Fields[name];
            return field.GetItems();
        } 
    }
}
