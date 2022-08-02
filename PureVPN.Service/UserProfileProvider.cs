using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    /// <summary>
    /// Provides access to user profile
    /// </summary>
    public class UserProfileProvider : IUserProfileProvider
    {
        /// <summary>
        /// Decribes whether user has subscribed to the <paramref name="addon"/>
        /// </summary>
        /// <param name="addon">Slug for Addon</param>
        /// <returns></returns>
        public bool HasAddon(string addon)
        {
            var isExist = AtomModel.Profile?.body?.ActiveAddons?.Any(x =>
                            !string.IsNullOrWhiteSpace(x?.Slug)
                            &&
                            x.Slug.ToLower().Contains(addon.ToLower()));

            return isExist.GetValueOrDefault();
        }
    }
}
