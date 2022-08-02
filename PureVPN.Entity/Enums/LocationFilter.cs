using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    public enum LocationFilter
    {
        [Description("all")]
        AllServer,
        [Description("tor")]
        TorServer,
        [Description("obfuscated")]
        ObfuscatedServer,
        [Description("p2p")]
        P2PServer,
        [Description("quantum-resistant")]
        QRServer,
        [Description("virtual")]
        VirtualServer
    }
}
