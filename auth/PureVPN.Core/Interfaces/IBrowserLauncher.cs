using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Interfaces
{
    /// <summary>
    /// Provides methods to communicate with the web browser
    /// </summary>
    public interface IBrowserLauncher
    {
        /// <summary>
        /// Launches browser with <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        void OpenURL(string url, bool isCloseable = true);
    }
}