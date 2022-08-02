using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Infrastructure.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.UnitTest
{
    [TestClass()]
    public class UnitTestBase : UnityTestActivator
    {
        public UnitTestBase()
        {
            Infrastructure.ComponentRegister.RegisterContainer();
        }
    }
}
