using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;

namespace NotificationCenter.Controls.ViewModels
{
    public class NotificationViewModel : PropertyChangedBase
    {
        public NotificationViewModel(NotificationDTO notification, INotificationActionHandler actionHandler, string locale)
        {
            Notification = notification;
            ActionHandler = actionHandler;
            Locale = locale;
            SetValues(this.Notification);
        }

        public void SetValues(NotificationDTO notification)
        {
            var icon = $"ic_{notification.Category}DrawingImage";
            this.Icon = System.Windows.Application.Current.FindResource(icon) as ImageSource;
            this.NotificationTitle = notification.Title?.GetLocalized(this.Locale);
            this.NotificationDescription = notification.Description?.GetLocalized(this.Locale);
            this.PrimaryButtonText = notification.CtaPrimary?.Title?.GetLocalized(this.Locale);
            this.SecondaryButtonText = notification.CtaSecondary?.Title?.GetLocalized(this.Locale);
            this.NotificationTime = GetDurationDescription(notification.CreatedOn);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                this.BackgroundColor = notification.IsRead ? new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)) : Brushes.White;
            });
        }


        private ImageSource _Icon;
        public ImageSource Icon
        {
            get { return _Icon; }
            set
            {
                _Icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        private Brush _BackgroundColor;
        public Brush BackgroundColor
        {
            get { return _BackgroundColor; }
            set
            {
                _BackgroundColor = value;
                NotifyOfPropertyChange(() => BackgroundColor);
            }
        }


        private string _NotificationTitle;
        public string NotificationTitle
        {
            get { return _NotificationTitle; }
            set
            {
                _NotificationTitle = value;
                NotifyOfPropertyChange(() => NotificationTitle);
            }
        }

        private string _NotificationDescription;
        public string NotificationDescription
        {
            get { return _NotificationDescription; }
            set
            {
                _NotificationDescription = value;
                NotifyOfPropertyChange(() => NotificationDescription);
            }
        }


        private string _NotificationTime;
        public string NotificationTime
        {
            get { return _NotificationTime; }
            set
            {
                _NotificationTime = value;
                NotifyOfPropertyChange(() => NotificationTime);
            }
        }


        private string _PrimaryButtonText;

        public string PrimaryButtonText
        {
            get { return _PrimaryButtonText; }
            set
            {
                _PrimaryButtonText = value;
                NotifyOfPropertyChange(() => PrimaryButtonText);
                NotifyOfPropertyChange(() => IsShowPrimaryButton);
            }
        }


        private string _SecondaryButtonText;

        public string SecondaryButtonText
        {
            get { return _SecondaryButtonText; }
            set
            {
                _SecondaryButtonText = value;
                NotifyOfPropertyChange(() => SecondaryButtonText);
                NotifyOfPropertyChange(() => IsShowSecondaryButton);
            }
        }

        public bool IsShowPrimaryButton => !String.IsNullOrEmpty(PrimaryButtonText);
        public bool IsShowSecondaryButton => !String.IsNullOrEmpty(SecondaryButtonText);

        public NotificationDTO Notification { get; }
        public INotificationActionHandler ActionHandler { get; }
        public string Locale { get; }

        public void PrimaryButton()
        {
            ActionHandler.OnPrimaryActionClick(this.Notification);
        }

        public void SecondaryButton()
        {
            ActionHandler.OnSecondaryActionClick(this.Notification);
        }

        public void DismissButton()
        {
            ActionHandler.OnDismissClick(this.Notification);
        }

        private string GetDurationDescription(long unixTimestamp)
        {
            var createdOn = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            var duration = DateTime.UtcNow - createdOn;

            int days = (int)duration.TotalDays;
            int weeks = days / 7;
            int months = days / 30;
            int years = days / 365;

            if (unixTimestamp <= default(int) || days < default(int))
                return string.Empty;

            if (years > 0)
                return $"{years} year{(years < 2 ? string.Empty : "s")} ago";

            if (months > 0)
                return $"{months} month{(months < 2 ? string.Empty : "s")} ago";

            if (weeks > 0)
                return $"{weeks} week{(weeks < 2 ? string.Empty : "s")} ago";

            if (days > 0)
                return $"{days} day{(days < 2 ? string.Empty : "s")} ago";

            else
                return $"today";
        }
    }
}