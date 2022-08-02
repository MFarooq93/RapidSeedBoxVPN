
using Atom.Core.Models;
using AutoMapper;
using PureVPN.Service.Contracts;

namespace PureVPN.Service
{
    public class PureAtomConfigurationService : IPureAtomConfigurationService
    {
        private IMapper mapper;
        public bool IsInitialized { get; set; }
        public AtomConfiguration AtomConfig { get; set; }
        public PureAtomConfigurationService InitializeAtomConfiguration(string secret, string VpnInterfaceName)
        {
            if (!IsInitialized)
            {
                AtomConfig = new AtomConfiguration(secret);
                AtomConfig.PersistVPNDetails = true;
                AtomConfig.VpnInterfaceName = VpnInterfaceName;
                IsInitialized = true;
            }
            return this;
        }
        public PureAtomConfigurationService(IMapper _mapper)
        {
            this.mapper = _mapper;
        }

        public PureAtomConfigurationService()
        {
        }
    }
}
