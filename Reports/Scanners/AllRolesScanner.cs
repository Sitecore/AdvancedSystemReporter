using System.Collections;
using System.Linq;
using ASR.Interface;
using Sitecore.Security.Domains;

namespace ASR.Reports.Roles
{
    public class AllRolesScanner : BaseScanner
    {
        public string DomainName { get; set; }

        public override ICollection Scan()
        {
            return Domain.GetDomain(DomainName).GetRoles().ToArray();
        }
    }
}