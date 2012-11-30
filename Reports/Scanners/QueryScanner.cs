using System.Collections;
using ASR.Reports.Items.Exceptions;
using ASR.Reports.Scanners;
using Sitecore.Data;
using Sitecore.Data.Items;

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
		    return results ?? new Item[0];
		}
	}
}

