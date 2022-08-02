using System;

namespace NotificationCenter.Infrastructure.Models
{
    public interface INotification
    {
        string Id { get; set; }
        string Category { get; set; }
        long CreatedOn { get; set; }
        ClickToActionDTO CtaPrimary { get; set; }
        ClickToActionDTO CtaSecondary { get; set; }
        ClickToActionDTO CtaTertiary { get; set; }
        TextContentDTO Description { get; set; }
        DateTime Expiry { get; set; }
        string Icon { get; set; }
        bool IsRead { get; set; }
        TextContentDTO Title { get; set; }
        string Type { get; set; }

        Notification GetNotification();
    }
}