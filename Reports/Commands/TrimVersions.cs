using Sitecore;
using System.Linq;

namespace ASR.Reports.Commands
{
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;

    public class TrimVersions : Command
    {
        public int VersionsToKeep { get; set; }
        public override void Execute([NotNull] CommandContext context)
        {
            foreach (var item in context.Items)
            {
                Item[] versions = item.Versions.GetVersions(false);
                var versionsToRemove = versions.Length - VersionsToKeep;
                if (versionsToRemove > 0)
                {
                    for (int i = 0; i < versionsToRemove; i++)
                    {
                        versions[i].Versions.RemoveVersion();
                    }
                }
            }
        }
    }
}
