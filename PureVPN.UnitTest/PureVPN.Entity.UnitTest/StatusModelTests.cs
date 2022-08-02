using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Entity.Models;
using PureVPN.UnitTest;
using PureVPN.Entity.Enums;

namespace PureVPN.Entity.Tests
{
    [TestClass()]
    public class StatusModelTests : UnitTestBase
    {
        [TestMethod()]
        public void StatusModel_CurrentStatus_AreEqual()
        {
            CurrentStatus currentStatus = CurrentStatus.Connected;
            StatusModel status = new StatusModel
            {
                CurrentStatus = currentStatus
            };
            Assert.AreEqual(currentStatus, status.CurrentStatus);
        }

        [TestMethod()]
        public void StatusModel_StatusString_AreEqual()
        {
            string StatusString = "StatusString";
            StatusModel status = new StatusModel
            {
                StatusString = StatusString
            };
            Assert.AreEqual(StatusString, status.StatusString);
        }

        [TestMethod()]
        public void StatusModel_Country_AreEqual()
        {
            string Country = "Country";
            StatusModel status = new StatusModel
            {
                Country = Country
            };
            Assert.AreEqual(Country, status.Country);
        }

        [TestMethod()]
        public void StatusModel_Ip_AreEqual()
        {
            string Ip = "Ip";
            StatusModel status = new StatusModel
            {
                Ip = Ip
            };
            Assert.AreEqual(Ip, status.Ip);
        }

    }
}