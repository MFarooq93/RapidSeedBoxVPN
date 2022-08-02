using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Service.Helper;
using PureVPN.UnitTest;

namespace PureVPN.Service.Tests
{
    [TestClass()]
    public class UtilitiesServiceTests : UnitTestBase
    {
        [TestMethod()]
        public void Utilities_GetMD5_AreEqual()
        {
            Assert.AreEqual(Utilities.GetMD5("ddc744a1-8dcd-4442-a3c5-e62071274d0c" + "*PVPN123#"), "5478e8c45b9fa3833da9039ed6e8d5cd");
        }


        [TestMethod()]
        public void Utilities_RegisterProgram_AreEqual()
        {
            string AppName = "PureVPN Test";
            string Version = "1.0.0.0";

            Utilities.RegisterProgram(AppName, Version);
            Assert.AreEqual(Utilities.GetLastAppVersion(AppName), Version);
        }

        [TestMethod()]
        public void Utilities_RemoveProgram_AreNotEqual()
        {
            string AppName = "PureVPN Test";
            string Version = "1.0.0.0";

            Utilities.RemoveProgram(AppName);
            Assert.AreNotEqual(Utilities.GetLastAppVersion(AppName), Version);
        }

    }
}