using System.Collections.Generic;

namespace PureVPN.Entity.Models.DTO
{
    public class DedicatedIP : BaseNetwork
    {
        public DedicatedIpApiBody body { get; set; }
    }
    

    public class DedicatedIpApiBody
    {
        public DedicatedIpDetail dedicated_ip_detail { get; set; }
    }

    public class DedicatedIpDetail
    {
        public string ip { get; set; }
        public string host { get; set; }

        public string country_iso { get; set; }
        
    }


}
