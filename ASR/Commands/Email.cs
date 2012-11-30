using Sitecore.Shell.Framework.Commands;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace ASR.Commands
{
    public class Email:Command
    {
        public override void Execute(CommandContext context)
        {
            Dictionary<string,List<Item>> itemsByUser = new Dictionary<string,List<Item>>();
            
            foreach (var item in context.Items)
            {
                string owner = item[Sitecore.FieldIDs.Owner];
                if (!string.IsNullOrEmpty(owner))
                {
                    if (!itemsByUser.ContainsKey(owner))
                    {
                        itemsByUser[owner] = new List<Item>();
                    }
                    itemsByUser[owner].Add(item);
                }
            }

            foreach (var owner in itemsByUser.Keys)
            {
                sendMail(owner, itemsByUser[owner]);
            }
        }

        private void sendMail(string owner, List<Item> list)
        {
            Sitecore.Security.UserProfile profile = getProfile(owner);
            string email = profile.Email;
            if (!string.IsNullOrEmpty(email))
            {
                System.Text.StringBuilder itempaths = new System.Text.StringBuilder();
                foreach (var item in list)
                {
                    itempaths.AppendFormat("{0},", item.Paths.FullPath);
                }
                itempaths.Remove(itempaths.Length - 1, 1);
                string text = Current.Context.ReportItem.EmailText
                    .Replace("$sc_user", profile.FullName)
                    .Replace("$sc_item", itempaths.ToString())
                    .Replace("$sc_currentuser", Sitecore.Context.User.Profile.FullName);

                Util.SendMail(email, text);
            }
        }

        private Sitecore.Security.UserProfile getProfile(string owner)
        {            
            return Sitecore.Security.Accounts.User.FromName(owner, false).Profile;
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Current.Context.Report == null)
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }
    }
    
}
