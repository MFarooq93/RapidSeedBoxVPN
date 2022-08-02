using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Entity.Models;
using PureVPN.UnitTest;

namespace PureVPN.Entity.Tests
{
    [TestClass()]
    public class ProtocolModelTests : UnitTestBase
    {
        [TestMethod()]
        public void _ProtocolModel_IsActive_AreEqual()
        {
            bool IsActive = true;
            ProtocolModel protocol = new ProtocolModel
            {
                IsActive = IsActive
            };
            Assert.AreEqual(IsActive, protocol.IsActive);
        }

        [TestMethod()]
        public void ProtocolModel_Id_AreEqual()
        {
            int Id = 5;
            ProtocolModel protocol = new ProtocolModel
            {
                Id = Id
            };
            Assert.AreEqual(Id, protocol.Id);
        }

        [TestMethod()]
        public void ProtocolModel_ProtocolSlug_AreEqual()
        {
            string ProtocolSlug = "protocolSlug";
            ProtocolModel protocol = new ProtocolModel
            {
                ProtocolSlug = ProtocolSlug
            };
            Assert.AreEqual(ProtocolSlug, protocol.ProtocolSlug);
        }
        

        [TestMethod()]
        public void ProtocolModel_Name_AreEqual()
        {
            string Name = "Name";
            ProtocolModel protocol = new ProtocolModel
            {
                Name = Name
            };
            Assert.AreEqual(Name, protocol.Name);
        }

        
    }
}