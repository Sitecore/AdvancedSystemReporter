using Sitecore.Shell.Framework.Commands;

namespace ASR.Commands
{
    public class Refresh : Command
    {       
        public override void Execute(CommandContext context)
        {
			Current.Context.Report.FlushCache();
            Sitecore.Context.ClientPage.Start(new ReportRunner(), "RunCommand");
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Current.Context.Report == null)
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }
    }
}
