using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Contracts
{
    /// <summary>
    /// Error logger to log errors
    /// </summary>
    public interface INotificationCenterErrorLogger
    {
        /// <summary>
        /// Logs Error <paramref name="ex"/>
        /// </summary>
        /// <param name="ex"></param>
        void LogError(Exception ex);

        /// <summary>
        /// Logs Error <paramref name="ex"/> with details <paramref name="properties"/> for <paramref name="eventName"/>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        /// <param name="ex"></param>
        void LogError(string eventName, Dictionary<string, object> properties = null, Exception ex = null);
    }
}
