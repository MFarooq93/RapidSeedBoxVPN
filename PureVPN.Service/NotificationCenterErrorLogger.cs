using NotificationCenter.Infrastructure.Contracts;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class NotificationCenterErrorLogger : INotificationCenterErrorLogger
    {
        public NotificationCenterErrorLogger(ISentryService sentryService)
        {
            SentryService = sentryService;
        }

        public ISentryService SentryService { get; }

        /// <summary>
        /// Logs Error <paramref name="ex"/>
        /// </summary>
        /// <param name="ex"></param>
        public void LogError(Exception ex)
        {
            SentryService.LoggingException(ex);
        }

        /// <summary>
        /// Logs Error <paramref name="ex"/> with details <paramref name="properties"/> for <paramref name="eventName"/>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        /// <param name="ex"></param>
        public void LogError(string eventName, Dictionary<string, object> properties = null, Exception ex = null)
        {
            SentryService.LoggingException(ex);
        }
    }
}