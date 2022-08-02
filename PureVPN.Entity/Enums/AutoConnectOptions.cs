
using System.ComponentModel;

namespace PureVPN.Entity.Enums
{
    public enum AutoConnectOption
    {
        [Description("Recommended Location")]
        recommendedlocation,

        [Description("Recently Connected Location")]
        recentconnectedlocation,

        [Description("Specific Location")]
        specificlocation

      
    }
}
