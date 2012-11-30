using System.Collections;
using System.Collections.Generic;
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

        private Item[] AddAllLanguageVersions(IEnumerable<Item> items)
        {
            if (items == null)
                return new Item[0];

            var results = new List<Item>();
            foreach (var item in items)
            {
                Language[] languages = item.Languages;
                foreach (var language in languages)
                {
                    Item itemInLang = item.Database.GetItem(item.ID, language);
                    if (itemInLang != null && itemInLang.Versions.Count > 0)
                        results.Add(itemInLang);
                }    
            }
            return results.ToArray();
        }

	}
}

