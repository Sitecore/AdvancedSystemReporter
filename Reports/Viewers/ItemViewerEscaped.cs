using System.Web;


namespace ASR.Reports.Items
{
    public class ItemViewerEscaped : ItemViewer
    {
        protected override string GetFriendlyFieldValue(string name, Sitecore.Data.Items.Item itemElement)
        {
             var field = itemElement.Fields[name];
             if (field != null)
             {
                 switch (field.TypeKey)
                 {
                     case "memo":
                     case "rich text":
                     case "single-line text":
                     case "html":
                     case "text":
                     case "multi-line text":
                         return HttpUtility.HtmlEncode(field.Value);                         
                     default:
                         return base.GetFriendlyFieldValue(name, itemElement);
                 }
             }
            
            return string.Empty;
        }
    }
}
