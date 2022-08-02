using PureVPN.Entity.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Manages dislike reasons
    /// </summary>
    public interface IDislikeReasonsService
    {
        void GetRealtimeChanges();
        /// <summary>
        /// Gets dislike reasons
        /// </summary>
        /// <returns></returns>
        IEnumerable<DislikeReasonModel> GetDislikeReasons();
    }
}
