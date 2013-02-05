using System.Linq;
using ASR.Interface;
using Sitecore.Security.Accounts;

namespace ASR.Reports.Roles
{
    public class RoleViewer : BaseViewer
    {
        public override string[] AvailableColumns
        {
            get
            {
                return new string[]
                {
                    "Name","Description","Domain","Users"
                }
                ;
            }
        }
        public override void Display(DisplayElement dElement)
        {
            var role = dElement.Element as Role;
            if ((Account)role == (Account)null)
                return;

            foreach (var column in Columns)
            {
                switch (column.Name)
                {
                    case "name":
                        dElement.AddColumn(column.Header, role.DisplayName); 
                        break;
                    case "description":
                        dElement.AddColumn(column.Header, role.Description);
                        break;
                    case "domain":
                        dElement.AddColumn(column.Header, role.Domain.Name);
                        break;
                    case "users":
                        string usersInRole = string.Join("|", RolesInRolesManager.GetUsersInRole(role, true).Select(u => u.DisplayName).ToArray());
                        dElement.AddColumn(column.Header, usersInRole);
                        break;

                }
            }
        }
    }
}