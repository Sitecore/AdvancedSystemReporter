using System.Collections;
using Sitecore.Web.Authentication;

namespace ASR.Reports.Sessions
{
    public class SessionsScanner : ASR.Interface.BaseScanner
    {
        public override ICollection Scan()
        {
            return DomainAccessGuard.Sessions;
        }
    }
}
