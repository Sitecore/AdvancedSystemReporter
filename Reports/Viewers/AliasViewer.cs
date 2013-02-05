using System.Collections.Generic;
using ASR.Interface;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;

namespace ASR.Reports.Viewers
{
    public class AliasViewer : BaseViewer
    {
       
        public override void Display(DisplayElement dElement)
        {
            var item = dElement.Element as Item;
            if (item == null) return;

            var alias = GetAlias(item);
            if(alias != null) dElement.AddColumn("Alias",alias);
        }

        private Dictionary<ID, string> _alias;
        private string GetAlias(Item item)
        {
            if (_alias == null)
            {
                InitializeAlias(item.Database);
                Error.AssertNotNull(_alias,"error initializing alias table");
            }
            return _alias.ContainsKey(item.ID) ? _alias[item.ID] : "[no alias]";
        }

        private void InitializeAlias(Database db)
        {
            _alias = new Dictionary<ID, string>();
            var aliasparent = db.GetItem("/sitecore/system/aliases");
            Error.AssertNotNull(aliasparent,"can't find aliases");
            using (new SecurityDisabler())
            {
                foreach (Item child in aliasparent.Children)
                {
                    LinkField lf = child.Fields["linked item"];
                    if (_alias.ContainsKey(lf.TargetID))
                    {
                        _alias[lf.TargetID] += string.Concat(", ", child.Name);
                    }
                    else
                    {
                        _alias.Add(lf.TargetID, child.Name);
                    }
                } 
            }
        }
    }
}
