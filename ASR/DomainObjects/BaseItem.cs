using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    public class BaseItem: CustomItem
    {
        public BaseItem(Item innerItem) : base(innerItem)
        {

        }

        public string Path {
            get { return InnerItem.Paths.FullPath; }
        }

        protected IEnumerable<T> GetMultilistField<T>(string name) where T : new ()
        {
            MultilistField field = InnerItem.Fields[name];
            return field.GetItems().Select(i => new T());
        } 
    }
}
