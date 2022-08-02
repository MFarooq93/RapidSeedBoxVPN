using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.RequestParameter
{
    internal class TokenRequestParameter : BaseHeaderParameters
    {
        internal string Code { get; set; }
        internal string RedirectUri { get; set; }
        internal string CodeVerifier { get; set; }
    }
}
