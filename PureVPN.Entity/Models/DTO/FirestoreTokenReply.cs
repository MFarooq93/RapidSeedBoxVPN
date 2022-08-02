using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    /// <summary>
    /// Represents Firestore token API response
    /// </summary>
    public class FirestoreTokenReply : BaseNetwork
    {
        [JsonProperty("body")]
        public FirestoreTokenBody Body { get; set; }
    }


    /// <summary>
    /// Body for <see cref="FirestoreTokenReply"/>
    /// </summary>
    public class FirestoreTokenBody
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
