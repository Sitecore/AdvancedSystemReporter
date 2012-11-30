namespace ASR.Commands
{
  using System;
  using System.Net.Mail;

  using Sitecore;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Web.UI.Sheer;

  public abstract class ExportBaseCommand : Command
  {
    #region Public methods

    public override void Execute(CommandContext context)
    {
      try
      {
        if (Context.IsAdministrator || Current.Context.Settings.AllowNonAdminDownloads)
        {
          var tempPath = this.GetFilePath();
          SheerResponse.Download(tempPath);
        }
        else
        {
          Context.ClientPage.Start(this, "Run");
        }
      }
      catch (Exception ex)
      {
        SheerResponse.Alert(ex.Message);
      }
    }

    public override CommandState QueryState(CommandContext context)
    {
      return Current.Context.Report == null ? 
        CommandState.Disabled : base.QueryState(context);
    }

    #endregion

    #region Protected methods

    protected abstract string GetFilePath();

    protected virtual void Run(ClientPipelineArgs args)
    {
      if (!args.IsPostBack)
      {
        var email = Context.User.Profile.Email;
        SheerResponse.Input("Enter your email address", email);
        args.WaitForPostBack();
      }
      else
      {
        if (args.HasResult)
        {
          var message = new MailMessage();
          message.To.Add(args.Result);
          var tempPath = this.GetFilePath();
          message.Attachments.Add(new Attachment(tempPath));
          message.Subject = string.Format("ASR Report ({0})", Current.Context.ReportItem.Name);
          message.From = new MailAddress(Current.Context.Settings.EmailFrom);
          message.Body = "Attached is your report sent at " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
          MainUtil.SendMail(message);
        }
      }
    }

    #endregion
  }
}