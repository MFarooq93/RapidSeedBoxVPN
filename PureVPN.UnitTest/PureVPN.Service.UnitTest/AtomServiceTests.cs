using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using PureVPN.UnitTest;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Atom.Core.Models;
using Atom.SDK.Core.Models;

namespace PureVPN.Service.Tests
{
    [TestClass()]
    public class AtomServiceTests : UnitTestBase
    {
        readonly string secret = "2a525ded0421ddbb6e166344bd514c1b";
        readonly string vpnAdapterName = "PureVPN";
        readonly string username = "purevpn0s8542341";
        readonly string password = "oqh2bph1";
        readonly List<Protocol> MockProtocols = new List<Protocol> { new Protocol { Name = "ikev2", ProtocolSlug = "ikev2" }, new Protocol { Name = "tcp/udp", ProtocolSlug = "tcp/udp" }, new Protocol { Name = "pptp", ProtocolSlug = "pptp" } };
        readonly List<Country> MockCountries = new List<Country> { new Country { CountrySlug = "UK" }, new Country { CountrySlug = "US" }, new Country { CountrySlug = "PK" } };
        readonly List<City> MockCities = new List<City> { new City { Name = "Seattle", Country = new Country { Name = "US", CountrySlug = "US", ISOCode = "US", Id = 2, IsActive = true, Latency = 100 }, Latency = 122, IsActive = true, CountrySlug = "US", CountryId = "2", Id = 5  } };

        [TestMethod()]
        public void LoginData_DialerToServer_ReturnTrue()
        {
            var loginData = this.GetService<IDialerToServerService>().LoginData(username, password);
            loginData.TryGetValue("sUsername", out object sUsername);
            Assert.IsNotNull(sUsername);
        }

        [TestMethod()]
        public async Task Login_DialerToServer_ReturnTrue()
        {
            // Get login data
            var loginData = this.GetService<IDialerToServerService>().LoginData(username, password);

            // Create mock function
            var dialerToServerService = new Mock<IDialerToServerService>();
            Entity.Models.DTO.LoginReply loginReply = new Entity.Models.DTO.LoginReply
            {
                body = new Entity.Models.DTO.LoginReplyBody { client_id = "1" }
            };
            dialerToServerService.Setup(x => x.Login(loginData)).Returns(Task.FromResult(loginReply));

            // Call mock object
            var response = await dialerToServerService.Object.Login(loginData);
            if(response.body.client_id != null)
            {
                AtomModel.Username = username;
                AtomModel.Password = password;
                AtomModel.IsLoggedIn = true;
            }
            Assert.IsTrue(AtomModel.IsLoggedIn);
        }

        [TestMethod()]
        public void InitializeAtomConfiguration_SDK_ReturnsTrue()
        {
            // Create mock function
            var service = new Mock<IPureAtomConfigurationService>();
            PureAtomConfigurationService PureAtomConfig = new PureAtomConfigurationService { IsInitialized = true };
            service.Setup(x => x.InitializeAtomConfiguration(secret, vpnAdapterName)).Returns(PureAtomConfig);

            // Call mock fuction
            PureAtomConfigurationService result = service.Object.InitializeAtomConfiguration(secret, vpnAdapterName);
            Assert.IsTrue(result.IsInitialized);
        }

        [TestMethod()]
        public void InitializeAtomManager_SDK_ReturnsTrue()
        {
            PureAtomConfigurationService PureAtomConfig = new PureAtomConfigurationService { IsInitialized = true };
            PureAtomManagerService PureAtomManager = new PureAtomManagerService { IsInitialized = true };

            // Create mock function
            var service = new Mock<IPureAtomManagerService>();
            service.Setup(x => x.InitializeAtomManager(PureAtomConfig)).Returns(PureAtomManager);

            // Call mock fuction
            PureAtomManagerService result = service.Object.InitializeAtomManager(PureAtomConfig);
            Assert.IsTrue(result.IsInitialized);
        }

        [TestMethod()]
        public async Task InitializeAtomBPCManager_SDK_ReturnsTrue()
        {
            PureAtomConfigurationService PureAtomConfig = new PureAtomConfigurationService { IsInitialized = true };
            PureAtomBPCManagerService PureAtomBPCManager = new PureAtomBPCManagerService { IsInitialized = true };

            // Create mock function
            var service = new Mock<IPureAtomBPCManagerService>();
            service.Setup(x => x.InitializeBPCAtomManager(PureAtomConfig)).Returns(Task.FromResult(PureAtomBPCManager));

            // Call mock fuction
            PureAtomBPCManagerService result = await service.Object.InitializeBPCAtomManager(PureAtomConfig);
            Assert.IsTrue(result.IsInitialized);
        }

        //[TestMethod()]
        //public void Initialize_SDK_ReturnsTrue()
        //{
        //    PureAtomManagerService PureAtomManager = new PureAtomManagerService { IsInitialized = true };
        //    PureAtomBPCManagerService PureAtomBPCManager = new PureAtomBPCManagerService { IsInitialized = true };

        //    var service = this.GetService<IAtomService>();
        //    service.Initialize(PureAtomManager, PureAtomBPCManager);

        //    Assert.IsTrue(AtomModel.ISSDKInitialized);
        //}

        [TestMethod()]
        public async Task Get_Protocols_ReturnsTrue()
        {
            var service = this.GetService<IAtomService>();
            var MockAtomService = new Mock<IAtomService>();
            MockAtomService.Setup(x => x.GetAtomProtocols()).Returns(Task.FromResult(MockProtocols));
            var protocols = service.MapAtomProtocols(await MockAtomService.Object.GetAtomProtocols());
            var result = protocols.Count > 0;
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task Get_Countries_ReturnsTrue()
        {
            var service = this.GetService<IAtomService>();
            var MockAtomService = new Mock<IAtomService>();
            MockAtomService.Setup(x => x.GetAtomCountries()).Returns(Task.FromResult(MockCountries));
            var countries = service.MapAtomCountries(await MockAtomService.Object.GetAtomCountries());
            var result = countries.Count > 0;
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task Get_Cities_ReturnsTrue()
        {
            var service = this.GetService<IAtomService>();
            var MockAtomService = new Mock<IAtomService>();
            MockAtomService.Setup(x => x.GetAtomCities()).Returns(Task.FromResult(MockCities));
            var cities = service.MapAtomCities(await MockAtomService.Object.GetAtomCities());
            var result = cities.Count > 0;
            Assert.IsTrue(result);
        }

        //[TestMethod()]
        //public async Task Get_QuickConnect_Properties_ReturnsTrue()
        //{
        //    var service = this.GetService<IAtomService>();
        //    VPNProperties vpnProperties = await service.GetQuickConnectProperties(MockProtocols);
        //    Assert.IsNotNull(vpnProperties.Protocol);
        //}

        //[TestMethod()]
        //public void Get_ConnectCountry_Properties_ReturnsTrue()
        //{
        //    var service = this.GetService<IAtomService>();
        //    VPNProperties vpnProperties = service.GetConnectCountryProperties(MockCountries, "US", MockProtocols, "ikev2");
        //    Assert.IsNotNull(vpnProperties.Country);
        //    Assert.IsNotNull(vpnProperties.Protocol);
        //}

        //[TestMethod()]
        //public void Get_ConnectCity_Properties_ReturnsTrue()
        //{
        //    var service = this.GetService<IAtomService>();
        //    VPNProperties vpnProperties = service.GetConnectCityProperties(MockCities, "Seattle", MockProtocols, "ikev2");
        //    Assert.IsNotNull(vpnProperties.City);
        //    Assert.IsNotNull(vpnProperties.Protocol);
        //}
    }
}