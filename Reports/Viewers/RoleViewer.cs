using System.Linq;
using ASR.Interface;
using Sitecore.Security.Accounts;

namespace ASR.Reports.Roles
{
    public class RoleViewer : BaseViewer
    {
        public override void Display(DisplayElement dElement)
        {
            Role role = dElement.Element as Role;
            if ((Account)role == (Account)null)
                return;

            dElement.AddColumn("Name", role.DisplayName);
            dElement.AddColumn("Description", role.Description);
            dElement.AddColumn("Domain", role.Domain.Name);

            string usersInRole = string.Join("|", RolesInRolesManager.GetUsersInRole(role, true).Select( u => u.DisplayName ).ToArray());
            dElement.AddColumn("Users", usersInRole);
        }
    }
}