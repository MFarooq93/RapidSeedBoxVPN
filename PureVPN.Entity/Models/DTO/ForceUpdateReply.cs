using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public class ForceUpdateReply : BaseNetwork
    {
        public ForceUpdateReplyBody body { get; set; }
    }

    public class ForceUpdateReplyBody
    {
        public string forced_version { get; set; }
        public string live_version { get; set; }
        public string release_notes_url { get; set; }
    }
}
