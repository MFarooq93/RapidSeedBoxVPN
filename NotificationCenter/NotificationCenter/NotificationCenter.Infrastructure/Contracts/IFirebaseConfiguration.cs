using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Contracts
{
    /// <summary>
    /// Configuration to setup Notification Center with Firebase Cloud Firestore
    /// </summary>
    public interface IFirebaseConfiguration
    {
        /// <summary>
        /// Cloud Firestore Query path to get user notifications
        /// </summary>
        string QueryPath { get; set; }

        /// <summary>
        /// Already initialized <see cref="FirestoreDb"/> instance with access to the Notifications collection
        /// </summary>
        object FirestoreDb { get; set; }
    }
}
