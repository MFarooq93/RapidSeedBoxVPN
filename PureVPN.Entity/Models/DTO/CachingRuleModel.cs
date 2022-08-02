using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    /// <summary>
    /// Describes Caching Rule
    /// </summary>
    public class CachingRuleModel
    {
        /// <summary>
        /// Unique Id for cached content resource
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Url to update cached resourec with <see cref="Id"/>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Duration in minutes to update resource from <see cref="Url"/>
        /// </summary>
        public int CachingDurationInMinutes { get; set; }

        /// <summary>
        /// Describes if content of file is already encrypted
        /// </summary>
        public bool? IsAlreadyEncrypted { get; set; } = false;
    }
}