using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public class ApiResponseMetrics
    {
        public double ResponseTime { get; set; }
        public string SuccessfulBaseUrl { get; set; }
        public string ResponseBody { get; set; }
        public string Host
        {
            get
            {
                try
                {
                    var uri = new Uri(SuccessfulBaseUrl);
                    return uri.Host;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
