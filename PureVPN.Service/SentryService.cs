using PureVPN.Service;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using Sentry;
using Sentry.Protocol;
using System;
using System.Threading.Tasks;

// One of the ways to set your DSN is via an attribute:
// It could be set via AssemblyInfo.cs and patched via CI.
// Other ways are via environment variable, configuration files and explictly via parameter to Init
[assembly: Dsn(SentryService.DefaultDsn)]
namespace PureVPN.Service
{
    public class SentryService : ISentryService
    {
        public SentryService()
        {

        }

        public const string DefaultDsn = "https://6908a8bac8be405dbbc2c4d291dd4b9f@sentry.io/1730372";

        public void LoggingException(Exception e)
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

        public void SendVPNConnectionError(string exception, string errorCode, string protocol, string type, string value)
        {
            if (!Service.Helper.Common.MuteTracker)
            {
                Task.Run(() =>
                {
                    using (SentrySdk.Init(o => { o.AttachStacktrace = true; o.SendDefaultPii = true; o.Environment = Common.Environment; o.Release = Common.ProductVersion; }))
                    {
                        User user = new User();
                        user.Username = Entity.Models.AtomModel.Username;
                        SentrySdk.WithScope(s =>
                        {
                            s.Level = SentryLevel.Fatal;
                            s.SetExtra("Type", type);
                            s.SetExtra("Value", value);
                            s.SetExtra("Protocol", protocol);
                            s.User = user;
                            s.SetTag("code", errorCode);
                            s.SetTag("message", exception);
                            s.SetTag("release_type ", Common.ReleaseType);
                            SentrySdk.CaptureMessage("Atom VPN Connection Error");
                        });
                        //SentrySdk.CaptureException(e);
                    }
                });
            }
        }
    }


}
