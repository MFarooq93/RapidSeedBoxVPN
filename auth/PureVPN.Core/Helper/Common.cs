using PureVPN.Core.Exceptions;
using PureVPN.Core.PureVPN.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Helper
{
    internal class Common
    {
        internal static void ThrowPureVPNCoreException(int errorCode, Exception ex = null)
        {
            throw new PureVPNCoreException(errorCode, ErrorMessages.GetErrorMessage(errorCode), ex);
        }
    }
}
