using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Contracts
{
    /// <summary>
    /// Provides access to <see cref="NotificationDTO"/> data
    /// </summary>
    public interface INotificationDatastore : IDisposable
    {
        /// <summary>
        /// Get or set <see cref="INotificationCenterErrorLogger"/> Error Logger
        /// </summary>
        INotificationCenterErrorLogger ErrorLogger { get; set; }

        /// <summary>
        /// Registers a <paramref name="callback"/> to changes to the notifications data
        /// </summary>
        /// <param name="callback"></param>
        void GetDataChanges(Action<List<DataChangeDTO<NotificationDTO>>> callback);

        /// <summary>
        /// Get all <see cref="NotificationDTO"/>s
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<NotificationDTO>> GetAll();
    }
}
