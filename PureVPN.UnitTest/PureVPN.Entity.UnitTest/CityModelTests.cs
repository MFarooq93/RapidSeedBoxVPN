using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Entity.Models;
using PureVPN.UnitTest;

namespace PureVPN.Entity.Tests
{
    [TestClass()]
    public class CityModelTests : UnitTestBase
    {
        [TestMethod()]
        public void CityModel_IsActive_AreEqual()
        {
            bool IsActive = true;
            CityModel city = new CityModel
            {
                IsActive = IsActive
            };
            Assert.AreEqual(IsActive, city.IsActive);
        }

        [TestMethod()]
        public void CityModel_Id_AreEqual()
        {
            int Id = 6;
            CityModel city = new CityModel
            {
                Id = Id
            };
            Assert.AreEqual(Id, city.Id);
        }

        [TestMethod()]
        public void CityModel_CountrySlug_AreEqual()
        {
            string CountrySlug = "countrySlug";
            CityModel city = new CityModel
            {
                CountrySlug = CountrySlug
            };
            Assert.AreEqual(CountrySlug, city.CountrySlug);
        }

        [TestMethod()]
        public void CityModel_CountryId_AreEqual()
        {
            string CountryId = "CountryId";
            CityModel city = new CityModel
            {
                CountryId = CountryId
            };
            Assert.AreEqual(CountryId, city.CountryId);
        }

        [TestMethod()]
        public void CityModel_Name_AreEqual()
        {
            string Name = "Name";
            CityModel city = new CityModel
            {
                Name = Name
            };
            Assert.AreEqual(Name, city.Name);
        }

        [TestMethod()]
        public void CityModel_Latency_AreEqual()
        {
            long Latency = 99;
            CityModel city = new CityModel
            {
                Latency = Latency
            };
            Assert.AreEqual(Latency, city.Latency);
        }

        [TestMethod()]
        public void CityModel_DisplayName_AreEqual()
        {
            long Latency = 99;
            string Name = "Name";
            CityModel city = new CityModel
            {
                Name = Name,
                Latency = Latency
            };
            Assert.AreEqual(Name + " - (" + Latency + " ms) ", city.DisplayName);
        }
    }
}