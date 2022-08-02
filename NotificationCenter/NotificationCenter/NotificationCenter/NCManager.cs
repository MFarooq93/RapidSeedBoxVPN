using NotificationCenter.Infrastructure;
using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;
using NotificationCenter.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NotificationCenter
{
    /// <summary>
    /// Provides methods to configure Notification Center
    /// </summary>
    public class NCManager : INCManager
    {
        public NCManager(INotificationDatastore datastore, INotificationLifecycleHandler notificationLifecycleHandler, INotificationRepository repository, INotificationUIHandler notificationUIHandler, INotificationCenterErrorLogger errorLogger)
        {
            this.Datastore = datastore;
            this.Datastore.ErrorLogger = errorLogger;
            this.NotificationLifecycleHandler = notificationLifecycleHandler;
            this.Repository = repository;
            this.NotificationUIHandler = notificationUIHandler;
            this.ErrorLogger = errorLogger;
            this.NotificationUIHandler.DismissButtonClickedEvent += NotificationUIHandler_DismissButtonClickedEvent;
            this.NotificationUIHandler.PrimaryButtonClickedEvent += NotificationUIHandler_PrimaryButtonClickedEvent;
            this.NotificationUIHandler.SecondaryButtonClickedEvent += NotificationUIHandler_SecondaryButtonClickedEvent;
        }

        public event EventHandlers.UnreadCountChanged UnreadCountChanged;

        private void NotificationUIHandler_SecondaryButtonClickedEvent(NotificationDTO notification)
        {
            this.NotificationLifecycleHandler.OnActionPerformed(NotificationActionType.NotificationAction, notification, notification?.CtaSecondary);
        }

        private void NotificationUIHandler_PrimaryButtonClickedEvent(NotificationDTO notification)
        {
            this.NotificationLifecycleHandler.OnActionPerformed(NotificationActionType.NotificationAction, notification, notification?.CtaPrimary);
        }

        private void NotificationUIHandler_DismissButtonClickedEvent(NotificationDTO notification)
        {
            this.NotificationUIHandler.Remove(notification);
            this.NotificationLifecycleHandler.OnActionPerformed(NotificationActionType.NotificationDismiss, notification, null);
        }

        public INotificationDatastore Datastore { get; }
        public INotificationLifecycleHandler NotificationLifecycleHandler { get; }
        public INotificationRepository Repository { get; }
        public INotificationUIHandler NotificationUIHandler { get; }
        public INotificationCenterErrorLogger ErrorLogger { get; }

        /// <summary>
        /// Initialize <see cref="NCManager"/>
        /// </summary>
        public async void Initialize()
        {
            List<NotificationDTO> exisingNotificationsDTO = null;
            var exisingNotifications = this.Repository.GetAll();
            if (exisingNotifications != null)
            {
                exisingNotificationsDTO = exisingNotifications.Select(x => new NotificationDTO(x)).ToList();
            }
            this.NotificationUIHandler.Add(exisingNotificationsDTO);
            UpdateUnreadCount();

            var notifications = await this.Datastore.GetAll();
            NotificationUIHandler.Add(notifications);

            if (exisingNotificationsDTO != null)
            {
                foreach (var item in exisingNotificationsDTO)
                {
                    var isExistInNew = notifications?.Any(x => x.Id == item.Id);
                    if (isExistInNew == false)
                    {
                        this.NotificationUIHandler.Remove(item);
                        this.Repository.Remove(item.Id);
                    }
                }
            }

            this.Repository.OnChange(OnLocalChangeCallback);
            if (notifications != null && notifications.Count() > default(int))
            {
                this.Repository.TruncateAndDump(notifications.Select(x => x.GetNotification()));
            }
            UpdateUnreadCount();
            this.Datastore.GetDataChanges(OnChangeCallback);
        }

        public void Dispose()
        {
            try
            {
                this.Datastore.Dispose();
                this.Repository.TruncateAndDump(new Notification[] { });
                this.Repository.Dispose();
                this.NotificationUIHandler.Clear();
                this.UpdateUnreadCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }

        /// <summary>
        /// Callback for changes to <see cref="NotificationDTO"/>s data from Remote database
        /// </summary>
        private Action<List<DataChangeDTO<NotificationDTO>>> OnChangeCallback => (notifications) =>
        {
            if (notifications == null || notifications.Count <= default(int))
            {
                return;
            }
            foreach (var item in notifications)
            {
                try
                {
                    if (item == null || String.IsNullOrWhiteSpace(item.Data?.Id))
                    {
                        continue;
                    }
                    switch (item.NotificationChangeType)
                    {
                        case NotificationChangeType.Added:
                        case NotificationChangeType.Updated:
                            this.NotificationUIHandler.Add(item.Data);
                            this.NotificationLifecycleHandler.OnNotificationReceived(item.Data);
                            this.Repository.Upsert(item.Data?.GetNotification());
                            break;
                        case NotificationChangeType.Deleted:
                            this.NotificationUIHandler.Remove(item.Data);
                            this.Repository.Remove(item.Data.Id);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger?.LogError(ex);
                }
            }
            UpdateUnreadCount();
        };

        /// <summary>
        /// Callback to handle changes to local database
        /// </summary>
        private Action<List<DataChangeDTO<Notification>>> OnLocalChangeCallback => (notifications) =>
        {
            //this.NotificationUIHandler.Add(notifications.Select(x => x.Data).ToList());
        };

        /// <summary>
        /// Dummy API to establish that consumer can communicate with the Notification Center
        /// </summary>
        /// <returns>A string value of <code>I am here</code></returns>
        public string AreYouThere()
        {
            return "I am here";
        }

        public void UpdateUnreadCount()
        {
            try
            {
                var unreadCount = Repository.GetUnreadCount();
                UnreadCountChanged?.Invoke(unreadCount);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }
    }
}