using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public  class MigrateTokenReply : BaseNetwork
    {
       public MigrateTokenReplyBody body { get; set; }

    }
    public class MigrateTokenReplyBody 
    {
        public string refreshToken { get; set; }
        public string token { get; set; }
    }
}
