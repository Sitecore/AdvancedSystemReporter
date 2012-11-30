using System;
using System.Linq;
using ASR.Interface;
using Sitecore.Workflows.Simple;
using System.Data.SqlClient;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.Scanners
{
    public class WorkflowHistory : BaseScanner
    {
        public const string AGE_PARAMETER = "Age";

        private int _days = int.MinValue;
        public int Days
        {
            get
            {
                if (_days == int.MinValue)
                {
                    if (!int.TryParse(getParameter(AGE_PARAMETER), out _days))
                    {
                        _days = 0;
                    }
                }
                return _days;
            }
        }

        public override System.Collections.ICollection Scan()
        {
            System.Collections.ArrayList result = new System.Collections.ArrayList();

            DateTime dt = DateTime.Now.AddDays(-Days);


            WorkflowProvider wProvider = Sitecore.Context.ContentDatabase.WorkflowProvider as WorkflowProvider;
            if (wProvider == null)
            {
                return result;
            }

            string stConnection =
                Sitecore.Configuration.Settings.GetConnectionString(Sitecore.Context.ContentDatabase.Name);

            SqlConnection conn = null;
            SqlCommand command = null;
            SqlDataReader reader = null;
            try
            {
                conn = new SqlConnection(stConnection);
                command = conn.CreateCommand();
                command.CommandText = "SELECT DISTINCT ItemID FROM WorkflowHistory WHERE Date > @date";
                command.Parameters.Add(
                    new SqlParameter("@date", System.Data.SqlDbType.DateTime)).Value = dt;

                conn.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Guid gId = reader.GetGuid(0);

                    Item item = Sitecore.Context.ContentDatabase.GetItem(new Sitecore.Data.ID(gId));

                    foreach (var wEvent in wProvider.HistoryStore.GetHistory(item).Where(hi => hi.Date > dt && hi.NewState != hi.OldState))
                    {
                        WorkflowEventCustom wec = new WorkflowEventCustom(item, wEvent);
                        result.Add(wec);
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message, this);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (conn != null) conn.Close();
            }

            return result;
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
