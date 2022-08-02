using PureVPN.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.RequestParameter
{
    internal class BaseHeaderParameters
    {
        internal string GrantType { get; set; }
        internal string ClientID => CommonKeys.ClientID;
    }
}
