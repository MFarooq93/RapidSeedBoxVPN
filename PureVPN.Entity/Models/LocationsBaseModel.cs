using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class LocationsBaseModel
    {
        /// <summary>
        /// Describes whether the location supports <see cref="LocationFeatures.p2p"/>
        /// </summary>
        public bool IsP2P
        {
            get
            {
                return SupportedFeatures.Any(x => x.Equals(LocationFeatures.p2p.ToString(), StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool IsTorServer { get; set; }

        /// <summary>
        /// Describes whether the location supports <see cref="LocationFeatures.QR"/>
        /// </summary>
        public bool IsQrServer
        {
            get
            {
                return SupportedFeatures.Any(x => x.Equals(LocationFeatures.QR.ToString(), StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Describes whether the location supports <see cref="LocationFeatures.OVPN_OBF"/>
        /// </summary>
        public bool IsObfuscatedServer
        {
            get
            {
                return SupportedFeatures.Any(x => x.Equals(LocationFeatures.OVPN_OBF.ToString(), StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// List of Feature names supported by this <see cref="CountryModel"/>
        /// </summary>
        public List<string> SupportedFeatures { get; set; }

        /// <summary>
        /// Describes whether location is virtual or not
        /// </summary>
        public bool IsVirtual { get; set; }
    }
}
