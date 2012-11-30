using System.Collections;
using System.IO;

namespace ASR.Reports.Presentation
{
    public class FileScanner : ASR.Interface.BaseScanner
    {
        public readonly string _foldersParameter = "folders";
        public readonly string _textsearch = "text";

        public string Text
        {
            get
            {
                return getParameter(_textsearch);
            }
        }

        public string _folders
        {
            get
            {
                return getParameter(_foldersParameter);
            }
        }
        public string[] Folders
        {
            get
            {
                if (_folders == null) return new string[0];
                return _folders.Split(',');
            }
        }
        public override ICollection Scan()
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (var folder in Folders)
            {
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Sitecore.IO.FileUtil.MapPath(folder));
                if (dirInfo.Exists)
                {
                    scanFolder(dirInfo, list);
                }
            }
            return list;
        }

        private void scanFolder(System.IO.DirectoryInfo dirInfo, System.Collections.ArrayList list)
        {
            FileInfo[] files = dirInfo.GetFiles("*.ascx|*.aspx");
            foreach (var file in files)
            {
                if(scanFile(file)) list.Add(file);
            }
        }

        private bool scanFile(FileInfo file)
        {
            StreamReader sReader = file.OpenText();
            string line;
            while((line = sReader.ReadLine()) != null)
            {
                if (line.Contains(Text)) return true; 
            }
            return false;
        }

       


    }
}
