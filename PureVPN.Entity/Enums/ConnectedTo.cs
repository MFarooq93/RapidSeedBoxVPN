
using System.ComponentModel;

namespace PureVPN.Entity.Enums
{
    public enum ConnectedTo
    {
        [Description("NotConnected")]
        NotConnected = 0,
        [Description("SmartConnect")]
        QuickConnect = 1,
        [Description("Location")]
        Location = 2,
        [Description("Recent")]
        Recent,
        [Description("Favorite")]
        Favorite,
        [Description("Taskbar")]
        Taskbar,
        [Description("Search")]
        Search,
        [Description("AutoConnectOnLaunch")]
        AutoConnectOnLaunch,
        [Description("AutoReconnect")]
        AutoReconnect,
        [Description("Recommended")]
        Recommended,
        [Description("List")]
        List,
        [Description("DedicatedIP")]
        DedicatedIP
        

    }
}
