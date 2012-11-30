using System.Linq;
using ASR.Interface;
using System.Collections;
using Sitecore.Data.Items;

namespace ASR.Reports.Scanners
{
  class LinkedItemsScanner : BaseScanner
  {
    public readonly static string DB_PARAMETER = "db";
    public readonly static string ROOT_PARAMETER = "root";
    public readonly static string CASCADE_PARAMETER = "search";
    public readonly static string MODE_PARAMETER = "mode";

    public override ICollection Scan()
    {
      var databasename = getParameter(DB_PARAMETER);
      var db = !string.IsNullOrEmpty(databasename) ? Sitecore.Configuration.Factory.GetDatabase(databasename) ?? Sitecore.Context.ContentDatabase : Sitecore.Context.ContentDatabase;

      var rootpath = getParameter(ROOT_PARAMETER);
      var rootitem = !string.IsNullOrEmpty(rootpath) ? db.GetItem(rootpath) ?? db.GetRootItem() : db.GetRootItem();

      Item[] items;
      switch(getParameter(CASCADE_PARAMETER))
      {        
        case "0" : //children
          items = rootitem.Children.InnerChildren.ToArray();
          break;
        case "1" : //descendants       
          items = rootitem.Axes.SelectItems("descendant-or-self::*");
          break;
        default:
        //case "-1": //item
          items = new[] { rootitem };
          break;
      }
      
      var linkdb = Sitecore.Configuration.Factory.GetLinkDatabase();

      var mode = getParameter(MODE_PARAMETER);

      var results = new ArrayList();

      foreach (var item in items)
      {
        if ("referrers" == mode)
        {
          foreach (var i in linkdb.GetReferrers(item).Select(l => l.GetSourceItem()))
          {
            if(i!=null) results.Add(i);
          }
        }
        else
        {
          foreach (var i in linkdb.GetReferences(item).Select(l => l.GetTargetItem()))
          {
            if (i != null) results.Add(i);
          }
        }
      }
      return results;
    }

  }
}
