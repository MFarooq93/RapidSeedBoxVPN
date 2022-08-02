using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    public enum SelectedInterfaceScreen
    {
        Taskbar,
        Dashboard,
        Location,
        /// <summary>
        /// Profile/Account details screen
        /// </summary>
        profile,
        /// <summary>
        /// Consent pop up shown to user before migrating user credentials to email
        /// </summary>
        migration_consent_popup,
        None
    }
}