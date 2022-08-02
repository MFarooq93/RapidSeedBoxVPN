using PureVPN.SpeedTest.ErrorHandler;
using PureVPN.SpeedTest.Models;
using System.Collections.Generic;

namespace PureVPN.SpeedTest.Delegates
{
    public delegate void DataReceived(object sender, SpeedTestResult speedTest, string identifier);
    public delegate void ErrorDataReceived(object sender, SpeedTestException exception, string identifier);
}
