using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationCenter.Infrastructure;
using NotificationCenter.Infrastructure.Builders;
using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;

namespace PureVPN.Service.Directors
{
    /// <summary>
    /// Director for <see cref="INCManager"/>
    /// </summary>
    public class NCDirector : INotificationInstanceDirector
    {
        ///// <summary>
        ///// <see cref="INCManager"/> instance to be used in the application
        ///// </summary>
        //private static INCManager _NCManager;

        public NCDirector(INCManagerBuilder builder, INotificationCenterErrorLogger notificationCenterErrorLogger)
        {
            Builder = builder;
            NotificationCenterErrorLogger = notificationCenterErrorLogger;
        }

        /// <summary>
        /// Builder provided by Notification Center
        /// </summary>
        public INCManagerBuilder Builder { get; }
        public INotificationCenterErrorLogger NotificationCenterErrorLogger { get; }

        /// <summary>
        /// Get <see cref="INCManager"/>
        /// </summary>
        /// <returns></returns>
        public INCManager GetNCManager()
        {
            var firebaseConfig = new FirebaseConfiguration
            {
                QueryPath = $"users/{AtomModel.UUID}/vpn_accounts/{AtomModel.Username}/notification",
                FirestoreDb = Common.FirestoreDb
            };

            Builder
                .WithNotificationLifecycleHandler(Common.NotificationLifecycleHandler)
                .WithNotificationUIHandler(Common.NotificationUIHandler)
                .WithFirebase(firebaseConfig)
                .WithErrorLogger(NotificationCenterErrorLogger);

            var _NCManager = Builder.Build();
            return _NCManager;
        }
    }
}
