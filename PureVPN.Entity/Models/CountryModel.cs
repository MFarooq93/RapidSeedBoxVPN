using Newtonsoft.Json;
using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PureVPN.Entity.Models
{
    public class CountryModel : LocationsBaseModel
    {
        public bool IsActive { get; set; }
        //
        // Summary:
        //     Gets or sets the integer id of this country. The valid Country id is required
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
        //     Gets or sets the ISO Alpha-2 Country code of the current country.
        [JsonProperty("iso_code")]
        public string ISOCode { get; set; }

        //
        // Summary:
        //     Gets or sets the name of the country.
        [JsonProperty("name")]
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the latitude of the country.
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        //
        // Summary:
        //     Gets or sets the logitude of the country.
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        //
        // Summary:
        //     Gets or sets the Latency of the country.
        [JsonProperty("latency")]
        public long Latency { get; set; }
        [JsonProperty("displayLatency")]
        public string DisplayLatency { get { return Latency > 0 ? $"{Latency} ms" : "-"; } }

        public string DisplayName
        {
            get { return $"{Name} - ({Latency} ms) "; }
        }

        public DateTime DateCreated { get; set; }
        public bool IsFavorite { get; set; }
        public string FavImgSource { get; set; }

        public string FavlocationlistAutomationId
        {
            get { return $"_favlocationlist_{IsFavorite}"; }
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

        public string MainCountryLatencyAutomationId
        {
            get { return $"{Name}_MainCountryLatency"; }
        }

        public string Flag
        {
            get { return !string.IsNullOrEmpty(CountrySlug) ? $"pack://application:,,,/assets/flags/v2/{CountrySlug.ToLower()}.png" : ""; }
        }

        public int Rank { get; set; }

        public bool FromTrayIcon { get; set; }
        public List<ProtocolModel> Protocols { get; set; } = new List<ProtocolModel>();

        public string NasIdentifier { get; set; }
        public bool GotThumbsUp { get; set; }
        public bool GotThumbsDown { get; set; }

        public bool IsRecommendedCountry { get; set; }

        public bool IsFromDashboardRecentBox { get; set; }

        public bool ShowPing { get; set; }

        public bool? AddedWithQRFilter { get; set; }

        public bool? AddedWithObfFilter { get; set; }

        public bool? AddedWithP2pFilter { get; set; }

        public bool? AddedWithTorFilter { get; set; }

        public bool? AddedWithVirtualFilter { get; set; }

    }
}