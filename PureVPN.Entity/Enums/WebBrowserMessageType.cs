using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    /// <summary>
    /// Represents supported types of messages from web browser
    /// </summary>
    public enum WebBrowserMessageType
    {
        /// <summary>
        /// Login message from browser
        /// </summary>
        login,
        /// <summary>
        /// Logout message from browser
        /// </summary>
        logout,
        /// <summary>
        /// Open url message from browser
        /// </summary>
        open_url,
    }
}
