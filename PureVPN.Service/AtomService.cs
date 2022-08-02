//using Atom.BPC;
//using Atom.Core.Models;
//using Atom.SDK.Core;
//using Atom.SDK.Core.Models;
//using Atom.SDK.Net;
//using AutoMapper;
//using PureVPN.Entity.Models;
//using PureVPN.Service.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Atom.Core.Extensions;
//using static PureVPN.Service.Helper.EventHandlers;
//using PureVPN.Service.Helper;
//using Atom.Core.Enums;
//using Atom.SDK.Core.CustomEventArgs;
//using System.Diagnostics;
//using Newtonsoft.Json;
//using System.IO;
//using ErrorEventArgs = Atom.SDK.Core.ErrorEventArgs;
//using PureVPN.Service.ThirdPartyUtilities;
//using PureVPN.SpeedTest.Enums;
//using PureVPN.Entity.Delegates;
//using PureVPN.SpeedTest;
//using static PureVPN.Entity.Delegates.SpeedMeasurementEventHandler;

//namespace PureVPN.Service
//{
//    public class AtomService : IAtomService
//    {

//        private static AtomManager atomManager { get; set; }
//        private static AtomBPCManager atomBpcManager { get; set; }

//        private IMapper mapper;

//        private ISentryService sentryService;
//        private IMixpanelService mixpanelService;
//        private IPureAtomConfigurationService pureAtomConfigurationService;
//        private IPureAtomManagerService pureAtomManagerService;
//        private IPureAtomBPCManagerService pureAtomBPCManagerService;
//        private SpeedMeasurementManager speedMeasurementManager;
//        private ConnectionDetails connectionDetails;

//        private const string _beforeConnection = "before";
//        private const string _duringConnection = "during";
//        private const string _afterConnection = "after";

//        #region Speed Measurement properties for mixpanel

//        private string includedNasIdentifiers;
//        private string excludedNasIdentifiers;

//        #endregion

//        public AtomService(IMapper _mapper, ISentryService _sentryService, IMixpanelService _mixpanelService, IPureAtomConfigurationService _pureAtomConfigurationService, IPureAtomManagerService _pureAtomManagerService, IPureAtomBPCManagerService _pureAtomBPCManagerService)
//        {
//            this.mapper = _mapper;
//            this.sentryService = _sentryService;
//            this.mixpanelService = _mixpanelService;
//            this.pureAtomConfigurationService = _pureAtomConfigurationService;
//            this.pureAtomManagerService = _pureAtomManagerService;
//            this.pureAtomBPCManagerService = _pureAtomBPCManagerService;
//            this.speedMeasurementManager = new SpeedMeasurementManager();
//            this.speedMeasurementManager.SpeedMeasurmentData += SpeedMeasurementManager_SpeedMeasurmentData;
//            this.speedMeasurementManager.SpeedMeasurementError += SpeedMeasurementManager_SpeedMeasurementError;
//        }

//        //This event can cause any method which conforms
//        //to CustomStatusEvent to be called.
//        public event CustomStatusEvent StatusChanged;
//        public event DisconnectStatusEvent DisconnectedOccured;
//        public event PacketTransmitEvent PacketTransmitOccured;
//        public event InitializingStatusEvent InitializingStatusChanged;
//        public event SpeedMeasurmentData SpeedMeasurmentData;
//        public event SpeedMeasurementError SpeedMeasurementError;

//        /// <summary>
//        /// Initializes a singleton of AtomManager
//        /// </summary>
//        /// <param name="secretKey">Key to be used for Initializing AtomManager instance</param>
//        public async Task<bool> Initialize(string secret, string vpnInterfaceName)
//        {
//            await Task.Run(async () =>
//            {
//                Service.PureAtomConfigurationService pureAtomConfig = pureAtomConfigurationService.InitializeAtomConfiguration(secret, vpnInterfaceName);

//                Service.PureAtomManagerService pureAtomManager = pureAtomManagerService.InitializeAtomManager(pureAtomConfig);
//                atomManager = pureAtomManager.AtomMgr;
//                RegisterAtomInitializedEvent(AtomManagerInstance_AtomInitialized);
//                RegisterAtomDependenciesMissingEvent(AtomManagerInstance_AtomDependenciesMissing);

//                PureAtomBPCManagerService pureAtomBPCManager = await pureAtomBPCManagerService.InitializeBPCAtomManager(pureAtomConfig);
//                Initialize(pureAtomManager, pureAtomBPCManager);

//            }).ConfigureAwait(false);
//            return AtomModel.ISSDKInitialized;
//        }

//        public void Initialize(PureAtomManagerService pureAtomManager, PureAtomBPCManagerService pureAtomBPCManager)
//        {
//            if (!AtomModel.ISSDKInitialized || (pureAtomManager != null & pureAtomBPCManager != null))
//            {
//                atomBpcManager = pureAtomBPCManager.AtomBpcMgr;
//                AtomModel.ISSDKInitialized = true;
//                AtomModel.IsConnected = atomManager.GetCurrentVPNStatus() == Atom.SDK.Core.Enumerations.VPNStatus.CONNECTED;
//                AtomModel.BackgroundVpnStatus = AtomModel.IsConnected;
//            }
//        }

//        public void InitializePreConnectionUserBaseSpeedMeasurementProcess()
//        {
//            //Note: If base speed of user found in cache then speed will not be calculated on every app launch
//            if (AtomModel.IsEnableNetworkAnalysis && Environment.Is64BitOperatingSystem)
//            {
//                speedMeasurementManager.StartSpeedMeasurement(SpeedMeasurementType.PreConnection);

//                SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = true;
//                SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = false;
//            }
//        }

//        public string GetState()
//        {
//            var a = atomManager.GetCurrentVPNStatus();
//            return a.ToString();
//        }

//        public void RegisterEvents()
//        {
//            if (!AtomModel.IsEventsRegister)
//            {
//                RegisterConnectedEvent(AtomManagerInstance_Connected);
//                RegisterDisconnectedEvent(AtomManagerInstance_Disconnected);
//                RegisterDialErrorEvent(AtomManagerInstance_DialError);
//                RegisterRedialingEvent(AtomManagerInstance_Redialing);
//                RegisterStateChangedEvent(AtomManagerInstance_StateChanged);
//                RegisterOnUnableToAccessInternetChangedEvent(AtomManagerInstance_UnableToAccessInternet);
//                RegisterConnectedLocationEvent(AtomManagerInstance_ConnectedLocation);
//                RegisterPacketsTransmittedEvent(AtomManagerInstance_PacketsTransmitted);

//                AtomModel.IsEventsRegister = true;
//            }
//        }

//        public void SetCredentials()
//        {
//            if (AtomModel.Username.IsStringNotNullorEmptyorWhitespace() && AtomModel.Password.IsStringNotNullorEmptyorWhitespace())
//            {
//                atomManager.Credentials = new Credentials(AtomModel.Username.Trim(), AtomModel.Password.Trim());
//            }
//        }

//        // Get Countries
//        public async Task<List<CountryModel>> GetCountries()
//        {
//            return MapAtomCountries(await GetAtomCountries());
//        }
//        public async Task<List<CountryModel>> GetCountriesWithPing()
//        {
//            List<Country> countries = await GetAtomCountries();

//            var stopWatch = Stopwatch.StartNew();
//            List<Country> countriesWithPing = await countries.PingAsync();
//            double pingTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();

//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "Country");
//            props.Add("app_ping_time", pingTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_ping_time, AtomModel.HostingID, props);
//            return MapAtomCountries(countriesWithPing);
//        }

//        public async Task<List<CountryModel>> GetCountriesWithPingByProtocol(string protocol = "")
//        {
//            List<Country> countries = await GetAtomCountriesByProtocol(protocol);

//            var stopWatch = Stopwatch.StartNew();
//            List<Country> countriesWithPing = await countries.PingAsync();
//            double pingTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();

//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "Country");
//            props.Add("app_ping_time", pingTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_ping_time, AtomModel.HostingID, props);
//            return MapAtomCountries(countriesWithPing);
//        }

//        public async Task<List<Country>> GetAtomCountries()
//        {
//            var stopWatch = Stopwatch.StartNew();
//            List<Country> countries = await atomBpcManager?.GetCountries();
//            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();

//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "Country");
//            props.Add("app_location_load_time", loadTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

//            return countries;
//        }



//        public async Task<List<Country>> GetAtomCountriesByProtocol(string protocol = "")
//        {
//            Protocol obj = new Protocol();
//            List<Country> countries = new List<Country>();
//            var stopWatch = Stopwatch.StartNew();
//            var protocols = await GetAtomProtocols();

//            if (protocols != null && !string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
//            {
//                if (string.IsNullOrEmpty(protocol))
//                    obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
//                else
//                    obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();
//            }


//            if (!string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
//            {
//                countries = await atomBpcManager?.GetCountriesByProtocol(obj);
//            }
//            else
//                countries = await atomBpcManager?.GetCountries();

//            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();

//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "Country");
//            props.Add("app_location_load_time", loadTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

//            return countries;
//        }

//        public List<CountryModel> GetAtomCountriesByProtocolSync(string protocol = "")
//        {
//            try
//            {
//                Protocol obj = new Protocol();
//                var stopWatch = Stopwatch.StartNew();
//                var protocols = GetAtomProtocolsSync();

//                if (protocols != null)
//                {
//                    if (string.IsNullOrEmpty(protocol))
//                        obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
//                    else
//                        obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();
//                }

//                Task<List<Country>> countries = atomBpcManager?.GetCountriesByProtocol(obj);
//                double loadTime = stopWatch.Elapsed.TotalMilliseconds;
//                stopWatch.Stop();

//                if (countries != null && countries.Result != null && countries.Result.Count > 0)
//                    return MapAtomCountries(countries.Result);
//                else
//                    return null;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }

//        }


//        public List<CountryModel> MapAtomCountries(List<Country> countries)
//        {
//            return mapper.Map<List<Atom.Core.Models.Country>, List<CountryModel>>(countries);
//        }

//        // Get Cities
//        public async Task<List<CityModel>> GetCities()
//        {
//            List<City> cities = await GetAtomCities();
//            return await GetCitiesWithPing(cities);
//        }
//        public async Task<List<CityModel>> GetCitiesByProtocol(string protocol = "")
//        {
//            List<City> cities = await GetAtomCitiesByProtocol(protocol);
//            return await GetCitiesWithPing(cities);
//        }

//        public List<CityModel> GetCitiesByProtocolSync(string protocol = "")
//        {
//            List<City> cities = GetAtomCitiesByProtocolSync(protocol);
//            return MapAtomCities(cities);
//        }
//        public async Task<List<CityModel>> GetCitiesWithPing(List<City> cities)
//        {
//            var stopWatch = Stopwatch.StartNew();
//            List<City> citiesWithPing = await cities.PingAsync();
//            double pingTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();


//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "City");
//            props.Add("app_ping_time", pingTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_ping_time, AtomModel.HostingID, props);

//            return MapAtomCities(citiesWithPing);
//        }
//        public async Task<List<City>> GetAtomCities()
//        {
//            var stopWatch = Stopwatch.StartNew();
//            List<City> cities = await atomBpcManager?.GetCities();
//            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();

//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "City");
//            props.Add("app_location_load_time", loadTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

//            return cities;
//        }


//        public async Task<List<City>> GetAtomCitiesByProtocol(string protocol = "")
//        {
//            var stopWatch = Stopwatch.StartNew();
//            Protocol obj = new Protocol();
//            List<City> cities = new List<City>();

//            var protocols = await GetAtomProtocols();

//            if (protocols != null && !string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
//            {
//                if (string.IsNullOrEmpty(protocol))
//                    obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
//                else
//                    obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();
//            }


//            if (!string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
//                cities = await atomBpcManager?.GetCitiesByProtocol(obj);
//            else
//                cities = await atomBpcManager?.GetCities();

//            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();

//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "City");
//            props.Add("app_location_load_time", loadTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

//            return cities;
//        }

//        public List<City> GetAtomCitiesByProtocolSync(string protocol = "")
//        {
//            var stopWatch = Stopwatch.StartNew();
//            Protocol obj = new Protocol();
//            var protocols = GetAtomProtocolsSync();

//            if (protocols != null)
//            {
//                if (string.IsNullOrEmpty(protocol))
//                    obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
//                else
//                    obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();

//            }


//            // List<City> cities = atomBpcManager?.GetCitiesByProtocol(obj).Result;
//            List<City> cities = atomBpcManager?.GetCitiesByProtocol(obj).Result;
//            return cities;
//        }

//        public List<CityModel> MapAtomCities(List<City> cities)
//        {
//            return mapper.Map<List<Atom.Core.Models.City>, List<CityModel>>(cities);
//        }
//        public async Task<List<CityModel>> GetCitiesByCountryWithPing(CountryModel country)
//        {
//            var con = mapper.Map<CountryModel, Country>(country);

//            var stopWatch = Stopwatch.StartNew();
//            List<City> cities = await atomBpcManager?.GetCitiesByCountry(con);
//            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
//            stopWatch.Stop();


//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("type", "City");
//            props.Add("app_location_load_time", loadTime);
//            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);


//            List<CityModel> citiesWithPing = await GetCitiesWithPing(cities);
//            return citiesWithPing;
//        }

//        // Get Protocols
//        public async Task<List<ProtocolModel>> GetProtocols()
//        {
//            return MapAtomProtocols(await GetAtomProtocols());
//        }
//        public async Task<List<Protocol>> GetAtomProtocols()
//        {
//            List<Protocol> Protocols = await atomBpcManager.GetProtocols();

//            foreach (Protocol protocol in Protocols)
//            {
//                protocol.Name = protocol.ProtocolSlug;
//            }

//            Protocol automaticProtocol = new Protocol();
//            automaticProtocol.Name = "Automatic";
//            automaticProtocol.ProtocolSlug = "IKEv2";
//            automaticProtocol.IsActive = true;
//            Protocols.Add(automaticProtocol);

//            Protocols = Protocols.OrderBy(x => x.Name).ToList();

//            return Protocols;

//        }

//        public List<Protocol> GetAtomProtocolsSync()
//        {
//            List<Protocol> Protocols = atomBpcManager.GetProtocols().Result;

//            foreach (Protocol protocol in Protocols)
//            {
//                protocol.Name = protocol.ProtocolSlug;
//            }

//            Protocol automaticProtocol = new Protocol();
//            automaticProtocol.Name = "Automatic";
//            automaticProtocol.ProtocolSlug = "IKEv2";
//            automaticProtocol.IsActive = true;
//            Protocols.Add(automaticProtocol);

//            Protocols = Protocols.OrderBy(x => x.Name).ToList();

//            return Protocols;

//        }
//        public List<ProtocolModel> MapAtomProtocols(List<Protocol> protocols)
//        {
//            return mapper.Map<List<Atom.Core.Models.Protocol>, List<ProtocolModel>>(protocols);
//        }

//        /// <summary>
//        /// Connect using AtomManager
//        /// </summary>
//        /// <param name="properties">Properties to be used for connection</param>
//        public void Connect(VPNProperties properties, PureVPN.Entity.Enums.ConnectingFrom ConnectingFrom, bool isFromChangeServer = false)
//        {
//            AtomModel.IsExperimentServerRequested = false;
//            AtomModel.SpeedtestGroup = string.Empty;
//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("selected_protocol_name", AtomModel.SelectedProtocol.ToUpper());
//            props.Add("iks_enabled", AtomModel.IsIksEnable);
//            props.Add("selected_location", AtomModel.BeforeConnectionLocation);

//            #region NAS Identifier
//            props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
//            props.Add("is_filtered_server", false);

//            if (properties != null && properties.ServerFilters != null && properties.ServerFilters.Count > 0 && AtomModel.IsSameExperienceCheckEnable)
//                props.Add("is_filtered_server_requested", true);
//            else
//                props.Add("is_filtered_server_requested", false);

//            if (properties != null && properties.ServerFilters != null && properties.ServerFilters.Count > 0 && AtomModel.IsSameExperienceCheckEnable)
//            {
//                foreach (var item in properties.ServerFilters)
//                {
//                    if (item?.FilterType == Atom.SDK.Core.Enumerations.ServerFilterType.Include)
//                    {
//                        AtomModel.IncludedNasIdentifiers = new List<string>();
//                        AtomModel.IncludedNasIdentifiers.Add(item.NASIdentifier);
//                    }

//                    else if (item?.FilterType == Atom.SDK.Core.Enumerations.ServerFilterType.Exclude)
//                    {
//                        AtomModel.ExcludedNasIdentifiers = new List<string>();
//                        AtomModel.ExcludedNasIdentifiers.Add(item.NASIdentifier);
//                    }

//                }

//                props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
//                props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);
//            }


//            #endregion

//            try
//            {
//                props.Add("connect_via", ConnectingFrom.ToString());
//                props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
//                props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
//                props.Add("is_split_enabled", AtomModel.IsSplitEnable);

//                if (AtomModel.ConnectedTo != Entity.Enums.ConnectedTo.NotConnected)
//                    props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
//                else
//                    props.Add("selected_interface", Entity.Enums.ConnectedTo.List);
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }

//            if (AtomModel.Experiments != null && AtomModel.Experiments.body != null && AtomModel.Experiments.header.response_code == 200)
//            {
//                var experimentname = AtomModel.Experiments.body.experiments.Where(x => x.name.ToLower() == "speedtest").FirstOrDefault();

//                if (experimentname != null)
//                {
//                    string groupname = experimentname.group;
//                    AtomModel.SpeedtestGroup = groupname;
//                }
//                else
//                    AtomModel.SpeedtestGroup = string.Empty;
//            }
//            else
//                AtomModel.SpeedtestGroup = string.Empty;

//            if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//            {
//                props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

//                if (AtomModel.SpeedtestGroup.ToLower() == "b")
//                {
//                    properties.ExperimentProperties = new ExperimentProperties() { IsExperimentedUser = true };
//                    AtomModel.IsExperimentServerRequested = true;

//                    if (SpeedMeasurementModel.PreConnectionBaseSpeedOfUser > default(double))
//                        properties.ExperimentProperties.BaseSpeed = SpeedMeasurementModel.PreConnectionBaseSpeedOfUser;
//                }
//                else
//                    AtomModel.IsExperimentServerRequested = false;
//            }

//            props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);

//            #region Connection filters
//            props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//            props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//            #endregion

//            mixpanelService.FireEvent(MixpanelEvents.app_connect, AtomModel.HostingID, props);

//            if (isFromChangeServer)
//                mixpanelService.FireEvent(MixpanelEvents.app_click_switch_server, AtomModel.HostingID, props);

//            AtomModel.connectingFrom = ConnectingFrom;
//            Common.connectTime = DateTime.UtcNow;

//            properties.EnableDNSLeakProtection = true;
//            properties.EnableIPv6LeakProtection = true;
//            properties.EnableIKS = AtomModel.IsIksEnable;
//            properties.DoCheckInternetConnectivity = true;
//            properties.UseSplitTunneling = AtomModel.IsSplitEnable;
//            properties.IsTrackInSessionUTB = true;

//            atomManager.Connect(properties);
//        }

//        // Connect Country
//        public async Task ConnectCountry(string countrySlug, string protocolName, bool enableProtocolSwitch, Entity.Enums.ConnectingFrom ConnectingFrom, string NasIdentifier = "", bool GotThumbsUp = false, bool GotThumbsDown = false, bool IsFromChangeServer = false, bool? AddedwithObfFilter = null, bool? AddedWithTorFilter = null, bool? AddedWithQrFilter = null, bool? AddedWithP2pFilter = null, bool? AddedWithVirtualFilter = null)
//        {
//            var countries = await GetAtomCountries();
//            var protocols = await GetAtomProtocols();
//            AtomModel.CountrySlugAboutToConnect = countrySlug;

//            VPNProperties properties = GetConnectCountryProperties(countries, countrySlug, protocols, protocolName);

//            if (AtomModel.IsSameExperienceCheckEnable && !IsFromChangeServer)
//                IncludeExcludeNasIdentifier(properties, GotThumbsDown, GotThumbsUp, NasIdentifier);
//            else if (IsFromChangeServer)
//                ExcludeNasIdentifier(properties);

//            if (properties.Protocol != null && !enableProtocolSwitch)
//            {
//                properties.EnableProtocolSwitch = false;
//                properties.SecondaryProtocol = properties.Protocol;
//                properties.TertiaryProtocol = properties.Protocol;
//                properties.UseSplitTunneling = true;
//            }

//            Connect(SendFilterToAtom(properties, AddedwithObfFilter, AddedWithQrFilter), ConnectingFrom, IsFromChangeServer);
//        }

//        private VPNProperties SendFilterToAtom(VPNProperties props, bool? ObfFilter, bool? QRFilter)
//        {
//            AtomModel.isObfSupported = ObfFilter;
//            AtomModel.isQrSupported = QRFilter;

//            if (ObfFilter != null)
//                props.DialWithOVPNObfuscatedServer = ObfFilter;
//            if (QRFilter != null)
//                props.DialWithQuantumResistantServer = QRFilter;

//            return props;
//        }

//        public async Task AddSplitApplication(List<ApplicationListModel> model)
//        {
//            for (int i = 0; i <= model.Count - 1; i++)
//            {
//                SplitApplication app = new SplitApplication();
//                app.CompleteExePath = model[i].Path;

//                atomManager.ApplySplitTunneling(app);


//                if (model[i].SupportingExe != null && model[i].SupportingExe.Count > 0)
//                {
//                    var abspath = Path.GetDirectoryName(model[i].Path);

//                    if (!string.IsNullOrEmpty(abspath))
//                    {
//                        foreach (var item in model[i].SupportingExe)
//                        {
//                            SplitApplication subapp = new SplitApplication();
//                            subapp.CompleteExePath = Path.Combine(abspath, item);

//                            atomManager.ApplySplitTunneling(subapp);
//                        }
//                    }
//                }
//            }
//        }

//        public async Task RemoveSplitApplication(List<ApplicationListModel> model)
//        {
//            for (int i = 0; i <= model.Count - 1; i++)
//            {
//                SplitApplication app = new SplitApplication();
//                app.CompleteExePath = model[i].Path;

//                atomManager.RemoveSplitTunnelingApplication(app);

//                if (model[i].SupportingExe != null && model[i].SupportingExe.Count > 0)
//                {
//                    var abspath = Path.GetDirectoryName(model[i].Path);

//                    if (!string.IsNullOrEmpty(abspath))
//                    {
//                        foreach (var item in model[i].SupportingExe)
//                        {
//                            SplitApplication subapp = new SplitApplication();
//                            subapp.CompleteExePath = Path.Combine(abspath, item);

//                            atomManager.RemoveSplitTunnelingApplication(subapp);
//                        }
//                    }
//                }
//            }
//        }

//        public async Task RemoveAllSplitApplications()
//        {
//            atomManager.RemoveAllSplitTunnelingApplications();
//        }

//        private void IncludeExcludeNasIdentifier(VPNProperties properties, bool GotThumbsDown, bool GotThumbsUp, string NasIdentifier)
//        {
//            var FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Include;

//            if (!GotThumbsDown && GotThumbsUp)
//                FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Include;
//            else if (GotThumbsDown && !GotThumbsUp)
//                FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Exclude;

//            if (!string.IsNullOrEmpty(NasIdentifier) && ((GotThumbsUp && !GotThumbsDown) || (!GotThumbsUp && GotThumbsDown)))
//            {
//                ServerFilter filter = new ServerFilter
//                {
//                    FilterType = FilterType,
//                    NASIdentifier = NasIdentifier
//                };

//                if (Common.ServerChoiceList == null)
//                    Common.ServerChoiceList = new List<ServerFilter>();

//                if (Common.ServerChoiceList.Any(x => x.NASIdentifier == filter.NASIdentifier))
//                    Common.ServerChoiceList.Remove(filter);

//                Common.ServerChoiceList.Add(filter);

//                if (properties.ServerFilters == null)
//                    properties.ServerFilters = new List<ServerFilter>();

//                properties.ServerFilters.AddRange(Common.ServerChoiceList);
//            }
//        }


//        private void ExcludeNasIdentifier(VPNProperties properties)
//        {
//            List<ServerFilter> ServerChoiceList = new List<ServerFilter>();
//            var FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Exclude;

//            foreach (var item in AtomModel.UserPreferenceServers)
//            {
//                if (!string.IsNullOrEmpty(item.Nasidentifier))
//                {
//                    ServerFilter filter = new ServerFilter
//                    {
//                        FilterType = FilterType,
//                        NASIdentifier = item.Nasidentifier
//                    };

//                    if (ServerChoiceList.Any(x => x.NASIdentifier == filter.NASIdentifier))
//                        ServerChoiceList.Remove(filter);

//                    ServerChoiceList.Add(filter);

//                    if (properties.ServerFilters == null)
//                        properties.ServerFilters = new List<ServerFilter>();
//                }
//            }
//            properties.ServerFilters.AddRange(ServerChoiceList);

//        }
//        public async Task ConnectDedicatedIP(string dedicatedIP, string protocolName)
//        {
//            var protocols = await GetAtomProtocols();

//            VPNProperties properties = GetConnectDedicatedIPProperties(dedicatedIP, protocols, protocolName);
//            Connect(properties, Entity.Enums.ConnectingFrom.DedicatedIP);
//        }

//        public VPNProperties GetConnectCountryProperties(List<Country> countries, string countrySlug, List<Protocol> protocols, string protocolName)
//        {
//            var selectedCountry = countries.FirstOrDefault(x => x.CountrySlug.ToLower() == countrySlug.ToLower());
//            var selectedProtocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());

//            if (selectedCountry != null)
//                AtomModel.BeforeConnectionLocation = selectedCountry.Name;
//            else
//                AtomModel.BeforeConnectionLocation = string.Empty;

//            AtomModel.SelectedProtocol = selectedProtocol.Name.ToUpper();

//            if (selectedProtocol.Name.ToUpper() == "AUTOMATIC")
//                return new VPNProperties(selectedCountry);
//            else
//                return new VPNProperties(selectedCountry, selectedProtocol);
//        }

//        public VPNProperties GetConnectDedicatedIPProperties(string dedicatedIP, List<Protocol> protocols, string protocolName)
//        {
//            var selectedProtocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());
//            AtomModel.SelectedProtocol = selectedProtocol.Name.ToUpper();
//            return new VPNProperties(dedicatedIP, selectedProtocol);
//        }

//        // Connect City
//        public async Task ConnectCity(string cityName, string protocolName, bool enableProtocolSwitch, string NasIdentifier = "", bool GotThumbsUp = false, bool GotThumbsDown = false, bool IsFromChangeServer = false, bool? AddedwithObfFilter = null, bool? AddedWithTorFilter = null, bool? AddedWithQrFilter = null, bool? AddedWithP2pFilter = null, bool? AddedWithVirtualFilter = null)
//        {
//            var cities = await GetAtomCitiesByProtocol();
//            var protocols = await GetAtomProtocols();
//            AtomModel.CountrySlugAboutToConnect = cityName;

//            VPNProperties properties = GetConnectCityProperties(cities, cityName, protocols, protocolName);

//            if (AtomModel.IsSameExperienceCheckEnable && !IsFromChangeServer)
//                IncludeExcludeNasIdentifier(properties, GotThumbsDown, GotThumbsUp, NasIdentifier);
//            else if (IsFromChangeServer)
//                ExcludeNasIdentifier(properties);

//            if (properties.Protocol != null && !enableProtocolSwitch)
//            {
//                properties.EnableProtocolSwitch = false;
//                properties.SecondaryProtocol = properties.Protocol;
//                properties.TertiaryProtocol = properties.Protocol;
//            }

//            Connect(SendFilterToAtom(properties, AddedwithObfFilter, AddedWithQrFilter), Entity.Enums.ConnectingFrom.City, IsFromChangeServer);
//        }
//        public VPNProperties GetConnectCityProperties(List<City> cities, string cityName, List<Protocol> protocols, string protocolName)
//        {
//            var selectedCity = cities.FirstOrDefault(x => x.Name.ToLower() == cityName.ToLower());
//            var selectedProtocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());

//            if (selectedCity != null)
//                AtomModel.BeforeConnectionLocation = selectedCity.Name;
//            else
//                AtomModel.BeforeConnectionLocation = string.Empty;

//            AtomModel.SelectedProtocol = selectedProtocol.Name.ToUpper();

//            if (selectedProtocol.Name.ToUpper() == "AUTOMATIC")
//                return new VPNProperties(selectedCity);
//            else
//                return new VPNProperties(selectedCity, selectedProtocol);

//        }

//        // Quick Connect
//        public async void QuickConnect(string ProtocolName, bool enableProtocolSwitch)
//        {
//            List<Protocol> protocols = await GetAtomProtocols();
//            VPNProperties properties = await GetQuickConnectProperties(protocols, ProtocolName);

//            if (properties.Protocol != null && !enableProtocolSwitch)
//            {
//                properties.EnableProtocolSwitch = false;
//                properties.SecondaryProtocol = properties.Protocol;
//                properties.TertiaryProtocol = properties.Protocol;
//            }

//            Connect(properties, Entity.Enums.ConnectingFrom.SmartConnect);
//        }
//        public async Task<VPNProperties> GetQuickConnectProperties(List<Protocol> protocols, string protocolName)
//        {
//            var protocol = new Protocol();

//            if (protocols == null || protocols.Count() == 0)
//                protocols = await GetAtomProtocols();

//            protocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());
//            var tagsList = new List<SmartConnectTag>
//            {
//                SmartConnectTag.AUTOMATIC,
//                SmartConnectTag.PAID
//            };

//            AtomModel.BeforeConnectionLocation = string.Empty;
//            AtomModel.SelectedProtocol = protocol.Name.ToUpper();

//            return new VPNProperties(protocol, tagsList);
//        }

//        /// <summary>
//        /// Disconnects the VPN
//        /// </summary>
//        public void Disconnect(bool isAsync = true)
//        {
//            try
//            {
//                if (isAsync)
//                    Task.Run(() => { try { atomManager.Disconnect(); } catch (Exception ex) { /*sentryService.LoggingException(ex);*/ } });
//                else
//                    atomManager.Disconnect();
//                // AtomModel.IsDisconnected = true;
//                AtomModel.IsConnected = false;
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        /// <summary>
//        /// Cancels the ongoing VPN connection
//        /// </summary>
//        public void Cancel()
//        {
//            try
//            {
//                atomManager.Cancel();
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        /// <summary>
//        /// Reconnect VPN connection
//        /// </summary>
//        public void Reconnect(bool IsFromAutoConnectOnLaunch = false)
//        {
//            try
//            {
//                if (IsFromAutoConnectOnLaunch)
//                    Common.connectTime = DateTime.UtcNow;

//                atomManager.ReConnect();
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        /// <summary>
//        /// Registers Connected Event
//        /// </summary>
//        /// <param name="onConnected">EventHandler for Connected event</param>
//        public void RegisterConnectedEvent(EventHandler<ConnectedEventArgs> onConnected)
//        {
//            atomManager.Connected += onConnected;
//        }

//        /// <summary>
//        /// Registers PacketsTransmitted Event
//        /// </summary>
//        /// <param name="onPacketsTransmitted">EventHandler for Connected event</param>
//        public void RegisterPacketsTransmittedEvent(EventHandler<PacketsTransmittedEventArgs> onPacketsTransmitted)
//        {
//            atomManager.PacketsTransmitted += onPacketsTransmitted;
//        }

//        /// <summary>
//        /// Registers Disconnected Event
//        /// </summary>
//        /// <param name="onConnect">EventHandler for Disconnected event</param>
//        public void RegisterDisconnectedEvent(EventHandler<DisconnectedEventArgs> onDisconnected)
//        {
//            atomManager.Disconnected += onDisconnected;
//        }

//        /// <summary>
//        /// Registers DialError Event
//        /// </summary>
//        /// <param name="onConnect">EventHandler for DialError event</param>
//        public void RegisterDialErrorEvent(EventHandler<DialErrorEventArgs> onDialError)
//        {
//            atomManager.DialError += onDialError;
//        }

//        /// <summary>
//        /// Registers StateChanged Event
//        /// </summary>
//        /// <param name="onConnect">EventHandler for StateChanged event</param>
//        public void RegisterStateChangedEvent(EventHandler<StateChangedEventArgs> onStateChanged)
//        {
//            atomManager.StateChanged += onStateChanged;
//        }

//        /// <summary>
//        /// Registers Redialing Event
//        /// </summary>
//        /// <param name="onConnect">EventHandler for Redialing event</param>
//        public void RegisterRedialingEvent(EventHandler<ErrorEventArgs> onRedial)
//        {
//            atomManager.Redialing += onRedial;
//        }

//        /// <summary>
//        /// Fetches the list of smart countries using AtomManager instance
//        /// </summary>
//        /// <returns>List of allowed Countries</returns>
//        public List<Country> GetSmartCountries()
//        {
//            return atomManager.GetCountriesForSmartDialing();
//        }

//        /// <summary>
//        /// Registers UnableToAccessInternet Event
//        /// </summary>
//        /// <param name="onUnableToAccessInternet">EventHandler for UnableToAccessInternet event</param>
//        public void RegisterOnUnableToAccessInternetChangedEvent(EventHandler<UnableToAccessInternetEventArgs> onUnableToAccessInternet)
//        {
//            atomManager.OnUnableToAccessInternet += onUnableToAccessInternet;
//        }

//        public void RegisterConnectedLocationEvent(EventHandler<ConnectedLocationEventArgs> onConnectedLocation)
//        {
//            atomManager.ConnectedLocation += onConnectedLocation;
//        }

//        public void RegisterAtomInitializedEvent(EventHandler<AtomInitializedEventArgs> onAtomInitialized)
//        {
//            atomManager.AtomInitialized += onAtomInitialized;
//        }

//        public void RegisterAtomDependenciesMissingEvent(EventHandler<AtomDependenciesMissingEventArgs> onAtomDependenciesMissing)
//        {
//            atomManager.AtomDependenciesMissing += onAtomDependenciesMissing;
//        }

//        private void AtomManagerInstance_Connected(object sender, ConnectedEventArgs e)
//        {
//            connectionDetails = e.ConnectionDetails;
//            DateTime connectedTime = DateTime.UtcNow;

//            string country = string.Empty;
//            string flag = string.Empty;
//            AtomModel.IsConnected = true;
//            AtomModel.IsCancelled = false;
//            AtomModel.IsConnecting = false;
//            AtomModel.ShowCancelButton = false;
//            AtomModel.StartConnecting = false;
//            AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
//            AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;

//            if (e.ConnectionDetails.City != null)
//            {
//                AtomModel.IsConnectedToCity = true;
//                AtomModel.LastConnectedCityName = e.ConnectionDetails.City.Name;
//                AtomModel.LastConnectedCountryName = string.Empty;
//            }
//            else
//            {
//                AtomModel.IsConnectedToCity = false;
//                AtomModel.LastConnectedCityName = string.Empty;
//                AtomModel.LastConnectedCountryName = e.ConnectionDetails.Country;
//            }

//            StatusModel status = new StatusModel();
//            status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnected);
//            status.CurrentStatus = Entity.Enums.CurrentStatus.Connected;
//            status.City = e.ConnectionDetails.City?.Name;
//            status.ServeIP = e.ConnectionDetails.ServerIP;

//            if (!string.IsNullOrEmpty(country))
//            {
//                status.Country = country;
//                status.Flag = flag;
//            }

//            StatusChanged(status);

//            try
//            {
//                if (status != null && !string.IsNullOrEmpty(status.Country) && (AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.Country || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.SmartConnect || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.DedicatedIP))
//                    AtomModel.AfterConnectionLocation = status.Country;
//                else if (status != null && !string.IsNullOrEmpty(status.City) && AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.City)
//                    AtomModel.AfterConnectionLocation = status.City;
//                else
//                    AtomModel.AfterConnectionLocation = string.Empty;
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }


//            if (AtomModel.SelectedAppsForSplit != null && AtomModel.SelectedAppsForSplit.Count > 0)
//            {
//                AddSplitApplication(AtomModel.SelectedAppsForSplit);
//            }

//            ServerPreference pref = new ServerPreference();
//            pref.GotThumbsDown = true;
//            pref.GotThumbsUp = false;
//            pref.Nasidentifier = e.ConnectionDetails.NASIdentifier;
//            pref.IsConnectedViaCountry = !AtomModel.IsConnectedToCity;
//            pref.SelectedProtocol = AtomModel.SelectedProtocol;
//            pref.LocationSlug = AtomModel.CountrySlugAboutToConnect;
//            pref.ConnectingFrom = AtomModel.connectingFrom;

//            if (AtomModel.UserPreferenceServers == null)
//                AtomModel.UserPreferenceServers = new List<ServerPreference>();

//            AtomModel.UserPreferenceServers.Add(pref);

//            #region Mixpanel

//            try
//            {
//                #region Connected event

//                var propsConn = new MixpanelProperties().MixPanelPropertiesDictionary;

//                if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerIP))
//                {
//                    AtomModel.ServerIP = e.ConnectionDetails.ServerIP;
//                    propsConn.Add("server_ip", e.ConnectionDetails.ServerIP);
//                }

//                if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerAddress))
//                {
//                    AtomModel.ServerAddress = e.ConnectionDetails.ServerAddress;
//                    propsConn.Add("server_dns", e.ConnectionDetails.ServerAddress);
//                }

//                if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
//                {
//                    propsConn.Add("selected_protocol_name", AtomModel.SelectedProtocol);
//                }

//                if (e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.ProtocolSlug))
//                {
//                    AtomModel.DialedProtocol = e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper();
//                    propsConn.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper());
//                    AtomModel.DialedProtocolForConnectionDetails = e.ConnectionDetails?.Protocol?.ProtocolSlug;
//                }

//                propsConn.Add("selected_location", AtomModel.BeforeConnectionLocation);
//                propsConn.Add("dialed_location", AtomModel.AfterConnectionLocation);

//                propsConn.Add("iks_enabled", AtomModel.IsIksEnable);

//                bool connected_with_desired_location = false;
//                if (!string.IsNullOrEmpty(AtomModel.BeforeConnectionLocation) && !string.IsNullOrEmpty(AtomModel.AfterConnectionLocation) && AtomModel.BeforeConnectionLocation.ToLower() == AtomModel.AfterConnectionLocation.ToLower())
//                    connected_with_desired_location = true;

//                propsConn.Add("connected_via_desired_location", connected_with_desired_location);

//                try
//                {
//                    propsConn.Add("connect_via", AtomModel.connectingFrom.ToString());

//                    propsConn.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());

//                    propsConn.Add("is_split_enabled", AtomModel.IsSplitEnable);

//                    if (AtomModel.IsIksEnable && e.ConnectionDetails.IKSIsEnabled)
//                    {
//                        propsConn.Add("connection_initiated_by", Entity.Enums.ConnectedTo.AutoReconnect.ToString());
//                    }
//                    else
//                    {
//                        propsConn.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
//                    }

//                    if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
//                    {
//                        propsConn.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
//                        AtomModel.SelectedInterface = Entity.Enums.ConnectedTo.List.ToString();
//                    }
//                    else
//                    {
//                        propsConn.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
//                        AtomModel.SelectedInterface = AtomModel.ConnectedTo.GetEnumDescription();
//                    }

//                    propsConn.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//                    propsConn.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }

//                #region NAS Identifier
//                try
//                {
//                    propsConn.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
//                    propsConn.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

//                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        propsConn.Add("is_filtered_server_requested", true);
//                    else
//                        propsConn.Add("is_filtered_server_requested", false);


//                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        propsConn.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


//                    propsConn.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
//                    propsConn.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);

//                    if (AtomModel.IncludedNasIdentifiers != null)
//                        includedNasIdentifiers = string.Join(",", AtomModel.IncludedNasIdentifiers.ToArray());

//                    if (AtomModel.ExcludedNasIdentifiers != null)
//                        excludedNasIdentifiers = string.Join(",", AtomModel.ExcludedNasIdentifiers.ToArray());
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }

//                #endregion
//                if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//                    propsConn.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

//                propsConn.Add("atom_session_id", e.ConnectionDetails.SessionID);

//                #region Connection filters
//                propsConn.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//                propsConn.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//                #endregion

//                mixpanelService.FireEvent(MixpanelEvents.app_connected, AtomModel.HostingID, propsConn);

//                #endregion

//                #region TTC event
//                var props = new MixpanelProperties().MixPanelPropertiesDictionary;

//                double duration = 0;
//                try { duration = (connectedTime.Subtract(Common.connectTime)).TotalSeconds; } catch (Exception ex) { /*sentryService.LoggingException(ex);*/ }

//                if (e.ConnectionDetails.TotalTimeTakenToConnect > 0)
//                    props.Add("time_to_connect_server", Math.Round(e.ConnectionDetails.TotalTimeTakenToConnect, 2));
//                else
//                    props.Add("time_to_connect_server", 0);

//                if (e.ConnectionDetails.TimeTakenToFindFastestServer > 0)
//                    props.Add("time_to_find_server", Math.Round(e.ConnectionDetails.TimeTakenToFindFastestServer, 2));
//                else
//                    props.Add("time_to_find_server", 0);

//                if (duration > 0)
//                    props.Add("$duration", Math.Round(duration, 2));

//                props.Add("server_ip", e.ConnectionDetails.ServerIP);
//                props.Add("server_dns", e.ConnectionDetails.ServerAddress);

//                if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
//                    props.Add("selected_protocol_name", AtomModel.SelectedProtocol);

//                if (e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.ProtocolSlug))
//                    props.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper());

//                props.Add("selected_location", AtomModel.BeforeConnectionLocation);
//                props.Add("dialed_location", AtomModel.AfterConnectionLocation);
//                props.Add("connect_via", AtomModel.connectingFrom.ToString());
//                props.Add("connected_via_desired_location", connected_with_desired_location);

//                if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
//                    props.Add("selected_interface", Entity.Enums.ConnectedTo.Location.ToString());
//                else
//                    props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());

//                props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//                AtomModel.IsExperimentedServer = e.ConnectionDetails.IsExperimentedServer;
//                props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//                props.Add("is_split_enabled", AtomModel.IsSplitEnable);
//                props.Add("atom_session_id", e.ConnectionDetails.SessionID);

//                #region Connection filters
//                props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//                props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//                #endregion


//                mixpanelService.FireEvent(MixpanelEvents.app_ttc, AtomModel.HostingID, props);

//                #endregion

//                BackgroundVpnModel bgVpnModel = new BackgroundVpnModel();
//                bgVpnModel.ConnectedTime = connectedTime;
//                bgVpnModel.ConnectedProtocol = AtomModel.DialedProtocolForConnectionDetails;
//                WriteBgVpnDetails(bgVpnModel);
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }

//            #endregion

//            if (AtomModel.IsEnableNetworkAnalysis && Environment.Is64BitOperatingSystem)
//            {
//                SpeedMeasurementModel.DuringConnectionSpeedInMBs = null;
//                speedMeasurementManager.StartSpeedMeasurement(SpeedMeasurementType.DuringConnection, shouldAddDelay: true);
//                SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = false;
//                SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = true;
//            }
//        }

//        public void WriteBgVpnDetails(BackgroundVpnModel bgVpnModel)
//        {
//            string settingsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PureVPN".ToLower());

//            string path = System.IO.Path.Combine(settingsPath, "conn.json");

//            string t = JsonConvert.SerializeObject(bgVpnModel);

//            if (!System.IO.Directory.Exists(settingsPath))
//                System.IO.Directory.CreateDirectory(settingsPath);

//            using (StreamWriter outputFile = new StreamWriter(path))
//            {
//                outputFile.WriteAsync(t);
//            }
//        }

//        public PureVPN.Entity.Models.LocationModel GetConnectedLocation()
//        {
//            PureVPN.Entity.Models.LocationModel loc = new LocationModel();
//            try
//            {
//                var con = atomManager.GetConnectedLocation();
//                if (con != null)
//                {
//                    loc.Name = con.Country?.Name;
//                    loc.Ip = con.Ip;

//                    if (loc.City == null)
//                        loc.City = new CityModel();

//                    loc.City.Name = con.City?.Name;
//                }
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//            return loc;
//        }

//        public void DisableIKS()
//        {
//            try
//            {
//                AtomModel.IsIksEnableAtAtom = false;
//                atomManager.DisableIKS();
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private void AtomManagerInstance_Disconnected(object sender, DisconnectedEventArgs e)
//        {
//            try
//            {
//                if (AtomModel.IsEnableNetworkAnalysis && Environment.Is64BitOperatingSystem && SpeedMeasurementModel.DuringConnectionSpeedInMBs?.SpeedTestServer?.ID > default(int) && !e.Cancelled)
//                {
//                    speedMeasurementManager.StartSpeedMeasurement(SpeedMeasurementType.PostConnection, speedTestServer: SpeedMeasurementModel.DuringConnectionSpeedInMBs.SpeedTestServer);
//                    SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = true;
//                    SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = false;
//                }

//                connectionDetails = e.ConnectionDetails;

//                if (e.ConnectionDetails.DisconnectionType == Atom.SDK.Core.Enumerations.DisconnectionMethodType.COCDisconnected)
//                    return;

//                AtomModel.StartConnecting = false;
//                AtomModel.IsConnected = false;
//                AtomModel.IsConnecting = false;
//                AtomModel.ShowCancelButton = false;
//                AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
//                AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;
//                try { RemoveAllSplitApplications(); } catch (Exception ex) { /*sentryService.LoggingException(ex); */}
//                // AtomModel.IsDisconnected = true;
//                StatusModel status = new StatusModel();
//                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
//                status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
//                status.Country = string.Empty;
//                status.Ip = string.Empty;
//                status.Flag = string.Empty;
//                status.IsFetchingFastestServer = false;
//                AtomModel.UserPreferenceServers = new List<ServerPreference>();

//                StatusChanged(status);

//                try
//                {
//                    if (e.ConnectionDetails != null)
//                    {
//                        AfterConnectionModel model = new AfterConnectionModel();
//                        model.NasIdentifier = e?.ConnectionDetails?.NASIdentifier;
//                        model.ServeIP = e?.ConnectionDetails?.ServerIP;
//                        model.ServerAddress = e?.ConnectionDetails?.ServerAddress;
//                        model.ServerType = e?.ConnectionDetails?.ServerType;
//                        model.ProtocolSlug = e?.ConnectionDetails?.Protocol?.ProtocolSlug;

//                        if (e.ConnectionDetails.ConnectionType == Atom.SDK.Core.Enumerations.ConnectionType.Country)
//                        {
//                            model.CountryName = e.ConnectionDetails.Country;
//                            model.ConnectionType = Entity.Enums.ConnectingFrom.Country;
//                        }
//                        else if (e.ConnectionDetails.ConnectionType == Atom.SDK.Core.Enumerations.ConnectionType.City)
//                        {
//                            if (e.ConnectionDetails.City != null)
//                                model.City = mapper.Map<Atom.Core.Models.City, CityModel>(e.ConnectionDetails.City);

//                            model.ConnectionType = Entity.Enums.ConnectingFrom.City;
//                        }

//                        if (e.ConnectionDetails.DisconnectionType != Atom.SDK.Core.Enumerations.DisconnectionMethodType.Cancelled && !e.ConnectionDetails.IsCancelled)
//                            DisconnectedOccured(model);

//                    }
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }

//                if (!e.ConnectionDetails.IsCancelled)
//                {
//                    var props = new MixpanelProperties().MixPanelPropertiesDictionary;

//                    if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerIP))
//                    {
//                        AtomModel.ServerIP = e.ConnectionDetails.ServerIP;
//                        props.Add("server_ip", e.ConnectionDetails.ServerIP);
//                    }

//                    if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerAddress))
//                    {
//                        AtomModel.ServerAddress = e.ConnectionDetails.ServerAddress;
//                        props.Add("server_dns", e.ConnectionDetails.ServerAddress);
//                    }

//                    if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
//                        props.Add("selected_protocol_name", AtomModel.SelectedProtocol);

//                    if (e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.ProtocolSlug))
//                    {
//                        AtomModel.DialedProtocol = e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper();
//                        props.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper());
//                    }

//                    props.Add("auto_dc", e.ConnectionDetails.IsDisconnectedManually ? false : true);

//                    props.Add("selected_location", AtomModel.BeforeConnectionLocation);
//                    props.Add("dialed_location", AtomModel.AfterConnectionLocation);

//                    bool connected_with_desired_location = false;
//                    if (!string.IsNullOrEmpty(AtomModel.BeforeConnectionLocation) && !string.IsNullOrEmpty(AtomModel.AfterConnectionLocation) && AtomModel.BeforeConnectionLocation.ToLower() == AtomModel.AfterConnectionLocation.ToLower())
//                        connected_with_desired_location = true;

//                    props.Add("connected_via_desired_location", connected_with_desired_location);

//                    #region NAS Identifier
//                    props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
//                    props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

//                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        props.Add("is_filtered_server_requested", true);
//                    else
//                        props.Add("is_filtered_server_requested", false);


//                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


//                    props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
//                    props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);

//                    #endregion

//                    try
//                    {
//                        props.Add("connect_via", AtomModel.connectingFrom.ToString());
//                        props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
//                        props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
//                        props.Add("is_split_enabled", AtomModel.IsSplitEnable);

//                        if (AtomModel.LastConnectedTo != Entity.Enums.ConnectedTo.NotConnected)
//                            props.Add("selected_interface", AtomModel.LastConnectedTo.GetEnumDescription());
//                        else
//                            props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
//                    }
//                    catch (Exception ex)
//                    {
//                        //sentryService.LoggingException(ex);
//                    }

//                    if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//                        props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

//                    props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//                    props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//                    props.Add("atom_session_id", e.ConnectionDetails.SessionID);

//                    #region Connection filters
//                    props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//                    props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//                    #endregion

//                    mixpanelService.FireEvent(MixpanelEvents.app_disconnected, AtomModel.HostingID, props);
//                    ResetFilterProperties();
//                }
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        public void RedialFromIKS()
//        {
//            try
//            {
//                AtomModel.IsConnecting = true;
//                AtomModel.IsCancelled = false;
//                AtomModel.IsConnected = false;
//                AtomModel.StartConnecting = true;
//                StatusModel status = new StatusModel();
//                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnecting);
//                status.CurrentStatus = Entity.Enums.CurrentStatus.Connecting;
//                status.Country = string.Empty;
//                status.Ip = string.Empty;
//                status.Flag = string.Empty;
//                StatusChanged(status);
//                Task.Run(() => { Reconnect(); });
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private async void Redial()
//        {
//            try
//            {
//                await Task.Delay(TimeSpan.FromSeconds(1));

//                var IsInternetAvailable = await Utilities.IsInternetAvailable();
//                if (!IsInternetAvailable)
//                {
//                    AtomModel.IsInternetDown = true;
//                    AtomModel.ShowCancelButton = true;
//                }

//                AtomModel.IsConnecting = true;
//                AtomModel.IsCancelled = false;
//                StatusModel status = new StatusModel();
//                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnecting);
//                status.CurrentStatus = Entity.Enums.CurrentStatus.Connecting;
//                status.Country = string.Empty;
//                status.Ip = string.Empty;
//                status.Flag = string.Empty;
//                StatusChanged(status);

//                IsInternetAvailable = await Utilities.IsInternetAvailable();

//                if (IsInternetAvailable)
//                {
//                    Reconnect();
//                }
//                else
//                {
//                    while (AtomModel.IsInternetDown && AtomModel.IsConnecting)
//                    {
//                        if (AtomModel.IsCancelled)
//                            break;

//                        IsInternetAvailable = await Utilities.IsInternetAvailable();
//                        if (IsInternetAvailable)
//                        {
//                            AtomModel.IsInternetDown = false;
//                            Reconnect();
//                        }

//                        await Task.Delay(TimeSpan.FromSeconds(1));
//                    }

//                    if (AtomModel.IsCancelled)
//                    {
//                        AtomModel.IsConnected = false;
//                        AtomModel.IsConnecting = false;
//                        AtomModel.IsInternetDown = false;
//                        AtomModel.IsCancelled = false;
//                        status = new StatusModel();
//                        status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
//                        status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
//                        status.Country = string.Empty;
//                        status.Ip = string.Empty;
//                        status.Flag = string.Empty;
//                        StatusChanged(status);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private void AtomManagerInstance_DialError(object sender, DialErrorEventArgs e)
//        {
//            try
//            {
//                connectionDetails = e.ConnectionDetails;
//                //sentryService.SendVPNConnectionError(e?.Message, e?.Type.ToString(), e?.ConnectionDetails?.Protocol?.ProtocolSlug, e?.ConnectionDetails?.ConnectionMethod, e?.ConnectionDetails?.Country);
//                var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//                props.Add("server_ip", e?.ConnectionDetails?.ServerIP);
//                props.Add("server_dns", e?.ConnectionDetails?.ServerAddress);
//                props.Add("selected_protocol_name", AtomModel.SelectedProtocol);

//                try
//                {
//                    props.Add("connect_via", AtomModel.connectingFrom.ToString());
//                    props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
//                    props.Add("is_split_enabled", AtomModel.IsSplitEnable);

//                    if (AtomModel.IsIksEnable && e.ConnectionDetails.IKSIsEnabled)
//                        props.Add("connection_initiated_by", Entity.Enums.ConnectedTo.AutoReconnect.ToString());
//                    else
//                        props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());

//                    if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
//                        props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
//                    else
//                        props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }


//                #region NAS Identifier
//                try
//                {
//                    props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
//                    props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

//                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        props.Add("is_filtered_server_requested", true);
//                    else
//                        props.Add("is_filtered_server_requested", false);


//                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


//                    props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
//                    props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }

//                #endregion

//                if (e?.Exception?.ErrorCode == 5041)
//                {
//                    #region Mixpanel

//                    props.Add("shown", false);
//                    props.Add("atom_error_code", e?.Exception?.ErrorCode);
//                    props.Add("iks_enabled", AtomModel.IsIksEnable);
//                    if (e.ConnectionDetails != null && e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.Name))
//                        props.Add("dialed_protocol_name", e?.ConnectionDetails?.Protocol?.ProtocolSlug);
//                    else
//                    {
//                        if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
//                            props.Add("dialed_protocol_name", AtomModel.SelectedProtocol);

//                    }
//                    props.Add("selected_location", AtomModel.BeforeConnectionLocation);

//                    if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//                        props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);


//                    props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//                    props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//                    props.Add("atom_session_id", e.ConnectionDetails.SessionID);

//                    #region Connection filters
//                    props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//                    props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//                    #endregion


//                    mixpanelService.FireEvent(MixpanelEvents.app_utc, AtomModel.HostingID, props);

//                    #region IKS blocked internet mixpanel event
//                    if (e != null && e.ConnectionDetails != null && e.ConnectionDetails.IKSIsEnabled)
//                    {
//                        var prop = new MixpanelProperties().MixPanelPropertiesDictionary;
//                        prop.Add("reason", "Internet Issue");
//                        prop.Add("is_split_enabled", AtomModel.IsSplitEnable);
//                        mixpanelService.FireEvent(MixpanelEvents.app_block_internet_iks, AtomModel.HostingID, prop);
//                    }

//                    #endregion

//                    #endregion

//                    Redial();
//                }
//                else
//                {
//                    #region Mixpanel
//                    props.Add("shown", true);
//                    props.Add("dialed_protocol_name", e?.ConnectionDetails?.Protocol?.ProtocolSlug);
//                    props.Add("atom_error_code", e?.Exception?.ErrorCode);
//                    props.Add("iks_enabled", AtomModel.IsIksEnable);
//                    props.Add("selected_location", AtomModel.BeforeConnectionLocation);
//                    // props.Add("iks_reconnecting", e?.ConnectionDetails?.IKSIsEnabled);

//                    if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//                        props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

//                    props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//                    props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//                    props.Add("atom_session_id", e.ConnectionDetails.SessionID);

//                    #region Connection filters
//                    props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//                    props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//                    #endregion

//                    mixpanelService.FireEvent(MixpanelEvents.app_utc, AtomModel.HostingID, props);

//                    #region Iks blocked internet mixpanel event
//                    if (e != null && e.ConnectionDetails != null && e.ConnectionDetails.IKSIsEnabled)
//                    {
//                        var prop = new MixpanelProperties().MixPanelPropertiesDictionary;
//                        prop.Add("reason", "VPN Issue");
//                        prop.Add("is_split_enabled", AtomModel.IsSplitEnable);
//                        prop.Add("atom_session_id", e.ConnectionDetails.SessionID);
//                        mixpanelService.FireEvent(MixpanelEvents.app_block_internet_iks, AtomModel.HostingID, prop);
//                    }
//                    #endregion

//                    #endregion

//                    AtomModel.IsConnected = false;
//                    AtomModel.IsConnecting = false;
//                    AtomModel.ShowCancelButton = false;
//                    AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
//                    AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;
//                    StatusModel status = new StatusModel();
//                    status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
//                    status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
//                    status.Country = string.Empty;
//                    status.Ip = string.Empty;
//                    status.Flag = string.Empty;

//                    status.ErrorCode = e.Exception.ErrorCode;
//                    status.ErrorOccured = true;
//                    status.ErrorMessage = e.Exception.ErrorMessage;
//                    status.ErrorFrom = "Atom";
//                    status.IsIksEnable = e.ConnectionDetails.IKSIsEnabled;


//                    if (e?.Exception?.ErrorCode == 5062)
//                        status.IsFromReconnectOnLaunch = true;
//                    else
//                        status.IsFromReconnectOnLaunch = false;

//                    status.MixPanelPropertiesDictionary = props;

//                    StatusChanged(status);
//                }
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private void AtomManagerInstance_Redialing(object sender, ErrorEventArgs e)
//        {
//            try
//            {
//                connectionDetails = e.ConnectionDetails;

//                #region Mixpanel
//                var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//                props.Add("shown", false);
//                props.Add("server_ip", e.ConnectionDetails.ServerIP);
//                props.Add("server_dns", e.ConnectionDetails.ServerAddress);
//                props.Add("selected_protocol_name", AtomModel.SelectedProtocol);
//                props.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug);
//                props.Add("atom_error_code", e?.Exception?.ErrorCode);
//                props.Add("iks_enabled", AtomModel.IsIksEnable);
//                props.Add("selected_location", AtomModel.BeforeConnectionLocation);
//                // props.Add("iks_reconnecting", e?.ConnectionDetails?.IKSIsEnabled);
//                try
//                {
//                    props.Add("connect_via", AtomModel.connectingFrom.ToString());
//                    props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
//                    props.Add("is_split_enabled", AtomModel.IsSplitEnable);

//                    if (AtomModel.IsIksEnable && e.ConnectionDetails.IKSIsEnabled)
//                        props.Add("connection_initiated_by", Entity.Enums.ConnectedTo.AutoReconnect.ToString());
//                    else
//                        props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());

//                    if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
//                        props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
//                    else
//                        props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }

//                #region NAS Identifier
//                try
//                {
//                    props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
//                    props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

//                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        props.Add("is_filtered_server_requested", true);
//                    else
//                        props.Add("is_filtered_server_requested", false);


//                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                        props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


//                    props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
//                    props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }

//                #endregion

//                if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//                    props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

//                props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//                props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//                props.Add("atom_session_id", e.ConnectionDetails.SessionID);


//                #region Connection filters
//                props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//                props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//                #endregion

//                mixpanelService.FireEvent(MixpanelEvents.app_utc, AtomModel.HostingID, props);


//                #region Iks blocked internet mixpanel event
//                if (e != null && e.ConnectionDetails != null && e.ConnectionDetails.IKSIsEnabled)
//                {
//                    var prop = new MixpanelProperties().MixPanelPropertiesDictionary;
//                    prop.Add("reason", "VPN Issue");
//                    prop.Add("is_split_enabled", AtomModel.IsSplitEnable);
//                    mixpanelService.FireEvent(MixpanelEvents.app_block_internet_iks, AtomModel.HostingID, prop);
//                }
//                #endregion
//                #endregion

//                AtomModel.IsConnected = false;
//                AtomModel.IsConnecting = true;
//                AtomModel.IsCancelled = false;
//                AtomModel.StartConnecting = true;
//                AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
//                AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;
//                //  AtomModel.IsDisconnected = true;
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private async void AtomManagerInstance_StateChanged(object sender, StateChangedEventArgs e)
//        {
//            try
//            {
//                if (e != null && !string.IsNullOrEmpty(e.State.ToString()))
//                {
//                    string state = e.State.ToString().ToUpper();

//                    if (state == "SETTING_VPN_ENTRY"
//                        || state == "SETTING_VPN_CREDENTIALS"
//                        || state == "OPENING_PORT"
//                        || state == "AUTHENTICATING"
//                        || state == "AUTHENTICATED")
//                        AtomModel.ShowCancelButton = false;
//                    else
//                        AtomModel.ShowCancelButton = true;

//                    if (state == "RECONNECTING")
//                    {
//                        AtomModel.IsInternetDown = !await Utilities.IsInternetAvailable();
//                    }
//                    else if (state == "BUILDING_CONFIGURATION")
//                    {
//                        AtomModel.IsInternetDown = false;
//                    }

//                }
//                else
//                {
//                    AtomModel.ShowCancelButton = false;
//                }

//                AtomModel.IsConnected = false;
//                AtomModel.IsConnecting = true;

//                StatusModel status = new StatusModel();
//                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnecting);
//                status.CurrentStatus = Entity.Enums.CurrentStatus.Connecting;
//                status.Country = string.Empty;
//                status.Ip = string.Empty;
//                status.Flag = string.Empty;
//                status.atomstatus = e.State.ToString();
//                StatusChanged(status);
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private void AtomManagerInstance_UnableToAccessInternet(object sender, UnableToAccessInternetEventArgs e)
//        {
//            try
//            {
//                TriggerUTBEventForTelemetry(e);

//                if (e.UTBEventSource == Atom.SDK.Core.Enumerations.UTBEventSource.SessionStart)
//                {
//                    StatusModel status = new StatusModel();
//                    status.IsUTBOccured = true;
//                    StatusChanged(status);
//                }
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }

//        }

//        private void AtomManagerInstance_ConnectedLocation(object sender, ConnectedLocationEventArgs e)
//        {
//            StatusModel status = new StatusModel();
//            status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnected);
//            status.CurrentStatus = Entity.Enums.CurrentStatus.Connected;
//            status.IsIpUpdate = true;

//            try
//            {
//                status.Ip = e.ConnectedLocation.Ip;
//                status.Country = e.ConnectedLocation.Country.Name;
//                status.City = e.ConnectedLocation.City.Name;
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//            finally
//            {
//                SetAfterConnectionLocationValue(status);
//                StatusChanged(status);
//            }
//        }

//        public CountryModel GetFastestServer()
//        {
//            Country country = atomManager.GetRecommendedCountry();
//            return mapper.Map<Atom.Core.Models.Country, CountryModel>(country);
//        }

//        public async Task<CountryModel> GetFastestServerByProtocol(string SelectedProtocol)
//        {
//            LocationFilter filter = new LocationFilter();

//            var protocols = await GetAtomProtocols();

//            if (protocols != null)
//            {
//                if (!string.IsNullOrEmpty(SelectedProtocol))
//                    filter.Protocol = protocols.Where(x => x.Name.ToLower() == SelectedProtocol.ToLower())?.FirstOrDefault();
//                else
//                    filter.Protocol = protocols.Where(x => x.Name.ToLower() == "ikev2")?.FirstOrDefault();
//            }

//            Location location = atomManager.GetRecommendedLocationByFilters(filter);
//            return mapper.Map<Atom.Core.Models.Country, CountryModel>(location?.Country);
//        }

//        public int GetPingForDedicatedIP()
//        {
//            if (AtomModel.dedicatedIP != null && AtomModel.dedicatedIP.body != null && AtomModel.dedicatedIP.body.dedicated_ip_detail != null && !string.IsNullOrEmpty(AtomModel.dedicatedIP.body.dedicated_ip_detail.ip))
//            {
//                DedicatedIPServerPing ping = new DedicatedIPServerPing();
//                ping.ServerAddress = AtomModel.dedicatedIP.body.dedicated_ip_detail.host;
//                var obj = atomManager.PingDedicatedIPServer(ping);
//                return obj.Latency;
//            }
//            return 0;
//        }
//        private void SetAfterConnectionLocationValue(StatusModel status)
//        {
//            try
//            {
//                if (status != null && !string.IsNullOrEmpty(status.Country) && (AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.Country || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.SmartConnect || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.DedicatedIP))
//                    AtomModel.AfterConnectionLocation = status.Country;
//                else if (status != null && !string.IsNullOrEmpty(status.City) && AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.City)
//                    AtomModel.AfterConnectionLocation = status.City;
//                else
//                    AtomModel.AfterConnectionLocation = string.Empty;
//            }
//            catch (Exception exp)
//            {
//                //sentryService.LoggingException(exp);
//            }
//        }


//        private void AtomManagerInstance_PacketsTransmitted(object sender, PacketsTransmittedEventArgs e)
//        {
//            try
//            {
//                StatusModel model = new StatusModel();

//                var bitrcv = e.BytesReceived * 8;
//                var bitsent = e.BytesSent * 8;

//                var rcv = GetAnotherSize(bitrcv - AtomModel.LastByteRecieved);
//                var sent = GetAnotherSize(bitsent - AtomModel.LastByteSent);

//                model.BytesRecieved = rcv.val;
//                model.MeasureUnitRcv = rcv.unit;

//                model.BytesSent = sent.val;
//                model.MeasureUnitSent = sent.unit;


//                AtomModel.LastByteRecieved = bitrcv;
//                AtomModel.LastByteSent = bitsent;

//                PacketTransmitOccured(model);

//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }

//        }


//        public class packetobj
//        {
//            public double val { get; set; }
//            public string unit { get; set; }
//        }

//        public packetobj GetDataSize(double value)
//        {
//            packetobj pk = new packetobj();

//            if (value <= 1024) { pk.val = value; pk.unit = "kbps"; }
//            else if (value <= (1024 * 1024)) { pk.val = Math.Round(value / 1024, 2); pk.unit = "Mbps"; }
//            else { pk.val = Math.Round(value / (1024 * 1024), 2); pk.unit = "Gbps"; }
//            return pk;
//        }

//        public packetobj GetAnotherSize(double ContentLength)
//        {
//            packetobj obj = new packetobj();

//            if (ContentLength >= 1073741824.00)
//            {
//                obj.val = Math.Round(ContentLength / 1073741824.00, 2);
//                obj.unit = "Gbps";
//            }
//            else if (ContentLength >= 1048576.00)
//            {
//                obj.val = Math.Round(ContentLength / 1048576.00, 2);
//                obj.unit = "Mbps";
//            }
//            else if (ContentLength >= 1024.00)
//            {
//                obj.val = Math.Round(ContentLength / 1024.00, 2);
//                obj.unit = "Kbps";
//            }
//            else
//            {
//                obj.val = ContentLength;
//                obj.unit = "bps";

//            }

//            return obj;
//        }

//        private void AtomManagerInstance_AtomInitialized(object sender, AtomInitializedEventArgs e)
//        {
//            InitializingStatusModel initializingStatusModel = new InitializingStatusModel
//            {
//                IsInitializingSuccess = true
//            };
//            InitializingStatusChanged(initializingStatusModel);
//        }

//        private void AtomManagerInstance_AtomDependenciesMissing(object sender, AtomDependenciesMissingEventArgs e)
//        {
//            InitializingStatusModel initializingStatusModel = new InitializingStatusModel
//            {
//                IsInitializingSuccess = false
//            };
//            InitializingStatusChanged(initializingStatusModel);
//        }

//        private void TriggerUTBEventForTelemetry(UnableToAccessInternetEventArgs e)
//        {
//            #region Mixpanel
//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
//            props.Add("server_ip", e?.ConnectionDetails?.ServerIP);
//            props.Add("server_dns", e?.ConnectionDetails?.ServerAddress);
//            props.Add("selected_protocol_name", AtomModel.SelectedProtocol);
//            props.Add("dialed_protocol_name", e?.ConnectionDetails?.Protocol?.ProtocolSlug.ToUpper());

//            props.Add("iks_enabled", AtomModel.IsIksEnable);
//            props.Add("selected_location", AtomModel.BeforeConnectionLocation);
//            props.Add("dialed_location", AtomModel.AfterConnectionLocation);


//            bool connected_with_desired_location = false;
//            if (!string.IsNullOrEmpty(AtomModel.BeforeConnectionLocation) && !string.IsNullOrEmpty(AtomModel.AfterConnectionLocation) && AtomModel.BeforeConnectionLocation.ToLower() == AtomModel.AfterConnectionLocation.ToLower())
//                connected_with_desired_location = true;

//            props.Add("connected_via_desired_location", connected_with_desired_location);

//            #region NAS Identifier
//            try
//            {
//                props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
//                props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

//                if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                    props.Add("is_filtered_server_requested", true);
//                else
//                    props.Add("is_filtered_server_requested", false);


//                if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
//                    props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


//                props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
//                props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);

//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }

//            #endregion


//            try
//            {
//                props.Add("connect_via", AtomModel.connectingFrom.ToString());
//                props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
//                props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
//                props.Add("is_split_enabled", AtomModel.IsSplitEnable);

//                if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
//                    props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
//                else
//                    props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }

//            if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
//                props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

//            props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
//            props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
//            props.Add("atom_session_id", e.ConnectionDetails.SessionID);

//            var connectionTime = atomManager.GetConnectedTime();
//            var duration = (DateTime.Now - connectionTime).TotalSeconds;

//            props.Add(MixpanelProperties.time_since_connection, duration);
//            props.Add(MixpanelProperties.triggered_at, e?.UTBEventSource.GetUTBEventSource());

//            #region Connection filters
//            props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
//            props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
//            #endregion

//            mixpanelService.FireEvent(MixpanelEvents.app_utb, AtomModel.HostingID, props);
//            #endregion
//        }

//        public void ReconnectAfterUTB()
//        {
//            try
//            {
//                AtomModel.IsConnecting = false;
//                StatusModel status = new StatusModel();
//                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
//                status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
//                status.Country = string.Empty;
//                status.Ip = string.Empty;
//                status.Flag = string.Empty;
//                StatusChanged(status);

//                try
//                {
//                    RedialFromIKS();
//                }
//                catch (Exception ex)
//                {
//                    //sentryService.LoggingException(ex);
//                }
//            }
//            catch (Exception ex)
//            {
//                //sentryService.LoggingException(ex);
//            }
//        }

//        private void ResetFilterProperties()
//        {
//            AtomModel.isObfSupported = null;
//            AtomModel.isP2pSupported = null;
//            AtomModel.isQrSupported = null;
//            AtomModel.isTorSupported = null;
//        }

//        private void SpeedMeasurementManager_SpeedMeasurmentData(SpeedMeasurement speedMeasurementData, SpeedMeasurementType speedMeasurementType)
//        {
//            SpeedTestProcessStatus(speedMeasurementType);
//            SendSpeedMeasurementMixpanelSuccessEvent(speedMeasurementData, speedMeasurementType, string.Empty);

//            if (SpeedMeasurmentData != null)
//                SpeedMeasurmentData(speedMeasurementData, speedMeasurementType);
//        }

//        private void SpeedMeasurementManager_SpeedMeasurementError(string errorMessage, SpeedMeasurementType speedMeasurementType)
//        {
//            SpeedTestProcessStatus(speedMeasurementType);
//            SendSpeedMeasurementMixpanelErrorEvent(errorMessage, speedMeasurementType);

//            if (SpeedMeasurementError != null)
//                SpeedMeasurementError(errorMessage, speedMeasurementType);
//        }

//        private void SendSpeedMeasurementMixpanelSuccessEvent(SpeedMeasurement speedMeasurementData, SpeedMeasurementType speedMeasurementType, string errorMessage)
//        {
//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;

//            if (speedMeasurementType == SpeedMeasurementType.PreConnection)
//            {
//                props.Add(MixpanelProperties.speed_check_time, _beforeConnection);
//            }
//            else
//            {
//                props.Add(MixpanelProperties.selected_interface_screen, AtomModel.selectedInterfaceScreen);
//                props.Add(MixpanelProperties.selected_interface, AtomModel.SelectedInterface);
//                props.Add(MixpanelProperties.connection_initiated_by, AtomModel.connectionInitiatedBy);
//                props.Add(MixpanelProperties.selected_protocol_name, AtomModel.SelectedProtocol);
//                props.Add(MixpanelProperties.dialed_protocol_name, AtomModel.DialedProtocol);
//                props.Add(MixpanelProperties.server_ip, AtomModel.ServerIP);
//                props.Add(MixpanelProperties.server_dns, AtomModel.ServerAddress);
//                props.Add(MixpanelProperties.is_expiremented_server, AtomModel.IsExperimentedServer);
//                props.Add(MixpanelProperties.is_expirement_server_requested, AtomModel.IsExperimentServerRequested);
//                props.Add(MixpanelProperties.included_nas_identifiers, includedNasIdentifiers);
//                props.Add(MixpanelProperties.excluded_nas_identifiers, excludedNasIdentifiers);
//                props.Add(MixpanelProperties.connect_via, AtomModel.connectingFrom.ToString());

//                if (speedMeasurementType == SpeedMeasurementType.DuringConnection)
//                {
//                    props.Add(MixpanelProperties.dialed_location, AtomModel.AfterConnectionLocation);
//                    props.Add(MixpanelProperties.speed_check_time, _duringConnection);
//                }
//                else
//                {
//                    props.Add(MixpanelProperties.last_dialed_location, AtomModel.AfterConnectionLocation);
//                    props.Add(MixpanelProperties.speed_check_time, _afterConnection);
//                }
//            }

//            props.Add(MixpanelProperties.download_speed, speedMeasurementData.DownloadSpeedMbs);
//            props.Add(MixpanelProperties.upload_speed, speedMeasurementData.UploadSpeedMbs);

//            mixpanelService.FireEvent(MixpanelEvents.app_user_speed, AtomModel.HostingID, props);
//        }

//        private void SendSpeedMeasurementMixpanelErrorEvent(string errorMessage, SpeedMeasurementType speedMeasurementType)
//        {
//            var props = new MixpanelProperties().MixPanelPropertiesDictionary;

//            if (speedMeasurementType == SpeedMeasurementType.PreConnection)
//                props.Add(MixpanelProperties.speed_check_time, _beforeConnection);
//            else if (speedMeasurementType == SpeedMeasurementType.DuringConnection)
//                props.Add(MixpanelProperties.speed_check_time, _duringConnection);
//            else
//                props.Add(MixpanelProperties.speed_check_time, _afterConnection);

//            props.Add(MixpanelProperties.error_message, errorMessage);
//            mixpanelService.FireEvent(MixpanelEvents.app_user_speed_failed, AtomModel.HostingID, props);
//        }

//        private void SpeedTestProcessStatus(SpeedMeasurementType speedMeasurementType)
//        {
//            if (speedMeasurementType == SpeedMeasurementType.PreConnection ||
//                speedMeasurementType == SpeedMeasurementType.PostConnection)
//                SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = false;
//            else
//                SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = false;
//        }
//    }
//}

//using Atom.BPC;
using Atom.Core.Models;
using Atom.SDK.Core;
using Atom.SDK.Core.Models;
using Atom.SDK.Net;
using AutoMapper;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atom.Core.Extensions;
using static PureVPN.Service.Helper.EventHandlers;
using PureVPN.Service.Helper;
using Atom.Core.Enums;
using Atom.SDK.Core.CustomEventArgs;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using ErrorEventArgs = Atom.SDK.Core.ErrorEventArgs;
using PureVPN.Service.ThirdPartyUtilities;
using PureVPN.SpeedTest.Enums;
using PureVPN.Entity.Delegates;
using PureVPN.SpeedTest;
using static PureVPN.Entity.Delegates.SpeedMeasurementEventHandler;

namespace PureVPN.Service
{
    public class AtomService : IAtomService
    {

        private static AtomManager atomManager { get; set; }
        //private static AtomBPCManager atomBpcManager { get; set; }

        private IMapper mapper;

        private ISentryService sentryService;
        private IMixpanelService mixpanelService;
        private IPureAtomConfigurationService pureAtomConfigurationService;
        private IPureAtomManagerService pureAtomManagerService;
        private IPureAtomBPCManagerService pureAtomBPCManagerService;
        private SpeedMeasurementManager speedMeasurementManager;
        private ConnectionDetails connectionDetails;

        private const string _beforeConnection = "before";
        private const string _duringConnection = "during";
        private const string _afterConnection = "after";

        #region Speed Measurement properties for mixpanel

        private string includedNasIdentifiers;
        private string excludedNasIdentifiers;

        #endregion

        public AtomService(IMapper _mapper, ISentryService _sentryService, IMixpanelService _mixpanelService, IPureAtomConfigurationService _pureAtomConfigurationService, IPureAtomManagerService _pureAtomManagerService, IPureAtomBPCManagerService _pureAtomBPCManagerService)
        {
            this.mapper = _mapper;
            this.sentryService = _sentryService;
            this.mixpanelService = _mixpanelService;
            this.pureAtomConfigurationService = _pureAtomConfigurationService;
            this.pureAtomManagerService = _pureAtomManagerService;
            this.pureAtomBPCManagerService = _pureAtomBPCManagerService;
            this.speedMeasurementManager = new SpeedMeasurementManager();
            this.speedMeasurementManager.SpeedMeasurmentData += SpeedMeasurementManager_SpeedMeasurmentData;
            this.speedMeasurementManager.SpeedMeasurementError += SpeedMeasurementManager_SpeedMeasurementError;
        }

        //This event can cause any method which conforms
        //to CustomStatusEvent to be called.
        public event CustomStatusEvent StatusChanged;
        public event DisconnectStatusEvent DisconnectedOccured;
        public event PacketTransmitEvent PacketTransmitOccured;
        public event InitializingStatusEvent InitializingStatusChanged;
        public event SpeedMeasurmentData SpeedMeasurmentData;
        public event SpeedMeasurementError SpeedMeasurementError;

        /// <summary>
        /// Initializes a singleton of AtomManager
        /// </summary>
        /// <param name="secretKey">Key to be used for Initializing AtomManager instance</param>
        public async Task<bool> Initialize(string secret, string vpnInterfaceName)
        {
            await Task.Run(async () =>
            {
                Service.PureAtomConfigurationService pureAtomConfig = pureAtomConfigurationService.InitializeAtomConfiguration(secret, vpnInterfaceName);

                Service.PureAtomManagerService pureAtomManager = pureAtomManagerService.InitializeAtomManager(pureAtomConfig);
                atomManager = pureAtomManager.AtomMgr;
                RegisterAtomInitializedEvent(AtomManagerInstance_AtomInitialized);
                RegisterAtomDependenciesMissingEvent(AtomManagerInstance_AtomDependenciesMissing);

                PureAtomBPCManagerService pureAtomBPCManager = await pureAtomBPCManagerService.InitializeBPCAtomManager(pureAtomConfig);
                Initialize(pureAtomManager, pureAtomBPCManager);

            }).ConfigureAwait(false);
            return AtomModel.ISSDKInitialized;
        }

        public void Initialize(PureAtomManagerService pureAtomManager, PureAtomBPCManagerService pureAtomBPCManager)
        {
            if (!AtomModel.ISSDKInitialized || (pureAtomManager != null & pureAtomBPCManager != null))
            {
                //atomBpcManager = pureAtomBPCManager.AtomBpcMgr;

                atomManager = pureAtomManager.AtomMgr;
                AtomModel.ISSDKInitialized = true;
                AtomModel.IsConnected = atomManager.GetCurrentVPNStatus() == Atom.SDK.Core.Enumerations.VPNStatus.CONNECTED;
                AtomModel.BackgroundVpnStatus = AtomModel.IsConnected;
            }
        }

        public void InitializePreConnectionUserBaseSpeedMeasurementProcess()
        {
            //Note: If base speed of user found in cache then speed will not be calculated on every app launch
            if (AtomModel.IsEnableNetworkAnalysis && Environment.Is64BitOperatingSystem)
            {
                speedMeasurementManager.StartSpeedMeasurement(SpeedMeasurementType.PreConnection);

                SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = true;
                SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = false;
            }
        }

        public string GetState()
        {
            var a = atomManager.GetCurrentVPNStatus();
            return a.ToString();
        }

        public void RegisterEvents()
        {
            if (!AtomModel.IsEventsRegister)
            {
                RegisterConnectedEvent(AtomManagerInstance_Connected);
                RegisterDisconnectedEvent(AtomManagerInstance_Disconnected);
                RegisterDialErrorEvent(AtomManagerInstance_DialError);
                RegisterRedialingEvent(AtomManagerInstance_Redialing);
                RegisterStateChangedEvent(AtomManagerInstance_StateChanged);
                RegisterOnUnableToAccessInternetChangedEvent(AtomManagerInstance_UnableToAccessInternet);
                RegisterConnectedLocationEvent(AtomManagerInstance_ConnectedLocation);
                RegisterPacketsTransmittedEvent(AtomManagerInstance_PacketsTransmitted);

                AtomModel.IsEventsRegister = true;
            }
        }

        public void SetCredentials()
        {
            if (AtomModel.Username.IsStringNotNullorEmptyorWhitespace() && AtomModel.Password.IsStringNotNullorEmptyorWhitespace())
            {
                atomManager.Credentials = new Credentials("partner5920s11469011", "d1wcahih");
                // atomManager.Credentials = new Credentials(AtomModel.Username.Trim(), AtomModel.Password.Trim());
            }
        }

        // Get Countries
        public async Task<List<CountryModel>> GetCountries()
        {
            return MapAtomCountries(await GetAtomCountries());
        }
        public async Task<List<CountryModel>> GetCountriesWithPing()
        {
            List<Country> countries = await GetAtomCountries();

            var stopWatch = Stopwatch.StartNew();
            List<Country> countriesWithPing = await countries.PingAsync();
            double pingTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();

            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "Country");
            props.Add("app_ping_time", pingTime);
            mixpanelService.FireEvent(MixpanelEvents.app_ping_time, AtomModel.HostingID, props);
            return MapAtomCountries(countriesWithPing);
        }

        public async Task<List<CountryModel>> GetCountriesWithPingByProtocol(string protocol = "")
        {
            List<Country> countries = await GetAtomCountriesByProtocol(protocol);

            var stopWatch = Stopwatch.StartNew();
            List<Country> countriesWithPing = await countries.PingAsync();
            double pingTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();

            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "Country");
            props.Add("app_ping_time", pingTime);
            mixpanelService.FireEvent(MixpanelEvents.app_ping_time, AtomModel.HostingID, props);
            return MapAtomCountries(countriesWithPing);
        }

        public async Task<List<Country>> GetAtomCountries()
        {
            var stopWatch = Stopwatch.StartNew();
            List<Country> countries = atomManager?.GetCountries();
            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();

            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "Country");
            props.Add("app_location_load_time", loadTime);
            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

            return countries;
        }



        public async Task<List<Country>> GetAtomCountriesByProtocol(string protocol = "")
        {
            Protocol obj = new Protocol();
            List<Country> countries = new List<Country>();
            var stopWatch = Stopwatch.StartNew();
            var protocols = await GetAtomProtocols();

            if (protocols != null && !string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
            {
                if (string.IsNullOrEmpty(protocol))
                    obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
                else
                    obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();
            }


            if (!string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
            {
                // countries = await atomBpcManager?.GetCountriesByProtocol(obj);
                countries = atomManager?.GetCountries().Where(x => x.Protocols.Any(p => p.ProtocolSlug.Equals(obj.ProtocolSlug, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            else
                countries = atomManager?.GetCountries();

            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();

            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "Country");
            props.Add("app_location_load_time", loadTime);
            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

            return countries;
        }

        public List<CountryModel> GetAtomCountriesByProtocolSync(string protocol = "")
        {
            try
            {
                Protocol obj = new Protocol();
                var stopWatch = Stopwatch.StartNew();
                var protocols = GetAtomProtocolsSync();

                if (protocols != null)
                {
                    if (string.IsNullOrEmpty(protocol))
                        obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
                    else
                        obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();
                }

                List<Country> countries = atomManager?.GetCountries().Where(x => x.Protocols.Any(p => p.ProtocolSlug.Equals(obj.ProtocolSlug, StringComparison.OrdinalIgnoreCase))).ToList();
                double loadTime = stopWatch.Elapsed.TotalMilliseconds;
                stopWatch.Stop();

                if (countries != null && countries != null && countries.Count > 0)
                    return MapAtomCountries(countries);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public List<CountryModel> MapAtomCountries(List<Country> countries)
        {
            return mapper.Map<List<Atom.Core.Models.Country>, List<CountryModel>>(countries);
        }

        // Get Cities
        public async Task<List<CityModel>> GetCities()
        {
            List<City> cities = await GetAtomCities();
            return await GetCitiesWithPing(cities);
        }
        public async Task<List<CityModel>> GetCitiesByProtocol(string protocol = "")
        {
            List<City> cities = await GetAtomCitiesByProtocol(protocol);
            return await GetCitiesWithPing(cities);
        }

        public List<CityModel> GetCitiesByProtocolSync(string protocol = "")
        {
            List<City> cities = GetAtomCitiesByProtocolSync(protocol);
            return MapAtomCities(cities);
        }
        public async Task<List<CityModel>> GetCitiesWithPing(List<City> cities)
        {
            var stopWatch = Stopwatch.StartNew();
            List<City> citiesWithPing = await cities.PingAsync();
            double pingTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();


            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "City");
            props.Add("app_ping_time", pingTime);
            mixpanelService.FireEvent(MixpanelEvents.app_ping_time, AtomModel.HostingID, props);

            return MapAtomCities(citiesWithPing);
        }
        public async Task<List<City>> GetAtomCities()
        {
            var stopWatch = Stopwatch.StartNew();
            List<City> cities = atomManager?.GetCities();
            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();

            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "City");
            props.Add("app_location_load_time", loadTime);
            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

            return cities;
        }


        public async Task<List<City>> GetAtomCitiesByProtocol(string protocol = "")
        {
            var stopWatch = Stopwatch.StartNew();
            Protocol obj = new Protocol();
            List<City> cities = new List<City>();

            var protocols = await GetAtomProtocols();

            if (protocols != null && !string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
            {
                if (string.IsNullOrEmpty(protocol))
                    obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
                else
                    obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();
            }


            if (!string.IsNullOrEmpty(AtomModel.protoselection) && AtomModel.protoselection.ToLower() != "automatic")
                // cities =  atomManager?.GetCities().Where(x=>x.Protocols.Contains(obj)).ToList();
                cities = atomManager?.GetCities().Where(x => x.Protocols.Any(p => p.ProtocolSlug.Equals(obj.ProtocolSlug, StringComparison.OrdinalIgnoreCase))).ToList();
            else
                cities = atomManager?.GetCities();

            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();

            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "City");
            props.Add("app_location_load_time", loadTime);
            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);

            return cities;
        }

        public List<City> GetAtomCitiesByProtocolSync(string protocol = "")
        {
            var stopWatch = Stopwatch.StartNew();
            Protocol obj = new Protocol();
            var protocols = GetAtomProtocolsSync();

            if (protocols != null)
            {
                if (string.IsNullOrEmpty(protocol))
                    obj = protocols.Where(x => x.Name.ToLower() == AtomModel.protoselection.ToLower())?.FirstOrDefault();
                else
                    obj = protocols.Where(x => x.Name.ToLower() == protocol.ToLower())?.FirstOrDefault();

            }

            // List<City> cities = atomBpcManager?.GetCitiesByProtocol(obj).Result;
            List<City> cities = atomManager?.GetCities().Where(x => x.Protocols.Any(p => p.ProtocolSlug.Equals(obj.ProtocolSlug, StringComparison.OrdinalIgnoreCase))).ToList();
            return cities;
        }

        public List<CityModel> MapAtomCities(List<City> cities)
        {
            return mapper.Map<List<Atom.Core.Models.City>, List<CityModel>>(cities);
        }
        public async Task<List<CityModel>> GetCitiesByCountryWithPing(CountryModel country)
        {
            var con = mapper.Map<CountryModel, Country>(country);

            var stopWatch = Stopwatch.StartNew();
            List<City> cities = atomManager?.GetCities().Where(x => x.Country == con).ToList();
            double loadTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Stop();


            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("type", "City");
            props.Add("app_location_load_time", loadTime);
            mixpanelService.FireEvent(MixpanelEvents.app_location_load_time, AtomModel.HostingID, props);


            List<CityModel> citiesWithPing = await GetCitiesWithPing(cities);
            return citiesWithPing;
        }

        // Get Protocols
        public async Task<List<ProtocolModel>> GetProtocols()
        {
            return MapAtomProtocols(await GetAtomProtocols());
        }
        public async Task<List<Protocol>> GetAtomProtocols()
        {
            List<Protocol> Protocols = atomManager.GetProtocols();

            foreach (Protocol protocol in Protocols)
            {
                protocol.Name = protocol.ProtocolSlug;
            }

            Protocol automaticProtocol = new Protocol();
            automaticProtocol.Name = "Automatic";
            automaticProtocol.ProtocolSlug = "IKEv2";
            automaticProtocol.IsActive = true;
            Protocols.Add(automaticProtocol);

            Protocols = Protocols.OrderBy(x => x.Name).ToList();

            return Protocols;

        }

        public List<Protocol> GetAtomProtocolsSync()
        {
            List<Protocol> Protocols = atomManager.GetProtocols();

            foreach (Protocol protocol in Protocols)
            {
                protocol.Name = protocol.ProtocolSlug;
            }

            Protocol automaticProtocol = new Protocol();
            automaticProtocol.Name = "Automatic";
            automaticProtocol.ProtocolSlug = "IKEv2";
            automaticProtocol.IsActive = true;
            Protocols.Add(automaticProtocol);

            Protocols = Protocols.OrderBy(x => x.Name).ToList();

            return Protocols;

        }
        public List<ProtocolModel> MapAtomProtocols(List<Protocol> protocols)
        {
            return mapper.Map<List<Atom.Core.Models.Protocol>, List<ProtocolModel>>(protocols);
        }

        /// <summary>
        /// Connect using AtomManager
        /// </summary>
        /// <param name="properties">Properties to be used for connection</param>
        public void Connect(VPNProperties properties, PureVPN.Entity.Enums.ConnectingFrom ConnectingFrom, bool isFromChangeServer = false)
        {
            AtomModel.IsExperimentServerRequested = false;
            AtomModel.SpeedtestGroup = string.Empty;
            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("selected_protocol_name", AtomModel.SelectedProtocol.ToUpper());
            props.Add("iks_enabled", AtomModel.IsIksEnable);
            props.Add("selected_location", AtomModel.BeforeConnectionLocation);

            #region NAS Identifier
            props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
            props.Add("is_filtered_server", false);

            if (properties != null && properties.ServerFilters != null && properties.ServerFilters.Count > 0 && AtomModel.IsSameExperienceCheckEnable)
                props.Add("is_filtered_server_requested", true);
            else
                props.Add("is_filtered_server_requested", false);

            if (properties != null && properties.ServerFilters != null && properties.ServerFilters.Count > 0 && AtomModel.IsSameExperienceCheckEnable)
            {
                foreach (var item in properties.ServerFilters)
                {
                    if (item?.FilterType == Atom.SDK.Core.Enumerations.ServerFilterType.Include)
                    {
                        AtomModel.IncludedNasIdentifiers = new List<string>();
                        AtomModel.IncludedNasIdentifiers.Add(item.NASIdentifier);
                    }

                    else if (item?.FilterType == Atom.SDK.Core.Enumerations.ServerFilterType.Exclude)
                    {
                        AtomModel.ExcludedNasIdentifiers = new List<string>();
                        AtomModel.ExcludedNasIdentifiers.Add(item.NASIdentifier);
                    }

                }

                props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
                props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);
            }


            #endregion

            try
            {
                props.Add("connect_via", ConnectingFrom.ToString());
                props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
                props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
                props.Add("is_split_enabled", AtomModel.IsSplitEnable);

                if (AtomModel.ConnectedTo != Entity.Enums.ConnectedTo.NotConnected)
                    props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
                else
                    props.Add("selected_interface", Entity.Enums.ConnectedTo.List);
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }

            if (AtomModel.Experiments != null && AtomModel.Experiments.body != null && AtomModel.Experiments.header.response_code == 200)
            {
                var experimentname = AtomModel.Experiments.body.experiments.Where(x => x.name.ToLower() == "speedtest").FirstOrDefault();

                if (experimentname != null)
                {
                    string groupname = experimentname.group;
                    AtomModel.SpeedtestGroup = groupname;
                }
                else
                    AtomModel.SpeedtestGroup = string.Empty;
            }
            else
                AtomModel.SpeedtestGroup = string.Empty;

            if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
            {
                props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

                if (AtomModel.SpeedtestGroup.ToLower() == "b")
                {
                    properties.ExperimentProperties = new ExperimentProperties() { IsExperimentedUser = true };
                    AtomModel.IsExperimentServerRequested = true;

                    if (SpeedMeasurementModel.PreConnectionBaseSpeedOfUser > default(double))
                        properties.ExperimentProperties.BaseSpeed = SpeedMeasurementModel.PreConnectionBaseSpeedOfUser;
                }
                else
                    AtomModel.IsExperimentServerRequested = false;
            }

            props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);

            #region Connection filters
            props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
            props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
            #endregion

            mixpanelService.FireEvent(MixpanelEvents.app_connect, AtomModel.HostingID, props);

            if (isFromChangeServer)
                mixpanelService.FireEvent(MixpanelEvents.app_click_switch_server, AtomModel.HostingID, props);

            AtomModel.connectingFrom = ConnectingFrom;
            Common.connectTime = DateTime.UtcNow;

            properties.EnableDNSLeakProtection = true;
            properties.EnableIPv6LeakProtection = true;
            properties.EnableIKS = AtomModel.IsIksEnable;
            properties.DoCheckInternetConnectivity = true;
            properties.UseSplitTunneling = AtomModel.IsSplitEnable;
            properties.IsTrackInSessionUTB = true;

            atomManager.Connect(properties);
        }

        // Connect Country
        public async Task ConnectCountry(string countrySlug, string protocolName, bool enableProtocolSwitch, Entity.Enums.ConnectingFrom ConnectingFrom, string NasIdentifier = "", bool GotThumbsUp = false, bool GotThumbsDown = false, bool IsFromChangeServer = false, bool? AddedwithObfFilter = null, bool? AddedWithTorFilter = null, bool? AddedWithQrFilter = null, bool? AddedWithP2pFilter = null, bool? AddedWithVirtualFilter = null)
        {
            var countries = await GetAtomCountries();
            var protocols = await GetAtomProtocols();
            AtomModel.CountrySlugAboutToConnect = countrySlug;

            VPNProperties properties = GetConnectCountryProperties(countries, countrySlug, protocols, protocolName);

            if (AtomModel.IsSameExperienceCheckEnable && !IsFromChangeServer)
                IncludeExcludeNasIdentifier(properties, GotThumbsDown, GotThumbsUp, NasIdentifier);
            else if (IsFromChangeServer)
                ExcludeNasIdentifier(properties);

            if (properties.Protocol != null && !enableProtocolSwitch)
            {
                properties.EnableProtocolSwitch = false;
                properties.SecondaryProtocol = properties.Protocol;
                properties.TertiaryProtocol = properties.Protocol;
                properties.UseSplitTunneling = true;
            }

            Connect(SendFilterToAtom(properties, AddedwithObfFilter, AddedWithQrFilter), ConnectingFrom, IsFromChangeServer);
        }

        private VPNProperties SendFilterToAtom(VPNProperties props, bool? ObfFilter, bool? QRFilter)
        {
            AtomModel.isObfSupported = ObfFilter;
            AtomModel.isQrSupported = QRFilter;

            if (ObfFilter != null)
                props.DialWithOVPNObfuscatedServer = ObfFilter;
            if (QRFilter != null)
                props.DialWithQuantumResistantServer = QRFilter;

            return props;
        }

        public async Task AddSplitApplication(List<ApplicationListModel> model)
        {
            for (int i = 0; i <= model.Count - 1; i++)
            {
                SplitApplication app = new SplitApplication();
                app.CompleteExePath = model[i].Path;

                atomManager.ApplySplitTunneling(app);


                if (model[i].SupportingExe != null && model[i].SupportingExe.Count > 0)
                {
                    var abspath = Path.GetDirectoryName(model[i].Path);

                    if (!string.IsNullOrEmpty(abspath))
                    {
                        foreach (var item in model[i].SupportingExe)
                        {
                            SplitApplication subapp = new SplitApplication();
                            subapp.CompleteExePath = Path.Combine(abspath, item);

                            atomManager.ApplySplitTunneling(subapp);
                        }
                    }
                }
            }
        }

        public async Task RemoveSplitApplication(List<ApplicationListModel> model)
        {
            for (int i = 0; i <= model.Count - 1; i++)
            {
                SplitApplication app = new SplitApplication();
                app.CompleteExePath = model[i].Path;

                atomManager.RemoveSplitTunnelingApplication(app);

                if (model[i].SupportingExe != null && model[i].SupportingExe.Count > 0)
                {
                    var abspath = Path.GetDirectoryName(model[i].Path);

                    if (!string.IsNullOrEmpty(abspath))
                    {
                        foreach (var item in model[i].SupportingExe)
                        {
                            SplitApplication subapp = new SplitApplication();
                            subapp.CompleteExePath = Path.Combine(abspath, item);

                            atomManager.RemoveSplitTunnelingApplication(subapp);
                        }
                    }
                }
            }
        }

        public async Task RemoveAllSplitApplications()
        {
            atomManager.RemoveAllSplitTunnelingApplications();
        }

        private void IncludeExcludeNasIdentifier(VPNProperties properties, bool GotThumbsDown, bool GotThumbsUp, string NasIdentifier)
        {
            var FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Include;

            if (!GotThumbsDown && GotThumbsUp)
                FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Include;
            else if (GotThumbsDown && !GotThumbsUp)
                FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Exclude;

            if (!string.IsNullOrEmpty(NasIdentifier) && ((GotThumbsUp && !GotThumbsDown) || (!GotThumbsUp && GotThumbsDown)))
            {
                ServerFilter filter = new ServerFilter
                {
                    FilterType = FilterType,
                    NASIdentifier = NasIdentifier
                };

                if (Common.ServerChoiceList == null)
                    Common.ServerChoiceList = new List<ServerFilter>();

                if (Common.ServerChoiceList.Any(x => x.NASIdentifier == filter.NASIdentifier))
                    Common.ServerChoiceList.Remove(filter);

                Common.ServerChoiceList.Add(filter);

                if (properties.ServerFilters == null)
                    properties.ServerFilters = new List<ServerFilter>();

                properties.ServerFilters.AddRange(Common.ServerChoiceList);
            }
        }


        private void ExcludeNasIdentifier(VPNProperties properties)
        {
            List<ServerFilter> ServerChoiceList = new List<ServerFilter>();
            var FilterType = Atom.SDK.Core.Enumerations.ServerFilterType.Exclude;

            foreach (var item in AtomModel.UserPreferenceServers)
            {
                if (!string.IsNullOrEmpty(item.Nasidentifier))
                {
                    ServerFilter filter = new ServerFilter
                    {
                        FilterType = FilterType,
                        NASIdentifier = item.Nasidentifier
                    };

                    if (ServerChoiceList.Any(x => x.NASIdentifier == filter.NASIdentifier))
                        ServerChoiceList.Remove(filter);

                    ServerChoiceList.Add(filter);

                    if (properties.ServerFilters == null)
                        properties.ServerFilters = new List<ServerFilter>();
                }
            }
            properties.ServerFilters.AddRange(ServerChoiceList);

        }
        public async Task ConnectDedicatedIP(string dedicatedIP, string protocolName)
        {
            var protocols = await GetAtomProtocols();

            VPNProperties properties = GetConnectDedicatedIPProperties(dedicatedIP, protocols, protocolName);
            Connect(properties, Entity.Enums.ConnectingFrom.DedicatedIP);
        }

        public VPNProperties GetConnectCountryProperties(List<Country> countries, string countrySlug, List<Protocol> protocols, string protocolName)
        {
            var selectedCountry = countries.FirstOrDefault(x => x.CountrySlug.ToLower() == countrySlug.ToLower());
            var selectedProtocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());

            if (selectedCountry != null)
                AtomModel.BeforeConnectionLocation = selectedCountry.Name;
            else
                AtomModel.BeforeConnectionLocation = string.Empty;

            AtomModel.SelectedProtocol = selectedProtocol.Name.ToUpper();

            if (selectedProtocol.Name.ToUpper() == "AUTOMATIC")
                return new VPNProperties(selectedCountry);
            else
                return new VPNProperties(selectedCountry, selectedProtocol);
        }

        public VPNProperties GetConnectDedicatedIPProperties(string dedicatedIP, List<Protocol> protocols, string protocolName)
        {
            var selectedProtocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());
            AtomModel.SelectedProtocol = selectedProtocol.Name.ToUpper();
            return new VPNProperties(dedicatedIP, selectedProtocol);
        }

        // Connect City
        public async Task ConnectCity(string cityName, string protocolName, bool enableProtocolSwitch, string NasIdentifier = "", bool GotThumbsUp = false, bool GotThumbsDown = false, bool IsFromChangeServer = false, bool? AddedwithObfFilter = null, bool? AddedWithTorFilter = null, bool? AddedWithQrFilter = null, bool? AddedWithP2pFilter = null, bool? AddedWithVirtualFilter = null)
        {
            var cities = await GetAtomCitiesByProtocol();
            var protocols = await GetAtomProtocols();
            AtomModel.CountrySlugAboutToConnect = cityName;

            VPNProperties properties = GetConnectCityProperties(cities, cityName, protocols, protocolName);

            if (AtomModel.IsSameExperienceCheckEnable && !IsFromChangeServer)
                IncludeExcludeNasIdentifier(properties, GotThumbsDown, GotThumbsUp, NasIdentifier);
            else if (IsFromChangeServer)
                ExcludeNasIdentifier(properties);

            if (properties.Protocol != null && !enableProtocolSwitch)
            {
                properties.EnableProtocolSwitch = false;
                properties.SecondaryProtocol = properties.Protocol;
                properties.TertiaryProtocol = properties.Protocol;
            }

            Connect(SendFilterToAtom(properties, AddedwithObfFilter, AddedWithQrFilter), Entity.Enums.ConnectingFrom.City, IsFromChangeServer);
        }
        public VPNProperties GetConnectCityProperties(List<City> cities, string cityName, List<Protocol> protocols, string protocolName)
        {
            var selectedCity = cities.FirstOrDefault(x => x.Name.ToLower() == cityName.ToLower());
            var selectedProtocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());

            if (selectedCity != null)
                AtomModel.BeforeConnectionLocation = selectedCity.Name;
            else
                AtomModel.BeforeConnectionLocation = string.Empty;

            AtomModel.SelectedProtocol = selectedProtocol.Name.ToUpper();

            if (selectedProtocol.Name.ToUpper() == "AUTOMATIC")
                return new VPNProperties(selectedCity);
            else
                return new VPNProperties(selectedCity, selectedProtocol);

        }

        // Quick Connect
        public async void QuickConnect(string ProtocolName, bool enableProtocolSwitch)
        {
            List<Protocol> protocols = await GetAtomProtocols();
            VPNProperties properties = await GetQuickConnectProperties(protocols, ProtocolName);

            if (properties.Protocol != null && !enableProtocolSwitch)
            {
                properties.EnableProtocolSwitch = false;
                properties.SecondaryProtocol = properties.Protocol;
                properties.TertiaryProtocol = properties.Protocol;
            }

            Connect(properties, Entity.Enums.ConnectingFrom.SmartConnect);
        }
        public async Task<VPNProperties> GetQuickConnectProperties(List<Protocol> protocols, string protocolName)
        {
            var protocol = new Protocol();

            if (protocols == null || protocols.Count() == 0)
                protocols = await GetAtomProtocols();

            protocol = protocols.FirstOrDefault(x => x.Name.ToLower() == protocolName.ToLower());
            var tagsList = new List<SmartConnectTag>
            {
                SmartConnectTag.AUTOMATIC,
                SmartConnectTag.PAID
            };

            AtomModel.BeforeConnectionLocation = string.Empty;
            AtomModel.SelectedProtocol = protocol.Name.ToUpper();

            return new VPNProperties(protocol, tagsList);
        }

        /// <summary>
        /// Disconnects the VPN
        /// </summary>
        public void Disconnect(bool isAsync = true)
        {
            try
            {
                if (isAsync)
                    Task.Run(() => { try { atomManager.Disconnect(); } catch (Exception ex) { /*sentryService.LoggingException(ex);*/ } });
                else
                    atomManager.Disconnect();
                // AtomModel.IsDisconnected = true;
                AtomModel.IsConnected = false;
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        /// <summary>
        /// Cancels the ongoing VPN connection
        /// </summary>
        public void Cancel()
        {
            try
            {
                atomManager.Cancel();
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        /// <summary>
        /// Reconnect VPN connection
        /// </summary>
        public void Reconnect(bool IsFromAutoConnectOnLaunch = false)
        {
            try
            {
                if (IsFromAutoConnectOnLaunch)
                    Common.connectTime = DateTime.UtcNow;

                atomManager.ReConnect();
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        /// <summary>
        /// Registers Connected Event
        /// </summary>
        /// <param name="onConnected">EventHandler for Connected event</param>
        public void RegisterConnectedEvent(EventHandler<ConnectedEventArgs> onConnected)
        {
            atomManager.Connected += onConnected;
        }

        /// <summary>
        /// Registers PacketsTransmitted Event
        /// </summary>
        /// <param name="onPacketsTransmitted">EventHandler for Connected event</param>
        public void RegisterPacketsTransmittedEvent(EventHandler<PacketsTransmittedEventArgs> onPacketsTransmitted)
        {
            atomManager.PacketsTransmitted += onPacketsTransmitted;
        }

        /// <summary>
        /// Registers Disconnected Event
        /// </summary>
        /// <param name="onConnect">EventHandler for Disconnected event</param>
        public void RegisterDisconnectedEvent(EventHandler<DisconnectedEventArgs> onDisconnected)
        {
            atomManager.Disconnected += onDisconnected;
        }

        /// <summary>
        /// Registers DialError Event
        /// </summary>
        /// <param name="onConnect">EventHandler for DialError event</param>
        public void RegisterDialErrorEvent(EventHandler<DialErrorEventArgs> onDialError)
        {
            atomManager.DialError += onDialError;
        }

        /// <summary>
        /// Registers StateChanged Event
        /// </summary>
        /// <param name="onConnect">EventHandler for StateChanged event</param>
        public void RegisterStateChangedEvent(EventHandler<StateChangedEventArgs> onStateChanged)
        {
            atomManager.StateChanged += onStateChanged;
        }

        /// <summary>
        /// Registers Redialing Event
        /// </summary>
        /// <param name="onConnect">EventHandler for Redialing event</param>
        public void RegisterRedialingEvent(EventHandler<ErrorEventArgs> onRedial)
        {
            atomManager.Redialing += onRedial;
        }

        /// <summary>
        /// Fetches the list of smart countries using AtomManager instance
        /// </summary>
        /// <returns>List of allowed Countries</returns>
        public List<Country> GetSmartCountries()
        {
            return atomManager.GetCountriesForSmartDialing();
        }

        /// <summary>
        /// Registers UnableToAccessInternet Event
        /// </summary>
        /// <param name="onUnableToAccessInternet">EventHandler for UnableToAccessInternet event</param>
        public void RegisterOnUnableToAccessInternetChangedEvent(EventHandler<UnableToAccessInternetEventArgs> onUnableToAccessInternet)
        {
            atomManager.OnUnableToAccessInternet += onUnableToAccessInternet;
        }

        public void RegisterConnectedLocationEvent(EventHandler<ConnectedLocationEventArgs> onConnectedLocation)
        {
            atomManager.ConnectedLocation += onConnectedLocation;
        }

        public void RegisterAtomInitializedEvent(EventHandler<AtomInitializedEventArgs> onAtomInitialized)
        {
            atomManager.AtomInitialized += onAtomInitialized;
        }

        public void RegisterAtomDependenciesMissingEvent(EventHandler<AtomDependenciesMissingEventArgs> onAtomDependenciesMissing)
        {
            atomManager.AtomDependenciesMissing += onAtomDependenciesMissing;
        }

        private void AtomManagerInstance_Connected(object sender, ConnectedEventArgs e)
        {
            connectionDetails = e.ConnectionDetails;
            DateTime connectedTime = DateTime.UtcNow;

            string country = string.Empty;
            string flag = string.Empty;
            AtomModel.IsConnected = true;
            AtomModel.IsCancelled = false;
            AtomModel.IsConnecting = false;
            AtomModel.ShowCancelButton = false;
            AtomModel.StartConnecting = false;
            AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
            AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;

            if (e.ConnectionDetails.City != null)
            {
                AtomModel.IsConnectedToCity = true;
                AtomModel.LastConnectedCityName = e.ConnectionDetails.City.Name;
                AtomModel.LastConnectedCountryName = string.Empty;
            }
            else
            {
                AtomModel.IsConnectedToCity = false;
                AtomModel.LastConnectedCityName = string.Empty;
                AtomModel.LastConnectedCountryName = e.ConnectionDetails.Country;
            }

            StatusModel status = new StatusModel();
            status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnected);
            status.CurrentStatus = Entity.Enums.CurrentStatus.Connected;
            status.City = e.ConnectionDetails.City?.Name;
            status.ServeIP = e.ConnectionDetails.ServerIP;

            if (!string.IsNullOrEmpty(country))
            {
                status.Country = country;
                status.Flag = flag;
            }

            StatusChanged(status);

            try
            {
                if (status != null && !string.IsNullOrEmpty(status.Country) && (AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.Country || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.SmartConnect || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.DedicatedIP))
                    AtomModel.AfterConnectionLocation = status.Country;
                else if (status != null && !string.IsNullOrEmpty(status.City) && AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.City)
                    AtomModel.AfterConnectionLocation = status.City;
                else
                    AtomModel.AfterConnectionLocation = string.Empty;
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }


            if (AtomModel.SelectedAppsForSplit != null && AtomModel.SelectedAppsForSplit.Count > 0)
            {
                AddSplitApplication(AtomModel.SelectedAppsForSplit);
            }

            ServerPreference pref = new ServerPreference();
            pref.GotThumbsDown = true;
            pref.GotThumbsUp = false;
            pref.Nasidentifier = e.ConnectionDetails.NASIdentifier;
            pref.IsConnectedViaCountry = !AtomModel.IsConnectedToCity;
            pref.SelectedProtocol = AtomModel.SelectedProtocol;
            pref.LocationSlug = AtomModel.CountrySlugAboutToConnect;
            pref.ConnectingFrom = AtomModel.connectingFrom;

            if (AtomModel.UserPreferenceServers == null)
                AtomModel.UserPreferenceServers = new List<ServerPreference>();

            AtomModel.UserPreferenceServers.Add(pref);

            #region Mixpanel

            try
            {
                #region Connected event

                var propsConn = new MixpanelProperties().MixPanelPropertiesDictionary;

                if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerIP))
                {
                    AtomModel.ServerIP = e.ConnectionDetails.ServerIP;
                    propsConn.Add("server_ip", e.ConnectionDetails.ServerIP);
                }

                if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerAddress))
                {
                    AtomModel.ServerAddress = e.ConnectionDetails.ServerAddress;
                    propsConn.Add("server_dns", e.ConnectionDetails.ServerAddress);
                }

                if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
                {
                    propsConn.Add("selected_protocol_name", AtomModel.SelectedProtocol);
                }

                if (e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.ProtocolSlug))
                {
                    AtomModel.DialedProtocol = e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper();
                    propsConn.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper());
                    AtomModel.DialedProtocolForConnectionDetails = e.ConnectionDetails?.Protocol?.ProtocolSlug;
                }

                propsConn.Add("selected_location", AtomModel.BeforeConnectionLocation);
                propsConn.Add("dialed_location", AtomModel.AfterConnectionLocation);

                propsConn.Add("iks_enabled", AtomModel.IsIksEnable);

                bool connected_with_desired_location = false;
                if (!string.IsNullOrEmpty(AtomModel.BeforeConnectionLocation) && !string.IsNullOrEmpty(AtomModel.AfterConnectionLocation) && AtomModel.BeforeConnectionLocation.ToLower() == AtomModel.AfterConnectionLocation.ToLower())
                    connected_with_desired_location = true;

                propsConn.Add("connected_via_desired_location", connected_with_desired_location);

                try
                {
                    propsConn.Add("connect_via", AtomModel.connectingFrom.ToString());

                    propsConn.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());

                    propsConn.Add("is_split_enabled", AtomModel.IsSplitEnable);

                    if (AtomModel.IsIksEnable && e.ConnectionDetails.IKSIsEnabled)
                    {
                        propsConn.Add("connection_initiated_by", Entity.Enums.ConnectedTo.AutoReconnect.ToString());
                    }
                    else
                    {
                        propsConn.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
                    }

                    if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
                    {
                        propsConn.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
                        AtomModel.SelectedInterface = Entity.Enums.ConnectedTo.List.ToString();
                    }
                    else
                    {
                        propsConn.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
                        AtomModel.SelectedInterface = AtomModel.ConnectedTo.GetEnumDescription();
                    }

                    propsConn.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
                    propsConn.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }

                #region NAS Identifier
                try
                {
                    propsConn.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
                    propsConn.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        propsConn.Add("is_filtered_server_requested", true);
                    else
                        propsConn.Add("is_filtered_server_requested", false);


                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        propsConn.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


                    propsConn.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
                    propsConn.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);

                    if (AtomModel.IncludedNasIdentifiers != null)
                        includedNasIdentifiers = string.Join(",", AtomModel.IncludedNasIdentifiers.ToArray());

                    if (AtomModel.ExcludedNasIdentifiers != null)
                        excludedNasIdentifiers = string.Join(",", AtomModel.ExcludedNasIdentifiers.ToArray());
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }

                #endregion
                if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
                    propsConn.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

                propsConn.Add("atom_session_id", e.ConnectionDetails.SessionID);

                #region Connection filters
                propsConn.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
                propsConn.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
                #endregion

                mixpanelService.FireEvent(MixpanelEvents.app_connected, AtomModel.HostingID, propsConn);

                #endregion

                #region TTC event
                var props = new MixpanelProperties().MixPanelPropertiesDictionary;

                double duration = 0;
                try { duration = (connectedTime.Subtract(Common.connectTime)).TotalSeconds; } catch (Exception ex) { /*sentryService.LoggingException(ex);*/ }

                if (e.ConnectionDetails.TotalTimeTakenToConnect > 0)
                    props.Add("time_to_connect_server", Math.Round(e.ConnectionDetails.TotalTimeTakenToConnect, 2));
                else
                    props.Add("time_to_connect_server", 0);

                if (e.ConnectionDetails.TimeTakenToFindFastestServer > 0)
                    props.Add("time_to_find_server", Math.Round(e.ConnectionDetails.TimeTakenToFindFastestServer, 2));
                else
                    props.Add("time_to_find_server", 0);

                if (duration > 0)
                    props.Add("$duration", Math.Round(duration, 2));

                props.Add("server_ip", e.ConnectionDetails.ServerIP);
                props.Add("server_dns", e.ConnectionDetails.ServerAddress);

                if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
                    props.Add("selected_protocol_name", AtomModel.SelectedProtocol);

                if (e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.ProtocolSlug))
                    props.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper());

                props.Add("selected_location", AtomModel.BeforeConnectionLocation);
                props.Add("dialed_location", AtomModel.AfterConnectionLocation);
                props.Add("connect_via", AtomModel.connectingFrom.ToString());
                props.Add("connected_via_desired_location", connected_with_desired_location);

                if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
                    props.Add("selected_interface", Entity.Enums.ConnectedTo.Location.ToString());
                else
                    props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());

                props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
                AtomModel.IsExperimentedServer = e.ConnectionDetails.IsExperimentedServer;
                props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
                props.Add("is_split_enabled", AtomModel.IsSplitEnable);
                props.Add("atom_session_id", e.ConnectionDetails.SessionID);

                #region Connection filters
                props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
                props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
                #endregion


                mixpanelService.FireEvent(MixpanelEvents.app_ttc, AtomModel.HostingID, props);

                #endregion

                BackgroundVpnModel bgVpnModel = new BackgroundVpnModel();
                bgVpnModel.ConnectedTime = connectedTime;
                bgVpnModel.ConnectedProtocol = AtomModel.DialedProtocolForConnectionDetails;
                WriteBgVpnDetails(bgVpnModel);
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }

            #endregion

            if (AtomModel.IsEnableNetworkAnalysis && Environment.Is64BitOperatingSystem)
            {
                SpeedMeasurementModel.DuringConnectionSpeedInMBs = null;
                speedMeasurementManager.StartSpeedMeasurement(SpeedMeasurementType.DuringConnection, shouldAddDelay: true);
                SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = false;
                SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = true;
            }
        }

        public void WriteBgVpnDetails(BackgroundVpnModel bgVpnModel)
        {
            string settingsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PureVPN".ToLower());

            string path = System.IO.Path.Combine(settingsPath, "conn.json");

            string t = JsonConvert.SerializeObject(bgVpnModel);

            if (!System.IO.Directory.Exists(settingsPath))
                System.IO.Directory.CreateDirectory(settingsPath);

            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteAsync(t);
            }
        }

        public PureVPN.Entity.Models.LocationModel GetConnectedLocation()
        {
            PureVPN.Entity.Models.LocationModel loc = new LocationModel();
            try
            {
                var con = atomManager.GetConnectedLocation();
                if (con != null)
                {
                    loc.Name = con.Country?.Name;
                    loc.Ip = con.Ip;

                    if (loc.City == null)
                        loc.City = new CityModel();

                    loc.City.Name = con.City?.Name;
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
            return loc;
        }

        public void DisableIKS()
        {
            try
            {
                AtomModel.IsIksEnableAtAtom = false;
                atomManager.DisableIKS();
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private void AtomManagerInstance_Disconnected(object sender, DisconnectedEventArgs e)
        {
            try
            {
                if (AtomModel.IsEnableNetworkAnalysis && Environment.Is64BitOperatingSystem && SpeedMeasurementModel.DuringConnectionSpeedInMBs?.SpeedTestServer?.ID > default(int) && !e.Cancelled)
                {
                    speedMeasurementManager.StartSpeedMeasurement(SpeedMeasurementType.PostConnection, speedTestServer: SpeedMeasurementModel.DuringConnectionSpeedInMBs.SpeedTestServer);
                    SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = true;
                    SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = false;
                }

                connectionDetails = e.ConnectionDetails;

                if (e.ConnectionDetails.DisconnectionType == Atom.SDK.Core.Enumerations.DisconnectionMethodType.COCDisconnected)
                    return;

                AtomModel.StartConnecting = false;
                AtomModel.IsConnected = false;
                AtomModel.IsConnecting = false;
                AtomModel.ShowCancelButton = false;
                AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
                AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;
                try { RemoveAllSplitApplications(); } catch (Exception ex) { /*sentryService.LoggingException(ex); */}
                // AtomModel.IsDisconnected = true;
                StatusModel status = new StatusModel();
                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
                status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
                status.Country = string.Empty;
                status.Ip = string.Empty;
                status.Flag = string.Empty;
                status.IsFetchingFastestServer = false;
                AtomModel.UserPreferenceServers = new List<ServerPreference>();

                StatusChanged(status);

                try
                {
                    if (e.ConnectionDetails != null)
                    {
                        AfterConnectionModel model = new AfterConnectionModel();
                        model.NasIdentifier = e?.ConnectionDetails?.NASIdentifier;
                        model.ServeIP = e?.ConnectionDetails?.ServerIP;
                        model.ServerAddress = e?.ConnectionDetails?.ServerAddress;
                        model.ServerType = e?.ConnectionDetails?.ServerType;
                        model.ProtocolSlug = e?.ConnectionDetails?.Protocol?.ProtocolSlug;

                        if (e.ConnectionDetails.ConnectionType == Atom.SDK.Core.Enumerations.ConnectionType.Country)
                        {
                            model.CountryName = e.ConnectionDetails.Country;
                            model.ConnectionType = Entity.Enums.ConnectingFrom.Country;
                        }
                        else if (e.ConnectionDetails.ConnectionType == Atom.SDK.Core.Enumerations.ConnectionType.City)
                        {
                            if (e.ConnectionDetails.City != null)
                                model.City = mapper.Map<Atom.Core.Models.City, CityModel>(e.ConnectionDetails.City);

                            model.ConnectionType = Entity.Enums.ConnectingFrom.City;
                        }

                        if (e.ConnectionDetails.DisconnectionType != Atom.SDK.Core.Enumerations.DisconnectionMethodType.Cancelled && !e.ConnectionDetails.IsCancelled)
                            DisconnectedOccured(model);

                    }
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }

                if (!e.ConnectionDetails.IsCancelled)
                {
                    var props = new MixpanelProperties().MixPanelPropertiesDictionary;

                    if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerIP))
                    {
                        AtomModel.ServerIP = e.ConnectionDetails.ServerIP;
                        props.Add("server_ip", e.ConnectionDetails.ServerIP);
                    }

                    if (!string.IsNullOrEmpty(e.ConnectionDetails.ServerAddress))
                    {
                        AtomModel.ServerAddress = e.ConnectionDetails.ServerAddress;
                        props.Add("server_dns", e.ConnectionDetails.ServerAddress);
                    }

                    if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
                        props.Add("selected_protocol_name", AtomModel.SelectedProtocol);

                    if (e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.ProtocolSlug))
                    {
                        AtomModel.DialedProtocol = e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper();
                        props.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug?.ToUpper());
                    }

                    props.Add("auto_dc", e.ConnectionDetails.IsDisconnectedManually ? false : true);

                    props.Add("selected_location", AtomModel.BeforeConnectionLocation);
                    props.Add("dialed_location", AtomModel.AfterConnectionLocation);

                    bool connected_with_desired_location = false;
                    if (!string.IsNullOrEmpty(AtomModel.BeforeConnectionLocation) && !string.IsNullOrEmpty(AtomModel.AfterConnectionLocation) && AtomModel.BeforeConnectionLocation.ToLower() == AtomModel.AfterConnectionLocation.ToLower())
                        connected_with_desired_location = true;

                    props.Add("connected_via_desired_location", connected_with_desired_location);

                    #region NAS Identifier
                    props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
                    props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        props.Add("is_filtered_server_requested", true);
                    else
                        props.Add("is_filtered_server_requested", false);


                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


                    props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
                    props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);

                    #endregion

                    try
                    {
                        props.Add("connect_via", AtomModel.connectingFrom.ToString());
                        props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
                        props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
                        props.Add("is_split_enabled", AtomModel.IsSplitEnable);

                        if (AtomModel.LastConnectedTo != Entity.Enums.ConnectedTo.NotConnected)
                            props.Add("selected_interface", AtomModel.LastConnectedTo.GetEnumDescription());
                        else
                            props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
                    }
                    catch (Exception ex)
                    {
                        //sentryService.LoggingException(ex);
                    }

                    if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
                        props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

                    props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
                    props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
                    props.Add("atom_session_id", e.ConnectionDetails.SessionID);

                    #region Connection filters
                    props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
                    props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
                    #endregion

                    mixpanelService.FireEvent(MixpanelEvents.app_disconnected, AtomModel.HostingID, props);
                    ResetFilterProperties();
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        public void RedialFromIKS()
        {
            try
            {
                AtomModel.IsConnecting = true;
                AtomModel.IsCancelled = false;
                AtomModel.IsConnected = false;
                AtomModel.StartConnecting = true;
                StatusModel status = new StatusModel();
                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnecting);
                status.CurrentStatus = Entity.Enums.CurrentStatus.Connecting;
                status.Country = string.Empty;
                status.Ip = string.Empty;
                status.Flag = string.Empty;
                StatusChanged(status);
                Task.Run(() => { Reconnect(); });
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private async void Redial()
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                var IsInternetAvailable = await Utilities.IsInternetAvailable();
                if (!IsInternetAvailable)
                {
                    AtomModel.IsInternetDown = true;
                    AtomModel.ShowCancelButton = true;
                }

                AtomModel.IsConnecting = true;
                AtomModel.IsCancelled = false;
                StatusModel status = new StatusModel();
                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnecting);
                status.CurrentStatus = Entity.Enums.CurrentStatus.Connecting;
                status.Country = string.Empty;
                status.Ip = string.Empty;
                status.Flag = string.Empty;
                StatusChanged(status);

                IsInternetAvailable = await Utilities.IsInternetAvailable();

                if (IsInternetAvailable)
                {
                    Reconnect();
                }
                else
                {
                    while (AtomModel.IsInternetDown && AtomModel.IsConnecting)
                    {
                        if (AtomModel.IsCancelled)
                            break;

                        IsInternetAvailable = await Utilities.IsInternetAvailable();
                        if (IsInternetAvailable)
                        {
                            AtomModel.IsInternetDown = false;
                            Reconnect();
                        }

                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }

                    if (AtomModel.IsCancelled)
                    {
                        AtomModel.IsConnected = false;
                        AtomModel.IsConnecting = false;
                        AtomModel.IsInternetDown = false;
                        AtomModel.IsCancelled = false;
                        status = new StatusModel();
                        status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
                        status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
                        status.Country = string.Empty;
                        status.Ip = string.Empty;
                        status.Flag = string.Empty;
                        StatusChanged(status);
                    }
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private void AtomManagerInstance_DialError(object sender, DialErrorEventArgs e)
        {
            try
            {
                connectionDetails = e.ConnectionDetails;
                //sentryService.SendVPNConnectionError(e?.Message, e?.Type.ToString(), e?.ConnectionDetails?.Protocol?.ProtocolSlug, e?.ConnectionDetails?.ConnectionMethod, e?.ConnectionDetails?.Country);
                var props = new MixpanelProperties().MixPanelPropertiesDictionary;
                props.Add("server_ip", e?.ConnectionDetails?.ServerIP);
                props.Add("server_dns", e?.ConnectionDetails?.ServerAddress);
                props.Add("selected_protocol_name", AtomModel.SelectedProtocol);

                try
                {
                    props.Add("connect_via", AtomModel.connectingFrom.ToString());
                    props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
                    props.Add("is_split_enabled", AtomModel.IsSplitEnable);

                    if (AtomModel.IsIksEnable && e.ConnectionDetails.IKSIsEnabled)
                        props.Add("connection_initiated_by", Entity.Enums.ConnectedTo.AutoReconnect.ToString());
                    else
                        props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());

                    if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
                        props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
                    else
                        props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }


                #region NAS Identifier
                try
                {
                    props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
                    props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        props.Add("is_filtered_server_requested", true);
                    else
                        props.Add("is_filtered_server_requested", false);


                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


                    props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
                    props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }

                #endregion

                if (e?.Exception?.ErrorCode == 5041)
                {
                    #region Mixpanel

                    props.Add("shown", false);
                    props.Add("atom_error_code", e?.Exception?.ErrorCode);
                    props.Add("iks_enabled", AtomModel.IsIksEnable);
                    if (e.ConnectionDetails != null && e.ConnectionDetails.Protocol != null && !string.IsNullOrEmpty(e.ConnectionDetails.Protocol.Name))
                        props.Add("dialed_protocol_name", e?.ConnectionDetails?.Protocol?.ProtocolSlug);
                    else
                    {
                        if (!string.IsNullOrEmpty(AtomModel.SelectedProtocol))
                            props.Add("dialed_protocol_name", AtomModel.SelectedProtocol);

                    }
                    props.Add("selected_location", AtomModel.BeforeConnectionLocation);

                    if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
                        props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);


                    props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
                    props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
                    props.Add("atom_session_id", e.ConnectionDetails.SessionID);

                    #region Connection filters
                    props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
                    props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
                    #endregion


                    mixpanelService.FireEvent(MixpanelEvents.app_utc, AtomModel.HostingID, props);

                    #region IKS blocked internet mixpanel event
                    if (e != null && e.ConnectionDetails != null && e.ConnectionDetails.IKSIsEnabled)
                    {
                        var prop = new MixpanelProperties().MixPanelPropertiesDictionary;
                        prop.Add("reason", "Internet Issue");
                        prop.Add("is_split_enabled", AtomModel.IsSplitEnable);
                        mixpanelService.FireEvent(MixpanelEvents.app_block_internet_iks, AtomModel.HostingID, prop);
                    }

                    #endregion

                    #endregion

                    Redial();
                }
                else
                {
                    #region Mixpanel
                    props.Add("shown", true);
                    props.Add("dialed_protocol_name", e?.ConnectionDetails?.Protocol?.ProtocolSlug);
                    props.Add("atom_error_code", e?.Exception?.ErrorCode);
                    props.Add("iks_enabled", AtomModel.IsIksEnable);
                    props.Add("selected_location", AtomModel.BeforeConnectionLocation);
                    // props.Add("iks_reconnecting", e?.ConnectionDetails?.IKSIsEnabled);

                    if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
                        props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

                    props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
                    props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
                    props.Add("atom_session_id", e.ConnectionDetails.SessionID);

                    #region Connection filters
                    props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
                    props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
                    #endregion

                    mixpanelService.FireEvent(MixpanelEvents.app_utc, AtomModel.HostingID, props);

                    #region Iks blocked internet mixpanel event
                    if (e != null && e.ConnectionDetails != null && e.ConnectionDetails.IKSIsEnabled)
                    {
                        var prop = new MixpanelProperties().MixPanelPropertiesDictionary;
                        prop.Add("reason", "VPN Issue");
                        prop.Add("is_split_enabled", AtomModel.IsSplitEnable);
                        prop.Add("atom_session_id", e.ConnectionDetails.SessionID);
                        mixpanelService.FireEvent(MixpanelEvents.app_block_internet_iks, AtomModel.HostingID, prop);
                    }
                    #endregion

                    #endregion

                    AtomModel.IsConnected = false;
                    AtomModel.IsConnecting = false;
                    AtomModel.ShowCancelButton = false;
                    AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
                    AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;
                    StatusModel status = new StatusModel();
                    status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
                    status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
                    status.Country = string.Empty;
                    status.Ip = string.Empty;
                    status.Flag = string.Empty;

                    status.ErrorCode = e.Exception.ErrorCode;
                    status.ErrorOccured = true;
                    status.ErrorMessage = e.Exception.ErrorMessage;
                    status.ErrorFrom = "Atom";
                    status.IsIksEnable = e.ConnectionDetails.IKSIsEnabled;


                    if (e?.Exception?.ErrorCode == 5062)
                        status.IsFromReconnectOnLaunch = true;
                    else
                        status.IsFromReconnectOnLaunch = false;

                    status.MixPanelPropertiesDictionary = props;

                    StatusChanged(status);
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private void AtomManagerInstance_Redialing(object sender, ErrorEventArgs e)
        {
            try
            {
                connectionDetails = e.ConnectionDetails;

                #region Mixpanel
                var props = new MixpanelProperties().MixPanelPropertiesDictionary;
                props.Add("shown", false);
                props.Add("server_ip", e.ConnectionDetails.ServerIP);
                props.Add("server_dns", e.ConnectionDetails.ServerAddress);
                props.Add("selected_protocol_name", AtomModel.SelectedProtocol);
                props.Add("dialed_protocol_name", e.ConnectionDetails?.Protocol?.ProtocolSlug);
                props.Add("atom_error_code", e?.Exception?.ErrorCode);
                props.Add("iks_enabled", AtomModel.IsIksEnable);
                props.Add("selected_location", AtomModel.BeforeConnectionLocation);
                // props.Add("iks_reconnecting", e?.ConnectionDetails?.IKSIsEnabled);
                try
                {
                    props.Add("connect_via", AtomModel.connectingFrom.ToString());
                    props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
                    props.Add("is_split_enabled", AtomModel.IsSplitEnable);

                    if (AtomModel.IsIksEnable && e.ConnectionDetails.IKSIsEnabled)
                        props.Add("connection_initiated_by", Entity.Enums.ConnectedTo.AutoReconnect.ToString());
                    else
                        props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());

                    if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
                        props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
                    else
                        props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }

                #region NAS Identifier
                try
                {
                    props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
                    props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

                    if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        props.Add("is_filtered_server_requested", true);
                    else
                        props.Add("is_filtered_server_requested", false);


                    if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                        props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


                    props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
                    props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }

                #endregion

                if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
                    props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

                props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
                props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
                props.Add("atom_session_id", e.ConnectionDetails.SessionID);


                #region Connection filters
                props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
                props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
                #endregion

                mixpanelService.FireEvent(MixpanelEvents.app_utc, AtomModel.HostingID, props);


                #region Iks blocked internet mixpanel event
                if (e != null && e.ConnectionDetails != null && e.ConnectionDetails.IKSIsEnabled)
                {
                    var prop = new MixpanelProperties().MixPanelPropertiesDictionary;
                    prop.Add("reason", "VPN Issue");
                    prop.Add("is_split_enabled", AtomModel.IsSplitEnable);
                    mixpanelService.FireEvent(MixpanelEvents.app_block_internet_iks, AtomModel.HostingID, prop);
                }
                #endregion
                #endregion

                AtomModel.IsConnected = false;
                AtomModel.IsConnecting = true;
                AtomModel.IsCancelled = false;
                AtomModel.StartConnecting = true;
                AtomModel.IsIksEnableAtAtom = e.ConnectionDetails.IKSIsEnabled;
                AtomModel.IsQuantumResistantServer = e.ConnectionDetails.IsDialedWithQuantumResistantServer;
                //  AtomModel.IsDisconnected = true;
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private async void AtomManagerInstance_StateChanged(object sender, StateChangedEventArgs e)
        {
            try
            {
                if (e != null && !string.IsNullOrEmpty(e.State.ToString()))
                {
                    string state = e.State.ToString().ToUpper();

                    if (state == "SETTING_VPN_ENTRY"
                        || state == "SETTING_VPN_CREDENTIALS"
                        || state == "OPENING_PORT"
                        || state == "AUTHENTICATING"
                        || state == "AUTHENTICATED")
                        AtomModel.ShowCancelButton = false;
                    else
                        AtomModel.ShowCancelButton = true;

                    if (state == "RECONNECTING")
                    {
                        AtomModel.IsInternetDown = !await Utilities.IsInternetAvailable();
                    }
                    else if (state == "BUILDING_CONFIGURATION")
                    {
                        AtomModel.IsInternetDown = false;
                    }

                }
                else
                {
                    AtomModel.ShowCancelButton = false;
                }

                AtomModel.IsConnected = false;
                AtomModel.IsConnecting = true;

                StatusModel status = new StatusModel();
                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnecting);
                status.CurrentStatus = Entity.Enums.CurrentStatus.Connecting;
                status.Country = string.Empty;
                status.Ip = string.Empty;
                status.Flag = string.Empty;
                status.atomstatus = e.State.ToString();
                StatusChanged(status);
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private void AtomManagerInstance_UnableToAccessInternet(object sender, UnableToAccessInternetEventArgs e)
        {
            try
            {
                TriggerUTBEventForTelemetry(e);

                if (e.UTBEventSource == Atom.SDK.Core.Enumerations.UTBEventSource.SessionStart)
                {
                    StatusModel status = new StatusModel();
                    status.IsUTBOccured = true;
                    StatusChanged(status);
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }

        }

        private void AtomManagerInstance_ConnectedLocation(object sender, ConnectedLocationEventArgs e)
        {
            StatusModel status = new StatusModel();
            status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusConnected);
            status.CurrentStatus = Entity.Enums.CurrentStatus.Connected;
            status.IsIpUpdate = true;

            try
            {
                status.Ip = e.ConnectedLocation.Ip;
                status.Country = e.ConnectedLocation.Country.Name;
                status.City = e.ConnectedLocation.City.Name;
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
            finally
            {
                SetAfterConnectionLocationValue(status);
                StatusChanged(status);
            }
        }

        public CountryModel GetFastestServer()
        {
            Country country = atomManager.GetRecommendedCountry();
            return mapper.Map<Atom.Core.Models.Country, CountryModel>(country);
        }

        public async Task<CountryModel> GetFastestServerByProtocol(string SelectedProtocol)
        {
            LocationFilter filter = new LocationFilter();

            var protocols = await GetAtomProtocols();

            if (protocols != null)
            {
                if (!string.IsNullOrEmpty(SelectedProtocol))
                    filter.Protocol = protocols.Where(x => x.Name.ToLower() == SelectedProtocol.ToLower())?.FirstOrDefault();
                else
                    filter.Protocol = protocols.Where(x => x.Name.ToLower() == "ikev2")?.FirstOrDefault();
            }

            Location location = atomManager.GetRecommendedLocationByFilters(filter);
            return mapper.Map<Atom.Core.Models.Country, CountryModel>(location?.Country);
        }

        public int GetPingForDedicatedIP()
        {
            if (AtomModel.dedicatedIP != null && AtomModel.dedicatedIP.body != null && AtomModel.dedicatedIP.body.dedicated_ip_detail != null && !string.IsNullOrEmpty(AtomModel.dedicatedIP.body.dedicated_ip_detail.ip))
            {
                DedicatedIPServerPing ping = new DedicatedIPServerPing();
                ping.ServerAddress = AtomModel.dedicatedIP.body.dedicated_ip_detail.host;
                var obj = atomManager.PingDedicatedIPServer(ping);
                return obj.Latency;
            }
            return 0;
        }
        private void SetAfterConnectionLocationValue(StatusModel status)
        {
            try
            {
                if (status != null && !string.IsNullOrEmpty(status.Country) && (AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.Country || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.SmartConnect || AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.DedicatedIP))
                    AtomModel.AfterConnectionLocation = status.Country;
                else if (status != null && !string.IsNullOrEmpty(status.City) && AtomModel.connectingFrom == Entity.Enums.ConnectingFrom.City)
                    AtomModel.AfterConnectionLocation = status.City;
                else
                    AtomModel.AfterConnectionLocation = string.Empty;
            }
            catch (Exception exp)
            {
                //sentryService.LoggingException(exp);
            }
        }


        private void AtomManagerInstance_PacketsTransmitted(object sender, PacketsTransmittedEventArgs e)
        {
            try
            {
                StatusModel model = new StatusModel();

                var bitrcv = e.BytesReceived * 8;
                var bitsent = e.BytesSent * 8;

                var rcv = GetAnotherSize(bitrcv - AtomModel.LastByteRecieved);
                var sent = GetAnotherSize(bitsent - AtomModel.LastByteSent);

                model.BytesRecieved = rcv.val;
                model.MeasureUnitRcv = rcv.unit;

                model.BytesSent = sent.val;
                model.MeasureUnitSent = sent.unit;


                AtomModel.LastByteRecieved = bitrcv;
                AtomModel.LastByteSent = bitsent;

                PacketTransmitOccured(model);

            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }

        }


        public class packetobj
        {
            public double val { get; set; }
            public string unit { get; set; }
        }

        public packetobj GetDataSize(double value)
        {
            packetobj pk = new packetobj();

            if (value <= 1024) { pk.val = value; pk.unit = "kbps"; }
            else if (value <= (1024 * 1024)) { pk.val = Math.Round(value / 1024, 2); pk.unit = "Mbps"; }
            else { pk.val = Math.Round(value / (1024 * 1024), 2); pk.unit = "Gbps"; }
            return pk;
        }

        public packetobj GetAnotherSize(double ContentLength)
        {
            packetobj obj = new packetobj();

            if (ContentLength >= 1073741824.00)
            {
                obj.val = Math.Round(ContentLength / 1073741824.00, 2);
                obj.unit = "Gbps";
            }
            else if (ContentLength >= 1048576.00)
            {
                obj.val = Math.Round(ContentLength / 1048576.00, 2);
                obj.unit = "Mbps";
            }
            else if (ContentLength >= 1024.00)
            {
                obj.val = Math.Round(ContentLength / 1024.00, 2);
                obj.unit = "Kbps";
            }
            else
            {
                obj.val = ContentLength;
                obj.unit = "bps";

            }

            return obj;
        }

        private void AtomManagerInstance_AtomInitialized(object sender, AtomInitializedEventArgs e)
        {
            InitializingStatusModel initializingStatusModel = new InitializingStatusModel
            {
                IsInitializingSuccess = true
            };
            InitializingStatusChanged(initializingStatusModel);
        }

        private void AtomManagerInstance_AtomDependenciesMissing(object sender, AtomDependenciesMissingEventArgs e)
        {
            InitializingStatusModel initializingStatusModel = new InitializingStatusModel
            {
                IsInitializingSuccess = false
            };
            InitializingStatusChanged(initializingStatusModel);
        }

        private void TriggerUTBEventForTelemetry(UnableToAccessInternetEventArgs e)
        {
            #region Mixpanel
            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("server_ip", e?.ConnectionDetails?.ServerIP);
            props.Add("server_dns", e?.ConnectionDetails?.ServerAddress);
            props.Add("selected_protocol_name", AtomModel.SelectedProtocol);
            props.Add("dialed_protocol_name", e?.ConnectionDetails?.Protocol?.ProtocolSlug.ToUpper());

            props.Add("iks_enabled", AtomModel.IsIksEnable);
            props.Add("selected_location", AtomModel.BeforeConnectionLocation);
            props.Add("dialed_location", AtomModel.AfterConnectionLocation);


            bool connected_with_desired_location = false;
            if (!string.IsNullOrEmpty(AtomModel.BeforeConnectionLocation) && !string.IsNullOrEmpty(AtomModel.AfterConnectionLocation) && AtomModel.BeforeConnectionLocation.ToLower() == AtomModel.AfterConnectionLocation.ToLower())
                connected_with_desired_location = true;

            props.Add("connected_via_desired_location", connected_with_desired_location);

            #region NAS Identifier
            try
            {
                props.Add("personalise_server_selection_enabled", AtomModel.IsSameExperienceCheckEnable);
                props.Add("is_filtered_server", e?.ConnectionDetails.IsFiltered);

                if (AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                    props.Add("is_filtered_server_requested", true);
                else
                    props.Add("is_filtered_server_requested", false);


                if (e.ConnectionDetails != null && !string.IsNullOrEmpty(e.ConnectionDetails.NASIdentifier) && AtomModel.IsSameExperienceCheckEnable && e.ConnectionDetails.ProvidedFilters != null && e.ConnectionDetails.ProvidedFilters.Count > 0)
                    props.Add("nas_identifier", e.ConnectionDetails.NASIdentifier);


                props.Add("included_nas_identifiers", AtomModel.IncludedNasIdentifiers);
                props.Add("excluded_nas_identifiers", AtomModel.ExcludedNasIdentifiers);

            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }

            #endregion


            try
            {
                props.Add("connect_via", AtomModel.connectingFrom.ToString());
                props.Add("selected_interface_screen", AtomModel.selectedInterfaceScreen.ToString());
                props.Add("connection_initiated_by", AtomModel.connectionInitiatedBy.ToString());
                props.Add("is_split_enabled", AtomModel.IsSplitEnable);

                if (AtomModel.ConnectedTo == Entity.Enums.ConnectedTo.NotConnected)
                    props.Add("selected_interface", Entity.Enums.ConnectedTo.List.ToString());
                else
                    props.Add("selected_interface", AtomModel.ConnectedTo.GetEnumDescription());
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }

            if (!string.IsNullOrEmpty(AtomModel.SpeedtestGroup))
                props.Add("speedtest_experiment_group", AtomModel.SpeedtestGroup);

            props.Add("is_experimented_server", e.ConnectionDetails.IsExperimentedServer);
            props.Add("is_experiment_server_requested", AtomModel.IsExperimentServerRequested);
            props.Add("atom_session_id", e.ConnectionDetails.SessionID);

            var connectionTime = atomManager.GetConnectedTime();
            var duration = (DateTime.Now - connectionTime).TotalSeconds;

            props.Add(MixpanelProperties.time_since_connection, duration);
            props.Add(MixpanelProperties.triggered_at, e?.UTBEventSource.GetUTBEventSource());

            #region Connection filters
            props.Add("is_obfuscated_server_requested", AtomModel.isObfSupported);
            props.Add("is_quantum_resistant_server_requested", AtomModel.isQrSupported);
            #endregion

            mixpanelService.FireEvent(MixpanelEvents.app_utb, AtomModel.HostingID, props);
            #endregion
        }

        public void ReconnectAfterUTB()
        {
            try
            {
                AtomModel.IsConnecting = false;
                StatusModel status = new StatusModel();
                status.StatusString = Common.Resources.GetString(Common.Resources.ConnectionStatusNotConnected);
                status.CurrentStatus = Entity.Enums.CurrentStatus.NotConnected;
                status.Country = string.Empty;
                status.Ip = string.Empty;
                status.Flag = string.Empty;
                StatusChanged(status);

                try
                {
                    RedialFromIKS();
                }
                catch (Exception ex)
                {
                    //sentryService.LoggingException(ex);
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }

        private void ResetFilterProperties()
        {
            AtomModel.isObfSupported = null;
            AtomModel.isP2pSupported = null;
            AtomModel.isQrSupported = null;
            AtomModel.isTorSupported = null;
        }

        private void SpeedMeasurementManager_SpeedMeasurmentData(SpeedMeasurement speedMeasurementData, SpeedMeasurementType speedMeasurementType)
        {
            SpeedTestProcessStatus(speedMeasurementType);
            SendSpeedMeasurementMixpanelSuccessEvent(speedMeasurementData, speedMeasurementType, string.Empty);

            if (SpeedMeasurmentData != null)
                SpeedMeasurmentData(speedMeasurementData, speedMeasurementType);
        }

        private void SpeedMeasurementManager_SpeedMeasurementError(string errorMessage, SpeedMeasurementType speedMeasurementType)
        {
            SpeedTestProcessStatus(speedMeasurementType);
            SendSpeedMeasurementMixpanelErrorEvent(errorMessage, speedMeasurementType);

            if (SpeedMeasurementError != null)
                SpeedMeasurementError(errorMessage, speedMeasurementType);
        }

        private void SendSpeedMeasurementMixpanelSuccessEvent(SpeedMeasurement speedMeasurementData, SpeedMeasurementType speedMeasurementType, string errorMessage)
        {
            var props = new MixpanelProperties().MixPanelPropertiesDictionary;

            if (speedMeasurementType == SpeedMeasurementType.PreConnection)
            {
                props.Add(MixpanelProperties.speed_check_time, _beforeConnection);
            }
            else
            {
                props.Add(MixpanelProperties.selected_interface_screen, AtomModel.selectedInterfaceScreen);
                props.Add(MixpanelProperties.selected_interface, AtomModel.SelectedInterface);
                props.Add(MixpanelProperties.connection_initiated_by, AtomModel.connectionInitiatedBy);
                props.Add(MixpanelProperties.selected_protocol_name, AtomModel.SelectedProtocol);
                props.Add(MixpanelProperties.dialed_protocol_name, AtomModel.DialedProtocol);
                props.Add(MixpanelProperties.server_ip, AtomModel.ServerIP);
                props.Add(MixpanelProperties.server_dns, AtomModel.ServerAddress);
                props.Add(MixpanelProperties.is_expiremented_server, AtomModel.IsExperimentedServer);
                props.Add(MixpanelProperties.is_expirement_server_requested, AtomModel.IsExperimentServerRequested);
                props.Add(MixpanelProperties.included_nas_identifiers, includedNasIdentifiers);
                props.Add(MixpanelProperties.excluded_nas_identifiers, excludedNasIdentifiers);
                props.Add(MixpanelProperties.connect_via, AtomModel.connectingFrom.ToString());

                if (speedMeasurementType == SpeedMeasurementType.DuringConnection)
                {
                    props.Add(MixpanelProperties.dialed_location, AtomModel.AfterConnectionLocation);
                    props.Add(MixpanelProperties.speed_check_time, _duringConnection);
                }
                else
                {
                    props.Add(MixpanelProperties.last_dialed_location, AtomModel.AfterConnectionLocation);
                    props.Add(MixpanelProperties.speed_check_time, _afterConnection);
                }
            }

            props.Add(MixpanelProperties.download_speed, speedMeasurementData.DownloadSpeedMbs);
            props.Add(MixpanelProperties.upload_speed, speedMeasurementData.UploadSpeedMbs);

            mixpanelService.FireEvent(MixpanelEvents.app_user_speed, AtomModel.HostingID, props);
        }

        private void SendSpeedMeasurementMixpanelErrorEvent(string errorMessage, SpeedMeasurementType speedMeasurementType)
        {
            var props = new MixpanelProperties().MixPanelPropertiesDictionary;

            if (speedMeasurementType == SpeedMeasurementType.PreConnection)
                props.Add(MixpanelProperties.speed_check_time, _beforeConnection);
            else if (speedMeasurementType == SpeedMeasurementType.DuringConnection)
                props.Add(MixpanelProperties.speed_check_time, _duringConnection);
            else
                props.Add(MixpanelProperties.speed_check_time, _afterConnection);

            props.Add(MixpanelProperties.error_message, errorMessage);
            mixpanelService.FireEvent(MixpanelEvents.app_user_speed_failed, AtomModel.HostingID, props);
        }

        private void SpeedTestProcessStatus(SpeedMeasurementType speedMeasurementType)
        {
            if (speedMeasurementType == SpeedMeasurementType.PreConnection ||
                speedMeasurementType == SpeedMeasurementType.PostConnection)
                SpeedMeasurementModel.IsConnectionSpeedTestProcessStarted = false;
            else
                SpeedMeasurementModel.IsDuringConnectionSpeedTestProcessStarted = false;
        }
    }
}

