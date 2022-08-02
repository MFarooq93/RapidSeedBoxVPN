using Caliburn.Micro;
using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Controls.ViewModels
{
    public class DummyViewModel : Screen, INotificationUIHandler, INotificationActionHandler
    {
        public string Locale { get; set; }

        private IObservableCollection<NotificationViewModel> notificationViewModels = new BindableCollection<NotificationViewModel>();

        public event EventHandlers.DismissButtonClicked DismissButtonClickedEvent;
        public event EventHandlers.PrimaryButtonClicked PrimaryButtonClickedEvent;
        public event EventHandlers.SecondaryButtonClicked SecondaryButtonClickedEvent;

        public IObservableCollection<NotificationViewModel> Notifications
        {
            get { return notificationViewModels; }
            set
            {
                notificationViewModels = value;
                NotifyOfPropertyChange(() => Notifications);
                NotifyOfPropertyChange(() => IsShowDescriptionLabel);
            }
        }

        public bool IsShowDescriptionLabel => Notifications == null || Notifications.Count == default(int);

        public void OnDismissClick(NotificationDTO notification)
        {
            DismissButtonClickedEvent?.Invoke(notification);
        }

        public void OnPrimaryActionClick(NotificationDTO notification)
        {
            PrimaryButtonClickedEvent?.Invoke(notification);
        }

        public void OnSecondaryActionClick(NotificationDTO notification)
        {
            SecondaryButtonClickedEvent?.Invoke(notification);
        }

        public void Add(NotificationDTO notification)
        {
            if (notification != null)
            {
                var existing = GetFromViewById(notification);
                if (existing != null)
                {
                    existing.SetValues(notification);
                }
                else
                {
                    Notifications.Insert(default(int), new NotificationViewModel(notification, this, Locale));
                }
            }
            NotifyOfPropertyChange(() => IsShowDescriptionLabel);
        }

        public void Remove(NotificationDTO notification)
        {
            NotificationViewModel existing = GetFromViewById(notification);
            if (existing != null)
            {
                Notifications.Remove(existing);
            }
            NotifyOfPropertyChange(() => IsShowDescriptionLabel);
        }

        public void Add(IEnumerable<NotificationDTO> notifications)
        {
            if (notifications != null)
            {
                notifications = notifications.OrderBy(x => x.CreatedOn);
                foreach (var item in notifications)
                {
                    Add(item);
                }
            }
        }

        public void Clear()
        {
            Notifications?.Clear();
            Notifications = new BindableCollection<NotificationViewModel>();
        }

        private NotificationViewModel GetFromViewById(NotificationDTO notification)
        {
            if (!String.IsNullOrWhiteSpace(notification?.Id))
            {
                return Notifications?.FirstOrDefault(x => x.Notification?.Id == notification?.Id);
            }
            return null;
        }
    }
}