using NotificationCenter.Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Provides Configuration for Firebase
    /// </summary>
    public class FirebaseConfiguration : IFirebaseConfiguration
    {
        /// <summary>
        /// Query path to listen changes for
        /// </summary>
        public string QueryPath { get; set; }

        /// <summary>
        /// Alrady initialized instance of FirestoreDb
        /// </summary>
        public object FirestoreDb { get; set; }
    }
}