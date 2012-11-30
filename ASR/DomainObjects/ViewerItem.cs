using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;
using Sitecore.Collections;
using System.Xml;

namespace ASR.DomainObjects
{
	[Template("System/ASR/Viewer")]
	public class ViewerItem : ReferenceItem
	{
		[Field("columns")]
		public string ColumnsXml { get; set; }

	}
}
