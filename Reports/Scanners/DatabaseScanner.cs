using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using ASR.Reports.Items.Exceptions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ASR.Reports.Scanners
{
    public abstract class DatabaseScanner : BaseScanner
    {
        public string Root { get; set; }
        public string Db { get; set; }

        Database _db = null;
        public Database Database
        {
            get
            {
                if (_db == null)
                {
                    _db = !string.IsNullOrEmpty(Db) ? 
                        Sitecore.Configuration.Factory.GetDatabase(Db) 
                        : Sitecore.Context.ContentDatabase;

                    if (_db == null)
                    {
                        throw new DatabaseNotFoundException();
                    }
                }
                return _db;
            }
        }
        protected Item GetRootItem()
        {
            var root = string.IsNullOrEmpty(Root) ? "/sitecore" : Root;
            return Database.GetItem(root);
        }
    }
}
