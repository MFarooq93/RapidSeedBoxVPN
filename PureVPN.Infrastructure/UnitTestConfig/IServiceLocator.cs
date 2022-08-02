using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Infrastructure.UnitTestConfig
{
    public interface IServiceLocator
    {
        T Get<T>();

    }
}
