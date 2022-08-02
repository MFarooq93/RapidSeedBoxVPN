using Newtonsoft.Json;
using System;
using System.Linq;

namespace PureVPN.Entity.Models
{
    public class DedicatedIPModel
    {
      public string IP { get; set; }
      public string Host { get; set; }

      public string ISO { get; set; }

        public string CountrySlug { get; set; }

        public string Flag
        {
            get { return !string.IsNullOrEmpty(CountrySlug) ? $"pack://application:,,,/assets/flags/v2/{CountrySlug.ToLower()}.png" : ""; }
        }
        public long? Latency { get; set; }
        public string DisplayLatency { get { return Latency > 0 ? $"{Latency} ms" : "-"; } }
        public string CountryName
        {
            get { return AtomModel.Countries?.Where(x => x.CountrySlug == CountrySlug)?.FirstOrDefault()?.Name; }
        }

        public string DedicatedIpAutomationId
        {
            get
            {
                return IP + "_dedicatedip";
            }
        }
        public bool ShowPing { get; set; }
    }
}
