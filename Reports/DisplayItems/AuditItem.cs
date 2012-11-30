using System.Text.RegularExpressions;

namespace ASR.Reports.Logs
{
    public class AuditItem : LogItem
    {
        
        public string User { get; private set; }
        public string Verb { get; private set; }
        
        public Sitecore.Data.ItemUri ItemUri { get; private set; }

        public void Initialize(string line)
        {            
            Match match = Regex.Match(line, @"\((?<user>\w+\\\w+)\)[\W:]+(?<verb>[^:\r\n]+)[:\W]*(?<rest>.*)");
            if (match.Success)
            {
                User = match.Groups["user"].Value;
                Verb = match.Groups["verb"].Value;

                string rest = match.Groups["rest"].Value;
                ItemUri = string.IsNullOrEmpty(rest) ? null : parseItemUri(rest);
            }
        }

        private Sitecore.Data.ItemUri parseItemUri(string Message)
        {
            Match match = Regex.Match(Message, @"(?<db>\w+):(?<path>[^,]+), language: (?<language>[^,]+), version: (?<version>[^,]+), id: (?<id>\{.{36}\}).*");
            if (!match.Success) return null;

            Sitecore.Data.Version version = new Sitecore.Data.Version(match.Groups["version"].Value);
            Sitecore.Globalization.Language language = Sitecore.Globalization.Language.Parse(match.Groups["language"].Value);
            Sitecore.Data.ID id = Sitecore.Data.ID.Parse(match.Groups["id"].Value);
            Sitecore.Data.Database db = Sitecore.Data.Database.GetDatabase(match.Groups["db"].Value);
            return new Sitecore.Data.ItemUri(id, language, version, db);
        }
       
    }
}
