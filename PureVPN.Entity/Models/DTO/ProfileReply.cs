using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{

    public partial class Body
    {
        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("signup_date")]
        public DateTimeOffset SignupDate { get; set; }

        [JsonProperty("billing_cycle")]
        public string BillingCycle { get; set; }
        
        [JsonProperty("expiry_date")]
        public string ExpiryDate { get; set; }

        [JsonProperty("grace_period")]
        public bool GracePeriod { get; set; }

        [JsonProperty("grace_period_expiry")]
        public string GracePeriodExpiry { get; set; }

        [JsonProperty("days_remaining")]
        public int DaysRemaining { get; set; }

        [JsonProperty("billing_plan")]
        public string BillingPlan { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("client_type")]
        public string ClientType { get; set; }

        [JsonProperty("hosting_id")]
        public string HostingId { get; set; }

        [JsonProperty("payment_gateway")]
        public PaymentGateway PaymentGateway { get; set; }

        [JsonProperty("active_addons")]
        public List<ActiveAddon> ActiveAddons { get; set; }

        [JsonProperty("desired_outcome")]
        public int DesiredOutcome { get; set; }

        [JsonProperty("password_manager")]
        public bool PasswordManager { get; set; }

        [JsonProperty("referral_link")]
        public string ReferralLink { get; set; }

    }

    public partial class ActiveAddon
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("expiry")]
        public string Expiry { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        //   public string AddonText { get { return $"{Title} (Expires On {Expiry.ToString("dd MMMM, yyyy")})"; } }
        public string AddonText { get; set; }
        public string AddonText_AutomationId
        {
            get; set;
        }

    }

    public partial class PaymentGateway
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("recurring_gateway")]
        public long RecurringGateway { get; set; }

        [JsonProperty("auto_recurring")]
        public long AutoRecurring { get; set; }

        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }
    }

    public class ProfileReply : BaseNetwork
    {
        public Body body { get; set; }
    }


}
