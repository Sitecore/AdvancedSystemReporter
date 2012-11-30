using ASR.Interface;

namespace ASR
{
	public class Context
	{
		internal Context()
		{ }

		public DomainObjects.ReportItem ReportItem { get; set; }

		public Report Report { get; set; }

		public string Name { get; set; }

		public Settings Settings { get { return Settings.Instance; } }
	}
}
