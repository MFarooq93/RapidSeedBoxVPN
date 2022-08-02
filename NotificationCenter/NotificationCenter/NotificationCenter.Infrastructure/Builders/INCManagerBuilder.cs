using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Builders
{
    /// <summary>
    /// Provides functionality to configure and instantiate <see cref="INCManager"/>
    /// </summary>
    public interface INCManagerBuilder
    {
        /// <summary>
        /// Adds <see cref="INotificationLifecycleHandler"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="lifecyleHandler"></param>
        /// <returns></returns>
        INCManagerBuilder WithNotificationLifecycleHandler(INotificationLifecycleHandler lifecyleHandler);
        /// <summary>
        /// Adds <see cref="INotificationUIHandler"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="uiHandler"></param>
        /// <returns></returns>
        INCManagerBuilder WithNotificationUIHandler(INotificationUIHandler uiHandler);

        /// <summary>
        /// Adds <see cref="IFirebaseConfiguration"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        INCManagerBuilder WithFirebase(IFirebaseConfiguration configuration);

        /// <summary>
        /// Adds <see cref="INotificationCenterErrorLogger"/> to <see cref="INCManager"/> instance
        /// </summary>
        /// <param name="errorLogger"></param>
        /// <returns></returns>
        INCManagerBuilder WithErrorLogger(INotificationCenterErrorLogger errorLogger);

        /// <summary>
        /// Creates and returns <see cref="INCManager"/> instance created with provided configuration
        /// </summary>
        /// <returns><see cref="INCManager"/> instance</returns>
        INCManager Build();
    }
}
