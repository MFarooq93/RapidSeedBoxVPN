using PureVPN.SpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class NetworkSpeed
    {
        public double DownloadSpeed { get; set; }
        public double UploadSpeed { get; set; }
        public Server SpeedTestServer { get; set; }
    }
}
