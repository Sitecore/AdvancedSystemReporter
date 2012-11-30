using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace ASR
{
    static class  Util
    {
        public static void LoadFromItem(this Sitecore.Web.UI.HtmlControls.MenuItem mi, Sitecore.Data.Items.Item item)
        {
             mi.Header = item["display name"];
             mi.Icon = item["icon"];
        }
        public static void ShowItemBrowser(string header, string text, string icon, string button, string root, string selected, string database)
        {           
            UrlString str2 = new UrlString("/sitecore/shell/Applications/Item browser.aspx");
            str2.Append("db", database);
            //str2.Append("id", selected.ToString());
            str2.Append("fo", selected);
            str2.Append("ro", root);
            str2.Append("he", header);
            str2.Append("txt", text);
            str2.Append("ic", icon);
            str2.Append("btn", button);
            SheerResponse.ShowModalDialog(str2.ToString(), true);
            
        }
        public static void ShowItemBrowser(string header, string text, string icon, string button, Sitecore.Data.ID root, Sitecore.Data.ID selected, string database)
        {
            ShowItemBrowser(
                header, text, icon, button, root.ToString(), selected.ToString(), database);
        }
        public static void ShowItemBrowser(string header, string text, string icon, string button, Sitecore.Data.ID root, Sitecore.Data.ID selected)
        {
            ShowItemBrowser(
                header, text, icon, button, root, selected, Sitecore.Context.ContentDatabase.Name);
        }
        public static void ShowItemBrowser(string header, string text, string icon, string button, string root, string selected)
        {

            ShowItemBrowser(
                header, text, icon, button, root, selected, Sitecore.Context.ContentDatabase.Name);
        }
        public static void OpenItem(Item item)
        {
            Assert.IsNotNull(item, "item is null");

            Sitecore.Text.UrlString parameters = new Sitecore.Text.UrlString();
            parameters.Add("id", item.ID.ToString());
            parameters.Add("fo", item.ID.ToString());
            parameters.Add("la", item.Language.Name);
            parameters.Add("vs", item.Version.Number.ToString());
            parameters.Add("sc_content", item.Database.Name);
            Sitecore.Shell.Framework.Windows.RunApplication("Content editor", parameters.ToString());
        }
        public static void OpenItem(ItemUri uri)
        {
            Util.OpenItem(Sitecore.Data.Database.GetItem(uri));            
        }

        public static void SendMail(string to, string text)
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(Current.Context.Settings.EmailFrom,to);
                message.Subject = "ASR notification";
                message.Body = text;
                message.IsBodyHtml = false;
                SheerResponse.Alert(
                    string.Format("Email to {0}\n{1}", to, text));
               // Sitecore.MainUtil.SendMail(message);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot send email to author", ex, "SendEmail");
            }            

        }

        public static string MakeDateReplacements(string original)
        {
            var replacement = original;
            replacement = replacement.Replace("$sc_nextyear", DateTime.Today.AddYears(1).ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_nextweek", DateTime.Today.AddDays(7).ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_nextmonth", DateTime.Today.AddMonths(1).ToString("yyyyMMddTHHmmss"));

            replacement = replacement.Replace("$sc_lastyear", DateTime.Today.AddYears(-1).ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_lastweek", DateTime.Today.AddDays(-7).ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_lastmonth", DateTime.Today.AddMonths(-1).ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_yesterday", DateTime.Today.AddDays(-1).ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_today", DateTime.Today.ToString("yyyyMMddTHHmmss"));
            replacement = replacement.Replace("$sc_now", DateTime.Now.ToString("yyyyMMddTHHmmss"));
            return replacement;
        }

    }
}
