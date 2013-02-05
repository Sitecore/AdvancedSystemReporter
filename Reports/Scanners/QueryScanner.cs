using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASR.Reports.Items.Exceptions;
using ASR.Reports.Scanners;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace ASR.Reports.Items
{
    /// <summary>
    /// Scans a database according to a Sitecore Query. Accepts as parameters:
    /// Query: the sitcore Query,
    /// db: the database to use. By default "master".
    /// root: the root item (an ID or a path). By default runs the Query against the database object.
    /// </summary>
    public class QueryScanner : DatabaseScanner
    {
        private const string _showeachlanguage = "ShowEachLanguage";

        //public static string QUERY_PARAMETER = "Query";

        public string Query { get; set; }


        public override ICollection Scan()
        {
            //string Query = getParameter(QUERY_PARAMETER);

            Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(Query, "Query can't be empty");

            Item[] results;


            if (Query.StartsWith("/") || Query.StartsWith("fast:"))
            {
                results = Database.SelectItems(Query);
            }
            else
            {
                Item rootItem = GetRootItem();
                if (rootItem == null)
                {
                    throw new RootItemNotFoundException("Can't find root item " + Root);
                }
                results = rootItem.Axes.SelectItems(Query);
            }

            return AddAllLanguageVersions(results);
        }

        private Item[] AddAllLanguageVersions(Item[] items)
        {
            if (items == null)
                return new Item[0];
            if (getParameter(_showeachlanguage) != "1") return items;

            return (
                from item in items 
                let languages = item.Languages 
                from language in languages
                select item.Database.GetItem(item.ID, language) 
                into itemInLang where itemInLang != null && itemInLang.Versions.Count > 0 select itemInLang
                ).ToArray();
        }

    }
}

