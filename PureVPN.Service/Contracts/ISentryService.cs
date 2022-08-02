using System;

namespace PureVPN.Service.Contracts
{
    public interface ISentryService
    {
        void LoggingException(Exception e);
        void SendVPNConnectionError(string exception, string errorCode, string protocol, string type, string value);
    }
}