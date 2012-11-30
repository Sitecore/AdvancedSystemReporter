using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Sitecore.Diagnostics;

namespace ASR.Reports.Logs
{
	public class LogScanner : ASR.Interface.BaseScanner
	{
		public readonly static string DEFAULT_PATTERN = "*.txt";
		public readonly static string ENTRY_TYPES_PARAMETER = "types";
		

		private string valid_types;
		protected string Valid_Types
		{
			get
			{
				if (valid_types == null)
				{
					valid_types = getParameter(ENTRY_TYPES_PARAMETER);
					if (string.IsNullOrEmpty(valid_types))
					{
						valid_types = "ERROR|WARN|INFO  AUDIT|INFO";
					}
					else
					{
						List<string> types = new List<string>();
						if (valid_types.Contains("audit"))
						{
							types.Add("INFO  AUDIT");
						}
						if (valid_types.Contains("info"))
						{
							types.Add("INFO");
						}
						if (valid_types.Contains("warning"))
						{
							types.Add("WARN");
						}
						if (valid_types.Contains("error"))
						{
							types.Add("ERROR");
						}
						valid_types = Sitecore.StringUtil.Join(types, "|");
					}
				}
				return valid_types;
			}
		}

		protected int Age
        {
            get; set;        
        }

		protected string logFolder
		{
			get
			{
				return Sitecore.Configuration.Settings.LogFolder;
			}
		}

		protected string tempFolder
		{
			get
			{
				return Sitecore.StringUtil.EnsurePostfix('\\', Sitecore.IO.FileUtil.MapPath(Sitecore.Configuration.Settings.TempFolderPath));
			}
		}

		public override ICollection Scan()
		{
			List<LogItem> results = new List<LogItem>();
			DirectoryInfo root = new DirectoryInfo(logFolder);
			if (!root.Exists)
			{
				return results;
			}
			foreach (FileInfo file in root.GetFiles(DEFAULT_PATTERN))
			{
				results.AddRange(Scan(file));
			}
			return results;
		}

		private IEnumerable<LogItem> Scan(FileInfo file)
		{
			List<LogItem> logItems = new List<LogItem>();
			try
			{
				DateTime filedate = getFileDate(file.FullName);
				if (Age > 0)
				{
					DateTime limit = DateTime.Now.AddHours(-Age);
					if (limit > filedate)
					{
						return logItems;
					}
				}

				StreamReader sr;
				try
				{
					sr = file.OpenText();
				}
				catch (Exception)
				{
					file = file.CopyTo(tempFolder + file.Name, true);
					sr = file.OpenText();
				}
				string filecontent = sr.ReadToEnd();
				sr.Close();

				string pattern = string.Concat(@"^(?<pid>\d+|ManagedPoolThread \#\d+) +(?<time>[\d:]{8}) +(?<type>", Valid_Types, @")(?<text>((?!^Managed|^\d+)[\W\w])*)");

				MatchCollection matches = Regex.Matches(filecontent, pattern, RegexOptions.Multiline);
				foreach (Match match in matches)
				{
					string[] timeparts = match.Groups["time"].Value.Split(':');
					DateTime dateTime = filedate.AddHours(int.Parse(timeparts[0]))
						.AddMinutes(int.Parse(timeparts[1]))
						.AddSeconds(int.Parse(timeparts[2]));

					logItems.Add(LogItem.Make(dateTime, match.Groups["pid"].Value, match.Groups["type"].Value, match.Groups["text"].Value));
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception during log file scanning.", ex, this);
			}
			return logItems;
		}

		private DateTime getFileDate(string p)
		{
			DateTime filedate = DateTime.MinValue;
			System.Text.RegularExpressions.Match match =
				System.Text.RegularExpressions.Regex.Match(p, @"log\.([0-9]*)\.");
			if (match.Groups.Count == 2)
			{
				filedate = DateTime.ParseExact(match.Groups[1].Value, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

			}
			return filedate;
		}
	}
}
