using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.RequestParameter
{
    class RefreshTokenRequestParameter : BaseHeaderParameters
    {
        internal string RefreshToken { get; set; }
    }
}
