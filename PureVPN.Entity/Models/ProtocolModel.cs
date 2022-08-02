using Newtonsoft.Json;

namespace PureVPN.Entity.Models
{
    public class ProtocolModel
    {
        public ProtocolModel()
        {
            Id = 0;
            Name = "Automatic";
            ProtocolSlug = "IKEv2";
            IsActive = true;
        }

        public bool IsActive { get; set; }
        //
        // Summary:
        //     Gets or sets the integer id of the protocol.
        [JsonIgnore]
        public int Id { get; set; }
        //
        // Summary:
        //     Gets or sets the ProtocolSlug of the protocol. The valid ProtocolSlug is required
        //     for VPN Dialing.
        [JsonProperty("protocol")]
        public string ProtocolSlug { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the protocol.
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }
    }
}
