using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class ServerPreference
    {
        public bool GotThumbsUp { get; set; }
        public bool GotThumbsDown { get; set; }
        public string Nasidentifier { get; set; }
        public bool IsConnectedViaCountry { get; set; }
        public string SelectedProtocol { get; set; }
        public string LocationSlug { get; set; }
        public ConnectingFrom ConnectingFrom { get; set; }
    }
}
