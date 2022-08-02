
namespace PureVPN.Entity.Models.DTO
{
    public class BaseNetwork
    {
        public Header header { get; set; }
    }

    public class Header
    {
        public int response_code { get; set; }
        public string message { get; set; }
        public string version { get; set; }
    }
}
