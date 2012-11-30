using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ASR.Interface;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Workflows;
using Sitecore.Workflows.Simple;

namespace ASR.Reports.Scanners
{
    public class WorkflowHistory : BaseScanner
    {
        public const string AGE_PARAMETER = "Age";
        private int _days = int.MinValue;
        private Database _db;
        private Item _root;

        public int Days
        {
            get
            {
                if (_days == int.MinValue && !int.TryParse(getParameter("Age"), out _days))
                    _days = 0;
                return _days;
            }
        }

        public Item RootItem
        {
            get { return _root ?? (_root = Db.GetItem(getParameter("root"))); }
        }

        protected Database Db
        {
            get
            {
                if (_db == null)
                    _db = Sitecore.Context.ContentDatabase ?? Factory.GetDatabase("master");
                return _db;
            }
        }

        public override ICollection Scan()
        {
            var arrayList = new ArrayList();
            DateTime dt = DateTime.Now.AddDays(-Days);
            var workflowProvider = Db.WorkflowProvider as WorkflowProvider;
            if (workflowProvider == null)
                return arrayList;
            string connectionString = Sitecore.Configuration.Settings.GetConnectionString(Db.Name);
            SqlConnection sqlConnection = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = "SELECT DISTINCT ItemID, Language FROM WorkflowHistory WHERE Date > @date";
                command.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = dt;
                sqlConnection.Open();
                sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Item obj = Db.GetItem(new ID(sqlDataReader.GetGuid(0)), Language.Parse(sqlDataReader.GetString(1)));
                    if (!obj.Paths.Path.StartsWith(RootItem.Paths.Path))
                        continue;

                    foreach (WorkflowEvent wEvent in (workflowProvider.HistoryStore.GetHistory(obj)).Where((hi =>
                    {
                        if (hi.Date > dt)
                            return hi.NewState != hi.OldState;
                        return
                            false;
                    })))
                    {
                        var workflowEventCustom = new WorkflowEventCustom(obj, wEvent);
                        arrayList.Add(workflowEventCustom);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
            }
            finally
            {
                if (sqlDataReader != null)
                    sqlDataReader.Close();
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
            return arrayList;
        }
    }

    public class WorkflowEventCustom
    {
        public WorkflowEvent WorkflowEvent { get; private set; }
        public Item Item { get; private set; }
        public WorkflowEventCustom(Item item, WorkflowEvent wEvent)
        {
            Item = item;
            WorkflowEvent = wEvent;
        }
    }
}
