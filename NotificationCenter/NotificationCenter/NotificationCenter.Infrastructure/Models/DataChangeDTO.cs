using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Models
{
    /// <summary>
    /// Describes the changed data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataChangeDTO<T>
    {
        /// <summary>
        /// Gets or sets the change type for <see cref="T"/>
        /// </summary>
        public NotificationChangeType NotificationChangeType { get; set; }

        /// <summary>
        /// Data after change
        /// </summary>
        public T Data { get; set; }
    }
}
