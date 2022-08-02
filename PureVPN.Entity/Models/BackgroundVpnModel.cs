using PureVPN.Entity.Enums;
using PureVPN.Entity.Models.DTO;
using System;

namespace PureVPN.Entity.Models
{
    public class BackgroundVpnModel
    {
        public DateTime ConnectedTime { get; set; }

        public string ConnectedProtocol { get; set; }

    }
}
