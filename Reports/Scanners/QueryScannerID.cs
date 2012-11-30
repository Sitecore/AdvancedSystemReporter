using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace ASR.Reports.Scanners
{
    class QueryScannerID : BaseScanner
    {
        public static string DB_PARAMETER = "db";
        public static string QUERY_PARAMETER = "Query";

        public override System.Collections.ICollection Scan()
        {
            var dbname = getParameter(DB_PARAMETER);
            var db = !string.IsNullOrEmpty(dbname)
                         ? Sitecore.Configuration.Factory.GetDatabase(dbname)
                         : Sitecore.Context.ContentDatabase;
            Assert.ArgumentNotNull(db,"Database can't be found");

            var query = getParameter(QUERY_PARAMETER);

            Assert.ArgumentNotNullOrEmpty(query, "Query can't be empty");

            return db.DataManager.DataSource.SelectIDs(query).ToArray();
        }
    }
}
