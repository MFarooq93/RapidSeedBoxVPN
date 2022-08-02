using NotificationCenter;
using NotificationCenter.Infrastructure;
using NotificationCenter.Infrastructure.Builders;
using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;
using NotificationCenter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Builders
{
    /// <summary>
    /// Provides functionality to configure and instantiate <see cref="INCManager"/>
    /// </summary>
    public class NCManagerBuilder : INCManagerBuilder
    {
        /// <summary>
        /// Lifecycle handler for <see cref="NotificationDTO"/>
        /// </summary>
        public INotificationLifecycleHandler LifecyleHandler { get; private set; }

        /// <summary>
        /// Datastore to <see cref="NotificationDTO"/>s
        /// </summary>
        public INotificationDatastore NotificationDatastore { get; private set; }
        public IFirebaseConfiguration FirebaseConfiguration { get; private set; }
        public INotificationUIHandler UIHandler { get; private set; }
        public INotificationCenterErrorLogger ErrorLogger { get; private set; }

        /// <summary>
        /// Creates and returns <see cref="NCManager"/> instance created with provided configuration
        /// </summary>
        /// <returns><see cref="NCManager"/> instance</returns>
        public INCManager Build()
        {
            this.NotificationDatastore = new NotificationDatastore(FirebaseConfiguration);
            return new NCManager(
                datastore: this.NotificationDatastore,
                notificationLifecycleHandler: this.LifecyleHandler,
                repository: new NotificationRepository(new DatabaseProvider()),
                notificationUIHandler: this.UIHandler,
                errorLogger: this.ErrorLogger);
        }

        /// <summary>
        /// Adds <see cref="INotificationLifecycleHandler"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="lifecyleHandler"></param>
        /// <returns></returns>
        public INCManagerBuilder WithNotificationLifecycleHandler(INotificationLifecycleHandler lifecyleHandler)
        {
            this.LifecyleHandler = lifecyleHandler;
            return this;
        }

        /// <summary>
        /// Adds <see cref="INotificationUIHandler"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="uiHandler"></param>
        /// <returns></returns>
        public INCManagerBuilder WithNotificationUIHandler(INotificationUIHandler uiHandler)
        {
            this.UIHandler = uiHandler;
            return this;
        }

        /// <summary>
        /// Adds <see cref="INotificationCenterErrorLogger"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="errorLogger"></param>
        /// <returns></returns>
        public INCManagerBuilder WithErrorLogger(INotificationCenterErrorLogger errorLogger)
        {
            this.ErrorLogger = errorLogger;
            return this;
        }

        public INCManagerBuilder WithFirebase(IFirebaseConfiguration configuraiton)
        {
            this.FirebaseConfiguration = configuraiton;
            return this;
        }
    }
}
