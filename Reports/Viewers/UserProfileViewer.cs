using ASR.Interface;
using Sitecore.Security.Accounts;

namespace ASR.Reports.Users
{
    public class UserProfileViewer : BaseViewer
    {
        public override void Display(DisplayElement dElement)
        {
            User user = dElement.Element as User;
            if ((Account) user == (Account) null)
                return;

            dElement.AddColumn("Email", user.Profile.Email);
            dElement.AddColumn("FullName", user.Profile.FullName);
            dElement.AddColumn("Last Updated", user.Profile.LastUpdatedDate.ToString("dd/MM/yy HH:mm"));
            dElement.AddColumn("Client Language", user.Profile.ClientLanguage);
            dElement.AddColumn("Content Language", user.Profile.ContentLanguage);
            dElement.AddColumn("Comment", user.Profile.Comment);
            
        }
    }
}