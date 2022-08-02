using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Manager troubleshoot errors
    /// </summary>
    public interface ITroubleshootErrorService
    {
        void GetRealtimeChanges();
    }
}
