using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PureVPN.Entity.Models
{
    public class CityModel : LocationsBaseModel
    {
        public bool IsActive { get; set; }
        //
        // Summary:
        //     Gets or sets the integer id of this ity. The valid city id is required
        //     for VPN Dialing.
        [JsonIgnore]
        public int Id { get; set; }
        //
        // Summary:
        //     Gets or sets the CountrySlug of the protocol. The valid CountrySlug is required
        //     for VPN Dialing.
        [JsonProperty("country")]
        public string CountrySlug { get; set; }
        //
        // Summary:
        //     Gets or sets the CountryId
        [JsonProperty("country_id")]
        public string CountryId { get; set; }

        //
        // Summary:
        //     Gets or sets the name of the city.
        [JsonProperty("name")]
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the Latency of the city.
        [JsonProperty("latency")]
        public long Latency { get; set; }
        [JsonProperty("displayLatency")]
        public string DisplayLatency { get { return Latency > 0 ? $"{Latency} ms" : "-"; } }
        public string DisplayName
        {
            get { return $"{Name} - ({Latency} ms) "; }
        }
        public string MainCityAutomationId
        {
            get { return $"{Name}_MainCity"; }
        }

        public string MainCityLatencyAutomationId
        {
            get { return $"{Name}_MainCityLatency"; }
        }

        public string FavlocationlistAutomationId
        {
            get { return $"_favlocationlist_{IsFavorite}"; }
        }

        public DateTime DateCreated { get; set; }
        public bool IsFavorite { get; set; }
        public string FavImgSource { get; set; }
        public bool FromTrayIcon { get; set; }
        public string Flag
        {
            get { return !string.IsNullOrEmpty(CountrySlug) ? $"pack://application:,,,/assets/flags/v2/{CountrySlug.ToLower()}.png" : ""; }
        }

        public List<ProtocolModel> Protocols { get; set; } = new List<ProtocolModel>();

        public string NasIdentifier { get; set; }
        public bool GotThumbsUp { get; set; }
        public bool GotThumbsDown { get; set; }

        public bool IsFromDashboardRecentBox { get; set; }

        public bool ShowPing { get; set; }

        public bool IsTorServer { get; set; }

        public bool? AddedWithQRFilter { get; set; }

        public bool? AddedWithObfFilter { get; set; }

        public bool? AddedWithP2pFilter { get; set; }

        public bool? AddedWithTorFilter { get; set; }

        public bool? AddedWithVirtualFilter { get; set; }
    }
}
