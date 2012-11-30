using System;
using Sitecore.Diagnostics;

namespace ASR.Reports.Items.Exceptions
{
	class ParameterException : Exception
	{
		public ParameterException() : this(null, null) { }

		public ParameterException(string message) : this(message, null) { }


		public ParameterException(string message, Exception innerException)
			: base(message, innerException)
		{
			Log.Error(message, innerException, this.Source);
		}
	}
}
