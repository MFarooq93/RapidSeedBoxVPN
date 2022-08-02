using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public class ReferAFriend : BaseNetwork
    {
        public ReferAFriendBody body { get; set; }
    }

    public class ReferAFriendBody
    {
        [JsonProperty("invites_available")]
        public int InvitesAvailable { get; set; }

        [JsonProperty("invites_sent")]
        public int InvitesSent { get; set; }

        [JsonProperty("referral_link")]
        public string ReferalLink { get; set; }
    }
}
