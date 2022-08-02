using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Infrastructure.UnitTestConfig
{
    public abstract class ServiceLocator<TContainer> : IServiceLocator
    {
        // DI container
        protected TContainer Container { get; private set; }

        protected ServiceLocator(TContainer container)
        {
            Container = container; 
        }

        public virtual T Get<T>()
        {
            return Get<T>(Container);
        }

        // Get service instance based on container specific logic
        protected abstract T Get<T>(TContainer container);
    }
}
