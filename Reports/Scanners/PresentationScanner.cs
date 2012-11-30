using System.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ASR.Reports.Presentation
{
    public class PresentationScanner : ASR.Interface.BaseScanner
    {
        public readonly string _textsearch = "renderingid";
        public readonly string[] FolderIDs = { 
                                             "{75CC5CE4-8979-4008-9D3C-806477D57619}", //LAYOUTS
                                             "{EB443C0B-F923-409E-85F3-E7893C8C30C2}"  //SUBLAYOUTS
                                             };
        private readonly string pathfieldname = "path";

        public string Text
        {
            get
            {
                return getParameter(_textsearch);
            }
        }

        public Database DB
        {
            get
            {
                return Sitecore.Configuration.Factory.GetDatabase("master");
            }
        }

        public override ICollection Scan()
        {
            
            ASR.Reports.Items.QueryScanner qs = new ASR.Reports.Items.QueryScanner();
            qs.AddParameters(
                string.Format("Query=/sitecore/content//*[contains(@__renderings,'{0}')]",Text));


            System.Collections.ArrayList list = new System.Collections.ArrayList(qs.Scan());

            foreach (var id in FolderIDs)
            {
                Item folder = DB.GetItem(id);
                if (folder != null)
                {
                    Item[] descendants = folder.Axes.GetDescendants();
                    foreach (var item in descendants)
                    {
                        if (searchItem(item)) list.Add(item);
                    }
                }
            }
            return list;
        }

        private bool searchItem(Item item)
        {
            string path = item[pathfieldname];
            if (string.Empty != path)
            {
                return searchFile(Sitecore.IO.FileUtil.MapPath(path));
            }
            return false;
        }

        private bool searchFile(string p)
        {
            bool result = false;
            System.IO.FileInfo file = new System.IO.FileInfo(p);
            if (file.Exists)
            {
                System.IO.StreamReader sReader = file.OpenText();
                result = searchFile(sReader);
                sReader.Close();
            }
            return result;
        }

        private bool searchFile(System.IO.StreamReader sReader)
        {
            string line;
            while ((line = sReader.ReadLine()) != null)
            {
                if (line.Contains(Text)) return true;
            }
            return false;
        }
    }
}
