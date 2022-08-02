using PureVPN.Entity.Enums;


namespace PureVPN.Entity.Models
{
    public class AfterConnectionModel
    { 
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string ServeIP { get; set; }
        public string NasIdentifier { get; set; }
        public string ProtocolSlug { get; set; }
        public string ServerAddress { get; set; }
        public string ServerType { get; set; }
        public bool MarkedFavorite { get; set; }
        public bool GotThumbsUp { get; set; }
        public bool GotThumbsDown { get; set; }
        public CountryModel Country { get; set; }
        public CityModel City { get; set; }
        public ConnectingFrom ConnectionType { get; set; }

        
    }
}
