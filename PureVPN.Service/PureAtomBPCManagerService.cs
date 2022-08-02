
//using Atom.BPC;
using AutoMapper;
using PureVPN.Service.Contracts;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class PureAtomBPCManagerService : IPureAtomBPCManagerService
    {
        private IMapper mapper;
        public bool IsInitialized { get; set; }
        //public AtomBPCManager AtomBpcMgr { get; set; }

        public PureAtomBPCManagerService(IMapper _mapper)
        {
            this.mapper = _mapper;
        }
        public PureAtomBPCManagerService()
        {

        }

        public async Task<PureAtomBPCManagerService>  InitializeBPCAtomManager(PureAtomConfigurationService pureAtomconfiguration)
        {
            if (!IsInitialized)
            {
                //AtomBpcMgr = await AtomBPCManager.InitializeAsync(pureAtomconfiguration.AtomConfig).ConfigureAwait(false);
                IsInitialized = true;
            }
            return this;
        }
    }
}
