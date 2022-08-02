using NotificationCenter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Director for <see cref="INCManager"/>
    /// </summary>
    public interface INotificationInstanceDirector
    {
        /// <summary>
        /// Get <see cref="INCManager"/>
        /// </summary>
        /// <returns></returns>
        INCManager GetNCManager();
    }
}
