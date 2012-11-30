using Sitecore.Diagnostics;
using System.Collections;

namespace ASR.Interface
{
	public abstract class BaseScanner : BaseReportObject
	{
		public abstract ICollection Scan();

		private static BaseScanner Create(string type)
		{
			return BaseReportObject.createObject(type) as BaseScanner;
		}

		internal static BaseScanner Create(string type, string parameters)
		{
			Assert.ArgumentNotNull(type, "type");
			Assert.ArgumentNotNull(parameters, "parameters");
			BaseScanner oScanner = BaseScanner.Create(type);
			oScanner.AddParameters(parameters);
			return oScanner;
		}
	}
}
