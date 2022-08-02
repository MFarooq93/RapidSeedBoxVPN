using System;
using System.Collections.Generic;
using System.Text;

namespace PureVPN.SpeedTest.Helpers
{
    internal class Constants
    {
        internal static string ExePathDirectory => @"Utility";
        internal static string SpeedTestExeName => "speedtest.exe";
        internal static string Arguments => "--accept-license --accept-gdpr -f json";
    }
}
