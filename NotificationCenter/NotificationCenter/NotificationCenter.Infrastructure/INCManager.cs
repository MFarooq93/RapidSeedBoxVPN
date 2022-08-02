using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NotificationCenter.Infrastructure.Models.EventHandlers;

namespace NotificationCenter.Infrastructure
{
    /// <summary>
    /// Provides methods to configure Notification Center
    /// </summary>
    public interface INCManager:IDisposable
    {
        /// <summary>
        /// Dummy API to establish that consumer can communicate with the Notification Center
        /// </summary>
        /// <returns>A string value of <code>I am here</code></returns>
        string AreYouThere();

        /// <summary>
        /// Initializes the <see cref="INCManager"/>
        /// </summary>
        void Initialize();

        event UnreadCountChanged UnreadCountChanged;
    }
}