using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.DomainObjects
{
	public class Column
	{
		private string _Name;
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value.ToLower();
			}
		}
		public string Header;
	}
}
