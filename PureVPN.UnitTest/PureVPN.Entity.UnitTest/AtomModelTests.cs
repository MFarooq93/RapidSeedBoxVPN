using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using PureVPN.UnitTest;
using System.Threading.Tasks;
using Atom.Core.Models;
using System.Collections.Generic;
using Moq;
using PureVPN.Entity.Enums;

namespace PureVPN.Entity.Tests
{
    [TestClass()]
    public class AtomModelTests : UnitTestBase
    {
        [TestMethod()]
        public void AtomModel_SecretKey_AreEqual()
        {
            string SecretKey = "1";
            AtomModel.SecretKey = SecretKey;
            Assert.AreEqual(SecretKey, AtomModel.SecretKey);
        }

        [TestMethod()]
        public void AtomModel_Username_AreEqual()
        {
            string Username = "2";
            AtomModel.Username = Username;
            Assert.AreEqual(Username, AtomModel.Username);
        }

        [TestMethod()]
        public void AtomModel_Password_AreEqual()
        {
            string Password = "3";
            AtomModel.Password = Password;
            Assert.AreEqual(Password, AtomModel.Password);
        }

        [TestMethod()]
        public void AtomModel_Token_AreEqual()
        {
            string Token = "4";
            AtomModel.Token = Token;
            Assert.AreEqual(Token, AtomModel.Token);
        }

        [TestMethod()]
        public void AtomModel_UUID_AreEqual()
        {
            string UUID = "5";
            AtomModel.UUID = UUID;
            Assert.AreEqual(UUID, AtomModel.UUID);
        }

        [TestMethod()]
        public void AtomModel_IsDisconnected_AreEqual()
        {
            bool IsDisconnected = true;
            Assert.AreEqual(IsDisconnected, AtomModel.IsDisconnected);
        }

        [TestMethod()]
        public void AtomModel_IsConnected_AreEqual()
        {
            bool IsConnected = true;
            AtomModel.IsConnected = IsConnected;
            Assert.AreEqual(IsConnected, AtomModel.IsConnected);
        }

        [TestMethod()]
        public void AtomModel_IsConnecting_AreEqual()
        {
            bool IsConnecting = true;
            AtomModel.IsConnecting = IsConnecting;
            Assert.AreEqual(IsConnecting, AtomModel.IsConnecting);
        }

        [TestMethod()]
        public void AtomModel_IsSDKInitializing_AreEqual()
        {
            bool IsSDKInitializing = true;
            AtomModel.IsSDKInitializing = IsSDKInitializing;
            Assert.AreEqual(IsSDKInitializing, AtomModel.IsSDKInitializing);
        }

        [TestMethod()]
        public void AtomModel_ISSDKInitialized_AreEqual()
        {
            bool ISSDKInitialized = true;
            AtomModel.ISSDKInitialized = ISSDKInitialized;
            Assert.AreEqual(ISSDKInitialized, AtomModel.ISSDKInitialized);
        }

        [TestMethod()]
        public void AtomModel_IsLoggedIn_AreEqual()
        {
            bool IsLoggedIn = true;
            AtomModel.IsLoggedIn = IsLoggedIn;
            Assert.AreEqual(IsLoggedIn, AtomModel.IsLoggedIn);
        }

        [TestMethod()]
        public void AtomModel_IsEventsRegister_AreEqual()
        {
            bool IsEventsRegister = true;
            AtomModel.IsEventsRegister = IsEventsRegister;
            Assert.AreEqual(IsEventsRegister, AtomModel.IsEventsRegister);
        }

        [TestMethod()]
        public async Task AtomModel_Countries_IsNotNull()
        {
            List<Country> MockCountries = new List<Country> { new Country { CountrySlug = "UK" }, new Country { CountrySlug = "US" }, new Country { CountrySlug = "PK" } };
            var service = this.GetService<IAtomService>();
            var MockAtomService = new Mock<IAtomService>();
            MockAtomService.Setup(x => x.GetAtomCountries()).Returns(Task.FromResult(MockCountries));
            AtomModel.Countries = service.MapAtomCountries(await MockAtomService.Object.GetAtomCountries());
            Assert.IsNotNull(AtomModel.Countries[0].CountrySlug);
        }

        [TestMethod()]
        public async Task AtomModel_CountriesRecent_IsNotNull()
        {
            List<Country> MockCountries = new List<Country> { new Country { CountrySlug = "UK" }, new Country { CountrySlug = "US" }, new Country { CountrySlug = "PK" } };
            var service = this.GetService<IAtomService>();
            var MockAtomService = new Mock<IAtomService>();
            MockAtomService.Setup(x => x.GetAtomCountries()).Returns(Task.FromResult(MockCountries));
            AtomModel.CountriesRecent = service.MapAtomCountries(await MockAtomService.Object.GetAtomCountries());
            Assert.IsNotNull(AtomModel.CountriesRecent[0].CountrySlug);
        }

        [TestMethod()]
        public async Task AtomModel_Cities_IsNotNull()
        {
            List<City> MockCities = new List<City> { new City { Name = "Seattle", Country = new Country { Name = "US", CountrySlug = "US", ISOCode = "US", Id = 2, IsActive = true, Latency = 100 }, Latency = 122, IsActive = true, CountrySlug = "US", CountryId = "2", Id = 5 } };
            var service = this.GetService<IAtomService>();
            var MockAtomService = new Mock<IAtomService>();
            MockAtomService.Setup(x => x.GetAtomCities()).Returns(Task.FromResult(MockCities));
            AtomModel.Cities = service.MapAtomCities(await MockAtomService.Object.GetAtomCities());
            Assert.IsNotNull(AtomModel.Cities[0].Name);
        }

        //[TestMethod()]
        //public async Task AtomModel_Protocols_IsNotNull()
        //{
        //    List<Protocol> MockProtocols = new List<Protocol> { new Protocol { Name = "ikev2", ProtocolSlug = "ikev2" }, new Protocol { Name = "tcp/udp", ProtocolSlug = "tcp/udp" }, new Protocol { Name = "pptp", ProtocolSlug = "pptp" } };
        //    var service = this.GetService<IAtomService>();
        //    var MockAtomService = new Mock<IAtomService>();
        //    MockAtomService.Setup(x => x.GetAtomProtocols()).Returns(Task.FromResult(MockProtocols));
        //    AtomModel.Protocols = service.MapAtomProtocols(await MockAtomService.Object.GetAtomProtocols());
        //    Assert.IsNotNull(AtomModel.Protocols[0].Name);
        //}

        [TestMethod()]
        public void AtomModel_ConnectedTo_AreEqual()
        {
            ConnectedTo connectedTo = ConnectedTo.NotConnected;
            AtomModel.ConnectedTo = connectedTo;
            Assert.AreEqual(connectedTo, AtomModel.ConnectedTo);
        }

        [TestMethod()]
        public void AtomModel_Instance_IsNotNull()
        {
            Assert.IsNotNull(AtomModel.Instance);
        }
        
    }
}