using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace ASR.Commands
{
     class ExportXML : ExportBaseCommand
    {
        protected override string GetFilePath()
        {
           return new Export.XMLExport().Save("asr","xml");
        }
    }
}
