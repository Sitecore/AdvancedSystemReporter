using ASR.Interface;
using System.Linq;

namespace ASR.Reports.Users
{
    class UserViewer : ASR.Interface.BaseViewer
    {
        public override string[] AvailableColumns
        {
            get { return new string[] { "Name", "DisplayName", "Domain", "IsAdmin", "Roles", "Email", "LastUpdated", "ClientLanguage", "ContentLanguage", "Comment" }; }
        }
        public override void Display(DisplayElement dElement)
        {
            Sitecore.Security.Accounts.User user = dElement.Element as Sitecore.Security.Accounts.User;

            if (user == null)
            {
                return;
            }

            foreach (var column in Columns)
            {
                switch (column.Name)
                {
                    case "name":
                        dElement.AddColumn(column.Header, user.Name);
                        break;
                    case "displayname":
                        dElement.AddColumn(column.Header, user.DisplayName);
                        break;
                    case "domain":
                        dElement.AddColumn(column.Header, user.Domain.Name);
                        break;
                    case "isadmin":

                        dElement.AddColumn(column.Header, user.IsAdministrator ? "yes" : "no");
                        break;
                    case "roles":
                        user.Roles.Select(r => r.DisplayName).Aggregate((a, c) => a += ", " + c);
                        break;
                    case "fullname":
                        dElement.AddColumn(column.Header, user.Profile.FullName);
                        break;
                    case "email":
                        dElement.AddColumn(column.Header, user.Profile.Email);
                        break;

                    case "lastupdated":
                        dElement.AddColumn(column.Header, user.Profile.LastUpdatedDate.ToString("dd/MM/yy HH:mm"));
                        break;
                    case "clientlanguage":
                        dElement.AddColumn(column.Header, user.Profile.ClientLanguage);
                        break;
                    case "contentlanguage":
                        dElement.AddColumn(column.Header, user.Profile.ContentLanguage);
                        break;
                    case "comment":
                        dElement.AddColumn(column.Header, user.Profile.Comment);
                        break;
                    default:
                        dElement.AddColumn(column.Header, user.Profile[column.Name]);
                        break;
                }
            }
        }
      
    }
}
