using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    /// <summary>
    /// Represents tabs on settings screen
    /// </summary>
    public enum SettingsTab
    {
        /// <summary>
        /// Tab for general settings
        /// </summary>
        General = 0,
        /// <summary>
        /// Tab for protcol settings
        /// </summary>
        Protocol,
        /// <summary>
        /// Tab for split tunneling settings
        /// </summary>
        SplitTunneling,
        /// <summary>
        /// Tab for Language settings
        /// </summary>
        Language,
             /// <summary>
             /// Tab for about settings
             /// </summary>
        About

    }
}