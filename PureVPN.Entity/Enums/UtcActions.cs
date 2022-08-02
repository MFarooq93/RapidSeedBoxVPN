using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    public enum Action
    {
        troubleshoot,
        support,
        expired,
        show_protocols_screen,
        show_locations_screen,
        logout,
        reinstall
    }

    public enum NonTroubleshootAction
    {
        purchase
    }
}
