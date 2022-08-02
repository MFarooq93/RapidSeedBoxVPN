using System.Collections.Generic;

namespace PureVPN.Entity.Models.DTO
{
    public class TicketReply : BaseNetwork
    {
        public TicketReplyBody body { get; set; }
    }
    public class TicketReplyBody
    {
        public string status { get; set; }
        public int ticket_id { get; set; }
        public string url { get; set; }

    }

}
