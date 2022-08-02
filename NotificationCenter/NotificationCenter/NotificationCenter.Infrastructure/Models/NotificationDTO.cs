using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Models
{
    /// <summary>
    /// Represents a Notification
    /// </summary>
    public class NotificationDTO : INotification
    {
        public NotificationDTO()
        {

        }

        public NotificationDTO(Notification notification)
        {
            this.Id = notification.Id;
            this.Category = notification.Category;
            this.Expiry = notification.Expiry;
            this.Icon = notification.Icon;
            this.IsRead = notification.IsRead;
            this.Type = notification.Type;
            this.CreatedOn = notification.CreatedOn;
            this.Title = new TextContentDTO(notification.Title);
            this.Description = new TextContentDTO(notification.Description);
            this.CtaPrimary = new ClickToActionDTO(notification.CtaPrimary);
            this.CtaSecondary = new ClickToActionDTO(notification.CtaSecondary);
            this.CtaTertiary = new ClickToActionDTO(notification.CtaTertiary);
        }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("expiry")]
        public DateTime Expiry { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("is_read")]
        public bool IsRead { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created_on")]
        public long CreatedOn { get; set; }

        [JsonProperty("title")]
        public TextContentDTO Title { get; set; }

        [JsonProperty("description")]
        public TextContentDTO Description { get; set; }

        [JsonProperty("cta_primary")]
        public ClickToActionDTO CtaPrimary { get; set; }

        [JsonProperty("cta_secondary")]
        public ClickToActionDTO CtaSecondary { get; set; }

        [JsonProperty("cta_tertiary")]
        public ClickToActionDTO CtaTertiary { get; set; }

        public Notification GetNotification()
        {
            var notification = new Notification();

            notification.Id = this.Id;
            notification.Category = this.Category;
            notification.Expiry = this.Expiry;
            notification.Icon = this.Icon;
            notification.IsRead = this.IsRead;
            notification.Type = this.Type;
            notification.CreatedOn = this.CreatedOn;
            notification.Title = this.Title?.GetTextContent();
            notification.Description = this.Description?.GetTextContent();
            notification.CtaPrimary = this.CtaPrimary?.GetClickToAction();
            notification.CtaSecondary = this.CtaSecondary?.GetClickToAction();
            notification.CtaTertiary = this.CtaTertiary?.GetClickToAction();

            return notification;
        }
    }

    public class ClickToActionDTO
    {
        public ClickToActionDTO()
        {

        }

        public ClickToActionDTO(ClickToAction clickToAction)
        {
            this.Title = new TextContentDTO(clickToAction.Title);
            this.Action = clickToAction.Action;
            this.Destination = clickToAction.Destination;
        }

        [JsonProperty("title")]
        public TextContentDTO Title { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        public ClickToAction GetClickToAction()
        {
            var action = new ClickToAction();
            action.Action = this.Action;
            action.Destination = this.Destination;
            action.Title = this.Title?.GetTextContent();
            return action;
        }
    }

    public class TextContentDTO
    {
        public TextContentDTO()
        {

        }

        public TextContentDTO(TextContent textContent)
        {
            this.En = textContent.En;
            this.Fr = textContent.Fr;
            this.De = textContent.De;
        }

        [JsonProperty("en")]
        public string En { get; set; }

        [JsonProperty("fr")]
        public string Fr { get; set; }

        [JsonProperty("de")]
        public string De { get; set; }

        public TextContent GetTextContent()
        {
            var content = new TextContent();
            content.En = this.En;
            content.Fr = this.Fr;
            content.De = this.De;
            return content;
        }

        /// <summary>
        /// Get Localized text for <paramref name="locale"/>
        /// </summary>
        /// <param name="locale">locale</param>
        /// <returns></returns>
        public string GetLocalized(string locale)
        {
            string text = null;
            switch (locale)
            {
                case "fr":
                    text = Fr;
                    break;
                case "de":
                    text = De;
                    break;
            }
            return String.IsNullOrWhiteSpace(text) ? En : text;
        }
    }
}
