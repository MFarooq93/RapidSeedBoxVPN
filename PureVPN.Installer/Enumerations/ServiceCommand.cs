using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enumerations
{
    public enum ServiceCommand
    {  
        StartFiringEvents = 128,
        StopFiringEvents = 129,
        SetDNS = 130,
        RevertDNS = 131,
        GetDNS = 132,
        PrepareAssets = 133,
        FireNetworkChange = 134,
        SetAppPath = 135,
        SendPureVPNMetrics = 136,
        EnableIPv6LeakProtection = 137,
        DisableIPv6LeakProtection = 138,
        SetSecureDNS = 139,
        ResetSecureDNS = 140,
        SetPermissionsAtInstallation = 152,
        InstallSplitDriver = 153,
        InstallChromeExtensionOnly = 154,
        InstallFirefoxExtensionOnly = 155,
        InstallChromFirefoxExtensionBoth = 156
    }
}
