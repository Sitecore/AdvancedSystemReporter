using System.Collections;
using System.Linq;

namespace ASR.Reports.Users
{
    public class AllUsersScanner : ASR.Interface.BaseScanner
    {
        public string DomainName { 
            get
            {
                return "sitecore";
            }
        }
        public override ICollection Scan()
        {
            var domain = Sitecore.Security.Domains.Domain.GetDomain(DomainName);

            return domain.GetUsers().ToArray<Sitecore.Security.Accounts.User>();
        }
    }
}
