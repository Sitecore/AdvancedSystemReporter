
namespace ASR.Reports.Logs
{
  public class AuditFilter : Interface.BaseFilter
  {
      public string Verb { get; set; }
      public string User { get; set; }

    public override bool Filter(object element)
    {
      var ai = element as AuditItem;

      if (ai == null)
      {
        return true;
      }

      return (string.IsNullOrEmpty(Verb) || ai.Verb == null ||
              ai.Verb.Contains(Verb))
              && (string.IsNullOrEmpty(User) || ai.User == null ||
            ai.User.Equals(User, System.StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
