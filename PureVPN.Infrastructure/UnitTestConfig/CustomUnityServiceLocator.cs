using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace PureVPN.Infrastructure.UnitTestConfig
{
    public class CustomUnityServiceLocator : ServiceLocator<IUnityContainer>
    {
        public CustomUnityServiceLocator(IUnityContainer container)
            : base(container)
        {  }

        // Override base method in order to get service instance based on container specific logic
        protected override T Get<T>(IUnityContainer container)
        {

            return this.Container.Resolve<T>();
        }
    }
}
