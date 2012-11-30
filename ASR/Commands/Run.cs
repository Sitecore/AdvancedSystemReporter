using Sitecore.Shell.Framework.Commands;

namespace ASR.Commands
{
    public class Run:Command
    {
        public override void Execute(CommandContext context)
        {
            Current.Context.Report = null;

            Sitecore.Context.ClientPage.Start(new ReportRunner(), "RunCommand");                
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Current.Context.ReportItem == null)
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }
    }
    
}
