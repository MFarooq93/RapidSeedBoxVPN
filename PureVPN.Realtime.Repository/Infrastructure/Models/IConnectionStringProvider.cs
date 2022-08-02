using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Realtime.Repository.Infrastructure.Models
{
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Provides Custom token for User
        /// </summary>
        /// <returns></returns>
        Task<string> GetCustomTokenAsync();

        /// <summary>
        /// Firebase API Key
        /// </summary>
        string APIKey { get; }

        /// <summary>
        /// Firebase Project Id
        /// </summary>
        string ProjectId { get; }
    }
}
