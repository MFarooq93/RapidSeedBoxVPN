using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provides access to user profile
    /// </summary>
    public interface IUserProfileProvider
    {
        /// <summary>
        /// Decribes whether user has subscribed to the <paramref name="addon"/>
        /// </summary>
        /// <param name="addon">Slug for Addon</param>
        /// <returns></returns>
        bool HasAddon(string addon);
    }
}
