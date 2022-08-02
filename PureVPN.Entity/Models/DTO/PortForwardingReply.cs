using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public class PortForwardingReply : BaseNetwork
    {
        [JsonProperty("body")]
        public PortForwardingReplyBody Body { get; set; }
    }

    public class PortForwardingReplyBody
    {
        /// <summary>
        /// Describes if all ports are open
        /// </summary>
        [JsonProperty("open_all_ports")]
        public bool IsOpenAllPorts { get; set; }


        /// <summary>
        /// Describes if all ports are block
        /// </summary>
        [JsonProperty("block_all_ports")]
        public bool IsBlockAllPorts { get; set; }

        /// <summary>
        /// Describes if all ports are blocked except some
        /// </summary>
        [JsonProperty("block_all_ports_except")]
        public bool IsBlockAllPortsExcept { get; set; }

        /// <summary>
        /// Describes open port number
        /// </summary>
        [JsonProperty("ports")]
        public string Ports { get; set; }

        /// <summary>
        /// Gets port forwarding status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets expiry date for port forwarding
        /// </summary>
        [JsonProperty("expiry_date")]
        public string ExpiryDate { get; set; }
    }
}
