
namespace PureVPN.Service.Contracts
{
    public interface IPureAtomConfigurationService
    {
        PureAtomConfigurationService InitializeAtomConfiguration(string secret, string vpnInterfaceName);
    }
}
