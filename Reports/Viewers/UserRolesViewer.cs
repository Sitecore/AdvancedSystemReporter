using ASR.Interface;
using Sitecore.Security.Accounts;

namespace ASR.Reports.Users
{
    public class UserRolesViewer : BaseViewer
    {
        public override void Display(DisplayElement dElement)
        {
            User user = dElement.Element as User;
            if ((Account)user == (Account)null)
                return;

            foreach (var userRole in user.Roles)
            {
                dElement.AddColumn(userRole.Name, "yes");    
            }            
        }
    }
}