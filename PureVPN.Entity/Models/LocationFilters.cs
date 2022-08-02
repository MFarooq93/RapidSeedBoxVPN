using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class LocationFilters
    {
        public bool? IsQRFilter { get; set; }
        public bool? IsObfFilter { get; set; }
        public bool? IsP2PFilter { get; set; }
    }
}
