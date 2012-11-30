using Sitecore.Collections;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.Authentication;

namespace ASR.Reports.Sessions
{
	public class Kick : Command
	{

		public override void Execute(CommandContext context)
		{
			StringList sl = context.CustomData as StringList;
			if (sl != null)
			{
				foreach (var sessionId in sl)
				{
					DomainAccessGuard.Kick(sessionId);
				}
				Sitecore.Context.ClientPage.SendMessage(this, "asr:refresh");
			}
		}
	}
}
