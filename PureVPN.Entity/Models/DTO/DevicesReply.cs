using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public class DevicesReply : BaseNetwork
    {
        public SetupDevicesBody body { get; set; }
    }
    public class SetupDevicesBody
    {
        public List<DeviceModel> setup_devices { get; set; }
    }
    public class DeviceModel
    {
        public string title { get; set; }
        public string url { get; set; }
        public string light_x1 { get; set; }
        public string light_x2 { get; set; }
        public string light_x3 { get; set; }
        public string light_x4 { get; set; }
        public string dark_x1 { get; set; }
        public string dark_x2 { get; set; }
        public string dark_x3 { get; set; }
        public string dark_x4 { get; set; }

        
    }
}
