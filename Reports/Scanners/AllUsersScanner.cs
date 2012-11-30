using System.Collections;
using System.Linq;
using Sitecore.Security.Domains;

namespace ASR.Reports.Users
{
    public class AllUsersScanner : ASR.Interface.BaseScanner
    {
        public string DomainName { get; set; }

        public override ICollection Scan()
        {
            return Domain.GetDomain(DomainName).GetUsers().ToArray();
        }
    }
}
