using ASR.Interface;

namespace ASR.Reports.Users
{
    class UserViewer : ASR.Interface.BaseViewer
    {
        public override void Display(DisplayElement dElement)
        {
            Sitecore.Security.Accounts.User user = dElement.Element as Sitecore.Security.Accounts.User;

            if (user == null)
            {
                return;
            }

            dElement.AddColumn("Name", user.Name);
            dElement.AddColumn("Display Name",user.DisplayName);
            dElement.AddColumn("Domain", user.Domain.Name);
            dElement.AddColumn("Is Admin", user.IsAdministrator ? "yes" : "no");            


        }
    }
}
