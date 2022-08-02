using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    /// <summary>
    /// Describes features on location
    /// </summary>
    public enum LocationFeatures
    {
        /// <summary>
        /// Describes that the location supports Peer to Peer Connection feature
        /// </summary>
        p2p,
        /// <summary>
        /// Describes that the location supports Quantum Resistant feature
        /// </summary>
        QR,
        /// <summary>
        /// Describes that the location supports OVPN Obfuscation feature
        /// </summary>
        OVPN_OBF
    }
}