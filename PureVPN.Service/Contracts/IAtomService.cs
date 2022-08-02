using Atom.Core.Models;
using Atom.SDK.Core.Models;
using PureVPN.Entity.Delegates;
using PureVPN.Entity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PureVPN.Entity.Delegates.SpeedMeasurementEventHandler;
using static PureVPN.Service.Helper.EventHandlers;

namespace PureVPN.Service.Contracts
{
    public interface IAtomService
    {
        // Initialization
        Task<bool> Initialize(string secret, string vpnInterfaceName);
        void Initialize(PureAtomManagerService pureAtomManager, PureAtomBPCManagerService pureAtomBPCManager);
        void InitializePreConnectionUserBaseSpeedMeasurementProcess();

        void SetCredentials();

        // Get all countries
        Task<List<CountryModel>> GetCountries();
        Task<List<Country>> GetAtomCountries();
        List<CountryModel> MapAtomCountries(List<Country> countries);

        // Get all countries with ping
        Task<List<CountryModel>> GetCountriesWithPing();

        // Get all cities
        Task<List<CityModel>> GetCities();
        Task<List<City>> GetAtomCities();
        List<CityModel> MapAtomCities(List<City> cities);

        // Get cities by country with ping
        Task<List<CityModel>> GetCitiesByCountryWithPing(CountryModel country);

        // Get all protocols
        Task<List<ProtocolModel>> GetProtocols();
        Task<List<Protocol>> GetAtomProtocols();
        List<ProtocolModel> MapAtomProtocols(List<Protocol> protocols);

        // Quick connect
        Task<VPNProperties> GetQuickConnectProperties(List<Protocol> protocols, string protocolName);
        void QuickConnect(string protocolName, bool enableProtocolSwitch);

        // Connect country
        VPNProperties GetConnectCountryProperties(List<Country> countries, string cityName, List<Protocol> protocols, string protocolName);
        Task ConnectCountry(string countrySlug, string protocolName, bool enableProtocolSwitch, Entity.Enums.ConnectingFrom ConnectingFrom, string NasIdentifier = "", bool GotThumbsUp = false, bool GotThumbsDown = false, bool IsFromChangeServer = false, bool? AddedwithObfFilter = null, bool? AddedWithTorFilter = null, bool? AddedWithQrFilter = null, bool? AddedWithP2pFilter = null, bool? AddedWithVirtualFilter = null);

        // Connect city
        VPNProperties GetConnectCityProperties(List<City> cities, string cityName, List<Protocol> protocols, string protocolName);
        Task ConnectCity(string countrySlug, string protocolName, bool enableProtocolSwitch, string NasIdentifier = "", bool GotThumbsUp = false, bool GotThumbsDown = false, bool IsFromChangeServer = false, bool? AddedwithObfFilter = null, bool? AddedWithTorFilter = null, bool? AddedWithQrFilter = null, bool? AddedWithP2pFilter = null, bool? AddedWithVirtualFilter = null);

        // Other standard events
        event CustomStatusEvent StatusChanged;
        event DisconnectStatusEvent DisconnectedOccured;
        event PacketTransmitEvent PacketTransmitOccured;
        event InitializingStatusEvent InitializingStatusChanged;
        event SpeedMeasurmentData SpeedMeasurmentData;
        event SpeedMeasurementError SpeedMeasurementError;

        void Connect(VPNProperties properties, PureVPN.Entity.Enums.ConnectingFrom ConnectingFrom, bool isFromChangeServer = false);
        void Disconnect(bool isAsync = true);
      
        void RegisterEvents();
        void Reconnect(bool IsFromAutoConnectOnLaunch = false);
        void Cancel();
        void ReconnectAfterUTB();
        PureVPN.Entity.Models.LocationModel GetConnectedLocation();

        Task ConnectDedicatedIP(string dedicatedIP, string protocolName);

        string GetState();
        void DisableIKS();
        void RedialFromIKS();

        CountryModel GetFastestServer();
        Task<List<Country>> GetAtomCountriesByProtocol(string protocol = "");
        Task<List<CountryModel>> GetCountriesWithPingByProtocol(string protocol = "");
        Task<CountryModel> GetFastestServerByProtocol(string SelectedProtocol);
        Task<List<City>> GetAtomCitiesByProtocol(string protocol="");
        Task<List<CityModel>> GetCitiesByProtocol(string protocol = "");

        List<CountryModel> GetAtomCountriesByProtocolSync(string protocol = "");

        List<Protocol> GetAtomProtocolsSync();

        List<CityModel> GetCitiesByProtocolSync(string protocol = "");

        Task AddSplitApplication(List<ApplicationListModel> model);

        Task RemoveAllSplitApplications();

        Task RemoveSplitApplication(List<ApplicationListModel> model);
        int GetPingForDedicatedIP();


    }
}
