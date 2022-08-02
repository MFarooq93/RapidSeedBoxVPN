using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Realtime.Repository.Infrastructure.Models
{
    /// <summary>
    /// Contract to represent data models for Realtime data
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Unique Id for Entity
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Describes whether entity is deleted
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Clone values from <paramref name="keyValuePairs"/>
        /// </summary>
        /// <param name="keyValuePairs"></param>
        void Clone(Dictionary<string, object> keyValuePairs);
    }
}