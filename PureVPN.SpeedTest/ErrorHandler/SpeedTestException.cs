using System;
using System.Collections.Generic;
using System.Text;

namespace PureVPN.SpeedTest.ErrorHandler
{
    public class SpeedTestException: Exception
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public SpeedTestException(int errorCode, string message, Exception innerException = null) : base($"{errorCode} - {message}", innerException)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
        }
    }
}
