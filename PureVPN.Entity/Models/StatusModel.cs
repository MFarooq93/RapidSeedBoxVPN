using PureVPN.Entity.Enums;
using System.Collections.Generic;

namespace PureVPN.Entity.Models
{
    public class StatusModel
    {
        public CurrentStatus CurrentStatus { get; set; }
        public string StatusString { get; set; }
        public string Country { get; set; }
        public string Flag { get; set; }
        public string City { get; set; }
        public string Ip { get; set; }
        public string atomstatus { get; set; }

        public string ServeIP { get; set; }

        public bool ErrorOccured { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorFrom { get; set; }
        public bool IsFromReconnectOnLaunch { get; set; }

        public bool IsIksEnable { get; set; }

        public bool IsIpUpdate { get; set; }

        public bool IsFavorite { get; set; }

        public string Duration { get; set; }
        public double BytesRecieved { get; set; }
        public double BytesSent { get; set; }
       
        public string MeasureUnitRcv { get; set; }
        public string MeasureUnitSent { get; set; }

        public bool IsFastestServerAvailable { get; set; }

        public bool IsFetchingFastestServer { get; set; }

        public Dictionary<string, object> MixPanelPropertiesDictionary { get; set; }
        public bool IsUTBOccured { get; set; }

    }
}
