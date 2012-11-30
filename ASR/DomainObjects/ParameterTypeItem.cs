using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;
using CorePoint.DomainObjects.SC;

namespace ASR.DomainObjects
{
    [Template("System/ASR/Value")]
    public class ValueItem:StandardTemplate
    {
        [Field("value")]
        public string Value
        {
            get;
            set;
        }
    }
}
