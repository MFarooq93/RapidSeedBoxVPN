using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Helper
{
    public class Sentry
    {
        public const string DefaultDsn = "https://6908a8bac8be405dbbc2c4d291dd4b9f@sentry.io/1730372";

        public static void LoggingException(Exception e)
        {
            if (!Service.Helper.Common.MuteTracker)
            {
                Task.Run(() =>
                {
                    using (SentrySdk.Init(o => { o.AttachStacktrace = true; o.SendDefaultPii = true; o.Environment = Common.Environment; o.Release = Common.ProductVersion; }))
                    {
                        SentrySdk.CaptureException(e);
                    }
                });
            }
        }

    }
}
