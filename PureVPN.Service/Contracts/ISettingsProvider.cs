using PureVPN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provides user settings saved at user system
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Username saved to settings to support Remember Me
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Password saved to settings to support Remember Me
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the login method used by the user
        /// </summary>
        string LoginMethod { get; set; }

        /// <summary>
        /// Last successful ServerServicesUrl
        /// </summary>
        string LastServerServicesUrl { get; set; }

        /// <summary>
        /// Last successful AuthTokenUrl
        /// </summary>
        string LastAuthTokenUrl { get; set; }

        /// <summary>
        /// Last successful OAuth2 API Url
        /// </summary>
        string LastOAuth2APIUrl { get; set; }

        /// <summary>
        /// Last successful login redirect web page Url
        /// </summary>
        string LastSuccessfulLoginRedirectWebpageUrl { get; set; }

        /// <summary>
        /// Access token json
        /// </summary>
        string AuthenticationJson { get; set; }

        /// <summary>
        /// Defines user is login with FusionAuth
        /// </summary>
        bool IsUserloginWithFusionAuthAfterUpdate { get; set; }

        void SaveSettings();
    }
}
