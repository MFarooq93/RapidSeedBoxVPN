using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.PureVPN.Core.Exceptions
{
    public class PureVPNCoreException : Exception
    {
        /// <summary>
        /// Gets the error code of the error that caused the exception.
        /// </summary>        
        /// <returns>
        /// A int error code.
        /// </returns>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Gets the error message of the error that caused the exception.
        /// </summary>  
        public string ErrorMessage { get; private set; }

        internal PureVPNCoreException(int errorCode, string message, Exception innerException = null) : base($"{errorCode} - {message}", innerException)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
        }
    }
}
