using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    public interface IPureAtomBPCManagerService
    {
        Task<PureAtomBPCManagerService> InitializeBPCAtomManager(PureAtomConfigurationService pureAtomconfiguration);
    }
}
