using System;
using System.Text.RegularExpressions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class AnyFieldContainsToken : ASR.Interface.BaseFilter
	{
        public const string REGEX_PATTERN = @"\$(name|date|parentname|time|now|id|parentid)";

		private Regex _regex;
		private Regex RegexObject
		{
			get
			{
				if (_regex == null)
				{
					_regex = new Regex(REGEX_PATTERN);
				}
				return _regex;
			}
		}

		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
                item.Fields.ReadAll();
                foreach (Field field in item.Fields)
                {
                    if (RegexObject.IsMatch(field.Value))
                        return true;
                }
			}
			return false;
		}
	}
}
