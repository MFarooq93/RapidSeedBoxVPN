using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Models
{
    public class Subscription
    {
        [JsonProperty("billingCycle")]
        public string BillingCycle { get; set; }

        [JsonProperty("expiry")]
        public string Expiry { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("vpnusername")]
        public string VPNUsername { get; set; }

        [JsonProperty("paymentGateway")]
        public string PaymentGateway { get; set; }

        [JsonProperty("expiryReason")]
        public string ExpiryReason { get; set; }
    }
}