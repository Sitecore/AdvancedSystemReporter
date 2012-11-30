using ASR.Interface;
using Sitecore.Diagnostics;

namespace ASR.Reports.Sessions
{
    public class SessionsViewer : ASR.Interface.BaseViewer
    {
        public override void Display( DisplayElement dElement)
        {
            Assert.ArgumentNotNull(dElement, "element");
            Sitecore.Web.Authentication.DomainAccessGuard.Session session =
                dElement.Element as Sitecore.Web.Authentication.DomainAccessGuard.Session;
            if (session != null)
            {
                dElement.AddColumn("User", session.UserName);
                dElement.AddColumn("Created", session.Created.ToString("dd/MM/yy HH:mm"));
                dElement.AddColumn("Last request", session.LastRequest.ToString("dd/MM/yy HH:mm"));
                dElement.AddColumn("ID", session.SessionID);

                dElement.Value = session.SessionID;
            }

            //return dElement;
        }

        //public override IEnumerable<string> GetColumnNames()
        //{
        //    return new string[] {"User","Created","Last request","ID"};
        //}
    }
}
