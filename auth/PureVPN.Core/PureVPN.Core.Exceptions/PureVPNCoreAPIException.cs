using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.PureVPN.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when web api has failed
    /// </summary>
    public class PureVPNCoreAPIException : PureVPNCoreException
    {
        internal PureVPNCoreAPIException(int errorCode, string message, Exception innerException = null) : base(errorCode, message, innerException)
        {
        }
    }
}
