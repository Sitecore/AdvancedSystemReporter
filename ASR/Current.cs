using System.Web;

namespace ASR
{
    internal static class Current
    {
        internal static ASR.Context Context
        {
            get
            {
                if (HttpContext.Current.Session["ASR.Context"] == null)
                {
                    ASR.Context c = new ASR.Context();
                    HttpContext.Current.Session["ASR.Context"] = c;
                }
                return (ASR.Context)HttpContext.Current.Session["ASR.Context"];
            }

            private set
            {
                HttpContext.Current.Session["ASR.Context"] = value;
            }

        }
        internal static void ClearContext()
        {
            Context = null;
        }

    }
}
