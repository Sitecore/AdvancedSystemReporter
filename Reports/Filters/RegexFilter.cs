using System;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class RegexFilter : ASR.Interface.BaseFilter
	{
		public const string REGEX_PARAMETER = "Regex";


		private Regex _regex;
		private Regex RegexObject
		{
			get
			{
				if (_regex == null)
				{
					string value = base.getParameter(REGEX_PARAMETER);
					if (!String.IsNullOrEmpty(value))
					{
						_regex = new Regex(value);
					}
				}
				return _regex;
			}
		}

		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				return RegexObject.IsMatch(item.Name);
			}
			return false;
		}
	}
}
