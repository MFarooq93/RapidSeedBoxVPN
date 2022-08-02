using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public static class Extensions
    {
        /// <summary>
        /// Get value for <paramref name="key"/> from <paramref name="keyValuePairs"/>
        /// </summary>
        /// <typeparam name="T">Type of value for <paramref name="key"/></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static T GetValue<T>(this Dictionary<string, object> keyValuePairs, string key)
        {
            object value;
            if (keyValuePairs.TryGetValue(key, out value) && value is T)
            {
                return (T)value;
            }
            return default(T);
        }
    }
}
