using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Extensions
{
    public static class NotificationExtensions
    {
        /// <summary>
        /// Clones value to <see cref="INotification"/> <paramref name="notification"/> from <paramref name="data"/>
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="data"></param>
        public static void Clone(this INotification notification, Dictionary<string, object> data)
        {
            notification.Category = GetValue<string>("category", data);
            notification.CreatedOn = GetValue<long>("created_on", data);
            notification.Icon = GetValue<string>("icon", data);
            notification.IsRead = GetValue<bool>("is_read", data);
            notification.Type = GetValue<string>("type", data);

            DateTime expiry;
            var expiryString = GetValue<string>("expiry", data);
            DateTime.TryParse(expiryString, out expiry);
            notification.Expiry = expiry;

            notification.CtaPrimary = CloneClickToAction("cta_primary", data);
            notification.CtaSecondary = CloneClickToAction("cta_secondary", data);
            notification.CtaTertiary = CloneClickToAction("cta_tertiary", data);

            notification.Title = CloneTextContent("title", data);
            notification.Description = CloneTextContent("description", data);
        }


        /// <summary>
        /// Get value for <paramref name="key"/> from <paramref name="keyValuePairs"/>
        /// </summary>
        /// <typeparam name="T">Type of value for <paramref name="key"/></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        private static T GetValue<T>(string key, Dictionary<string, object> keyValuePairs)
        {
            object value;
            if (keyValuePairs.TryGetValue(key, out value))
            {
                if (value is T)
                {
                    return (T)value;
                }
            }
            return default(T);
        }

        private static ClickToActionDTO CloneClickToAction(string key, Dictionary<string, object> keyValuePairsArgs)
        {
            ClickToActionDTO clickToAction = null;

            var keyValuePairs = GetValue<Dictionary<string, object>>(key, keyValuePairsArgs);

            if (keyValuePairs != null)
            {
                clickToAction = new ClickToActionDTO();
                clickToAction.Action = GetValue<string>("action", keyValuePairs);
                clickToAction.Destination = GetValue<string>("destination", keyValuePairs);
                clickToAction.Title = CloneTextContent("title", keyValuePairs);
            }
            return clickToAction;
        }

        private static TextContentDTO CloneTextContent(string key, Dictionary<string, object> keyValuePairsArgs)
        {
            TextContentDTO textContent = null;

            var keyValuePairs = GetValue<Dictionary<string, object>>(key, keyValuePairsArgs);

            if (keyValuePairs != null)
            {
                textContent = new TextContentDTO();
                textContent.En = GetValue<string>("en", keyValuePairs);
                textContent.De = GetValue<string>("de", keyValuePairs);
                textContent.Fr = GetValue<string>("fr", keyValuePairs);
            }
            return textContent;
        }
    }
}
