using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Service.Helper;
using PureVPN.UnitTest;

namespace PureVPN.Service.Tests
{
    [TestClass()]
    public class CommonServiceTests : UnitTestBase
    {
        //[TestMethod()]
        //public void Common_OSName_IsNotNull()
        //{
        //    Assert.IsNotNull(Common.GetOSName());
        //}

        //[TestMethod()]
        //public void Common_OSName_IsNotNull2()
        //{
        //    Assert.IsNotNull(Common.GetOSName());
        //}

        [TestMethod()]
        public void Common_OSName_IsNotNull_ProductName()
        {
            Assert.IsNotNull(Common.GetOSNameByProductNameKey());
        }

        [TestMethod()]
        public void Common_OSName_IsNotNull_CurrentVersion()
        {
            Assert.IsNotNull(Common.GetOSNameByCurrentVersionKey());
        }

        [TestMethod()]
        public void Common_OSName_IsNotNull_ManagementObjectSearcher()
        {
            Assert.IsNotNull(Common.GetOSNamebyMOS());
        }

        [TestMethod()]
        public void Common_ExeDirectory_IsNotNull()
        {
            Assert.IsNotNull(Common.ExeDirectory);
        }

        [TestMethod()]
        public void Common_ExeName_IsNotNull()
        {
            Assert.IsNotNull(Common.ExeName);
        }

        [TestMethod()]
        public void Common_ExePath_IsNotNull()
        {
            Assert.IsNotNull(Common.ExePath);
        }

        [TestMethod()]
        public void Common_Mixpanel_Properties_IsNotNull()
        {
            Assert.IsNotNull(new MixpanelProperties().MixPanelPropertiesDictionary);
        }

        [TestMethod()]
        public async void Common_CheckHttpTraffic_IsTrue()
        {
            Assert.IsTrue(await Utilities.IsInternetAvailable());
        }

        [TestMethod()]
        public void Common_Environment_IsNotNull()
        {
            string environment = "production";
            Common.Environment = environment;
            Assert.AreEqual(environment, Common.Environment);
        }

    }
}