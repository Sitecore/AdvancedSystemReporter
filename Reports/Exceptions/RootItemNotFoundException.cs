using System;

namespace ASR.Reports.Items.Exceptions
{
    public class RootItemNotFoundException:Exception
    {
        public RootItemNotFoundException(string message)
            : base(message)
        {
        }
    }
}
