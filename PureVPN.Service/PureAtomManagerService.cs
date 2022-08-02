using Atom.SDK.Net;
using AutoMapper;
using PureVPN.Service.Contracts;

namespace PureVPN.Service
{
    public class PureAtomManagerService : IPureAtomManagerService
    {
        private IMapper mapper;
        public bool IsInitialized { get; set; }
        public AtomManager AtomMgr { get; set; }
        public PureAtomManagerService InitializeAtomManager(PureAtomConfigurationService pureAtomconfiguration)
        {
            if(!IsInitialized)
            {
                AtomMgr = AtomManager.Initialize(pureAtomconfiguration.AtomConfig);
                IsInitialized = true;
            }
            return this;
        }
        public PureAtomManagerService(IMapper _mapper)
        {
            this.mapper = _mapper;
        }

        public PureAtomManagerService()
        {

        }
    }
}
