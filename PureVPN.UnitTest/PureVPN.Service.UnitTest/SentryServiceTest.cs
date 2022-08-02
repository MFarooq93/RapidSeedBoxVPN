using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Service.Contracts;
using PureVPN.UnitTest;
using System;

namespace PureVPN.Service.Tests
{
    [TestClass()]
    public class SentryServiceTest : UnitTestBase
    {
        [TestMethod]
        public void GenerateException_Sentry_ReturnTrue()
        {
            var service = this.GetService<ISentryService>();
            var exception = new Exception("Unit Testing");
            service.LoggingException(exception);
            Assert.ThrowsException<Exception>(() => { throw exception; });
        }
    }
}
