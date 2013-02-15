namespace ASR
{
  using Sitecore;

  public class Settings
	{
		private static Settings _instance;
		public static Settings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Settings();
				}
				return _instance;
			}
		}

		protected Settings() { }

    ///// <summary>
    ///// Gets the configuration node.
    ///// </summary>
    ///// <value>The configuration node.</value>
    //public string ConfigurationNodes
    //{
    //  get
    //  {
    //    return "/sitecore/system/Modules/ASR";
    //  }
    //}

		/// <summary>
		/// Gets the configuration database.
		/// </summary>
		/// <value>The configuration database.</value>
		/// 
		public string ConfigurationDatabase
		{
			get
			{
        return Sitecore.Configuration.Settings.GetSetting("ASR.ConfigurationDatabase", "master");
			}
		}

		
		/// <summary>
		/// Gets the reports folder.
		/// </summary>
		/// <value>The reports folder.</value>
    public string ReportsFolder
    {
      get
      {
        return Sitecore.Configuration.Settings.GetSetting("ASR.ReportsFolder", "/sitecore/system/Modules/ASR/Reports");
      }
    }

    public string ParametersFolder
    {
      get
      {
        return Sitecore.Configuration.Settings.GetSetting("ASR.ParametersFolder", "/sitecore/system/Modules/ASR/Configuration/Parameters");
      }
    }

    public string EmailFrom
    {
      get
      {
        return Sitecore.Configuration.Settings.GetSetting("ASR.EmailFrom", Sitecore.Context.User.Profile.Email);
      }
    }

      public string ParameterRegex
      {
          get { return Sitecore.Configuration.Settings.GetSetting("ASR.ParameterRegex", @"\{(\w*)\}"); }
      }

	  /// <summary>
	  /// Gets the size of the page.
	  /// </summary>
	  /// <value>The size of the page.</value>
	  private int pageSize = int.MinValue;
		public int PageSize
		{
			get
			{
        if (pageSize < 0) pageSize = int.Parse(Sitecore.Configuration.Settings.GetSetting("ASR.PageSize", "30"));
				return pageSize;
			}
		}

		/// <summary>
		/// Gets the max number pages.
		/// </summary>
		/// <value>The max number pages.</value>
    private int maxNumberPages = int.MinValue;
    public int MaxNumberPages
		{
			get
			{
        if (maxNumberPages < 0) maxNumberPages = int.Parse(Sitecore.Configuration.Settings.GetSetting("ASR.MaxNoPages", "40"));
        return maxNumberPages;
			}
		}
	
    public bool AllowNonAdminDownloads
    {
      get
      {
        return "true" == Sitecore.Configuration.Settings.GetSetting("ASR.AllowNonAdminDownloads", "false");
      }
    }
	}
}
