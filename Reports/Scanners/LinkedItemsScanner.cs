using System.Linq;
using ASR.Interface;
using System.Collections;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;

namespace ASR.Reports.Scanners
{
    internal class LinkedItemsScanner : BaseScanner
    {
        public static readonly string DB_PARAMETER = "db";
        public static readonly string ROOT_PARAMETER = "root";
        public static readonly string CASCADE_PARAMETER = "search";
        public static readonly string MODE_PARAMETER = "mode";


        public string LanguageFallback { get; set; }

        public override ICollection Scan()
        {
            var databasename = getParameter(DB_PARAMETER);
            var db = !string.IsNullOrEmpty(databasename)
                         ? Sitecore.Configuration.Factory.GetDatabase(databasename) ?? Sitecore.Context.ContentDatabase
                         : Sitecore.Context.ContentDatabase;

            var rootpath = getParameter(ROOT_PARAMETER);
            var rootitem = !string.IsNullOrEmpty(rootpath) ? db.GetItem(rootpath) ?? db.GetRootItem() : db.GetRootItem();

            Item[] items;
            switch (getParameter(CASCADE_PARAMETER))
            {
                case "0": //children
                    items = rootitem.Children.InnerChildren.ToArray();
                    break;
                case "1": //descendants       
                    items = rootitem.Axes.SelectItems("descendant-or-self::*");
                    break;
                default:
                    //case "-1": //item
                    items = new[] {rootitem};
                    break;
            }

            var linkdb = Sitecore.Configuration.Factory.GetLinkDatabase();

            var mode = getParameter(MODE_PARAMETER);

            var results = new ArrayList();

            foreach (var item in items)
            {
                if ("referrers" == mode)
                {
                    foreach (var i in linkdb.GetReferrers(item).Select(HackToRetrieveSourceItem))
                    {
                        if (i != null) results.Add(i);
                    }
                }
                else
                {
                    foreach (var i in linkdb.GetReferences(item).Select(HackToRetrieveTargetItem))
                    {
                        if (i != null) results.Add(i);
                    }
                }
            }
            return results;
        }


        private Item HackToRetrieveSourceItem(ItemLink l)
        {

            var database = Sitecore.Configuration.Factory.GetDatabase(l.SourceDatabaseName);
            var sourceItemLanguage = l.SourceItemLanguage == Language.Invariant && !string.IsNullOrEmpty(LanguageFallback) ? Language.Parse(LanguageFallback) : l.SourceItemLanguage;
            return database.GetItem(l.SourceItemID, sourceItemLanguage, l.SourceItemVersion);
        }

        private Item HackToRetrieveTargetItem(ItemLink l)
        {
            var database = Sitecore.Configuration.Factory.GetDatabase(l.TargetDatabaseName);

            var targetItemLanguage = l.TargetItemLanguage == Language.Invariant && !string.IsNullOrEmpty(LanguageFallback) ? Language.Parse(LanguageFallback) : l.TargetItemLanguage;
            return database.GetItem(l.TargetItemID, targetItemLanguage, l.TargetItemVersion);
        }
    }
}
