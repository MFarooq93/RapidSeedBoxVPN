using System;
using System.Collections.Generic;
using System.Text;

namespace PureVPN.SpeedTest.ErrorHandler
{
    internal class ErrorMessages
    {
        internal static string GetErrorMessage(int errorCode)
        {
            string ret = "";
            
            try
            {
                ret = _errorDictionary[errorCode];
            }
            catch
            {
                ret = "General error - " + errorCode;
            }

            return ret;
        }

        private static Dictionary<int, string> _errorDictionary = new Dictionary<int, string>() 
        {
            { ErrorCode.ProcessIsAlreadyRunning, "Process is already in running state"},
            { ErrorCode.SomethingWentWrong, "Something went wrong please see inner exception"},
            { ErrorCode.DataIsEmpty, "Empty response"}
        };
    }
}
