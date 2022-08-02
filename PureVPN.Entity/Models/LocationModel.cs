using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PureVPN.Entity.Models
{
    public class LocationModel
    {
        public bool IsActive { get; set; }

        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("country")]
        public string CountrySlug { get; set; }

        [JsonProperty("iso_code")]
        public string ISOCode { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }


        [JsonProperty("latency")]
        public long Latency { get; set; }
        [JsonProperty("displayLatency")]
        public string DisplayLatency { get { return Latency > 0 ? $"{Latency} ms" : "-"; } }

        public string DisplayName
        {
            get { return $"{Name} - ({Latency} ms) "; }
        }
        public string DisplayNameWithSlug
        {
            get { return IsCity ? $"{Name}, {CountrySlug}" : Name; }
        }
        public DateTime DateCreated { get; set; }
        public bool IsFavorite { get; set; }
        public string FavImgSource { get; set; }

        public bool IsCity { get; set; }


        public string Flag
        {
            get { return !string.IsNullOrEmpty(CountrySlug) ? $"pack://application:,,,/assets/flags/v2/{CountrySlug.ToLower()}.png" : ""; }
        }

        public string FavCountryAutomationId
        {
            get { return $"{Name}_Fav"; }
        }
        public string FavRecentCountryAutomationId
        {
            get { return $"{Name}_RecFav"; }
        }
        public string MainCountryListAutomationId
        {
            get { return $"{Name}_MainList"; }
        }
        public string MarkRemoveFavCountryAutomationId
        {
            get { return $"{Name}_Fav_{IsFavorite}"; }
        }
        public string MarkRemoveFavRecentCountryAutomationId
        {
            get { return $"{Name}_RecFav_{IsFavorite}"; }
        }
        public string FastestServerAutomationId
        {
            get { return $"{Name}_FastestServer"; }
        }
        public string FastestServerMarkRemoveFavAutomationId
        {
            get { return $"{Name}_FavFastestServer_{IsFavorite}"; }
        }
        public bool IsQrServer { get; set; }
        public bool IsObfuscatedServer { get; set; }
        public bool IsTorServer { get; set; }
        public bool IsP2P { get; set; }
        public bool IsVirtual { get; set; }
        public int Rank { get; set; }

        public string Ip { get; set; }

        public CityModel City { get; set; }

        public List<ProtocolModel> Protocols { get; set; } = new List<ProtocolModel>();

        public string NasIdentifier { get; set; }
        public bool GotThumbsUp { get; set; }
        public bool GotThumbsDown { get; set; }
        public bool FromTrayIcon { get; set; }
        public bool IsFromDashboardRecentBox { get; set; }

        public bool ProtocolSupported { get; set; }

        public bool ShowPing { get; set; }

        public List<Entity.Enums.LocationFilter> ActiveFiltersWhenAdded { get; set; }

        public bool? AddedWithQRFilter { get; set; }

        public bool? AddedWithObfFilter { get; set; }

        public bool? AddedWithP2pFilter { get; set; }

        public bool? AddedWithTorFilter { get; set; }

        public bool? AddedWithVirtualFilter { get; set; }

        public bool SetToRemoveFromFav { get; set; } = false;
    }
}
