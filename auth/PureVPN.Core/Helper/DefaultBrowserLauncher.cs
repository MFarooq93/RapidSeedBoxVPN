using PureVPN.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Helper
{
    /// <summary>
    /// Implementation of <see cref="IBrowserLauncher"/>. Provides methods to communicate with the web browser
    /// </summary>
    public class DefaultBrowserLauncher : IBrowserLauncher
    {
        /// <summary>
        /// Launches default browser with <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        public void OpenURL(string url, bool isCloseable = true)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}