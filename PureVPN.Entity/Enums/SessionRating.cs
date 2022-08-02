using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    public enum SessionRatingReason
    {
        [Description("None")]
        None = 0,
        [Description("Slow Speed")]
        SlowSpeed = 1,
        [Description("Unable to Browse")]
        UnableToBrowse = 2,
        [Description("Auto Disconnect")]
        AutoDisconnect = 3,
        [Description("Others")]
        Others = 4
    }

    public enum RatedType
    {
        [Description("None")]
        None = 0,
        [Description("good")]
        Good = 1,
        [Description("bad")]
        Bad = 2,
    }
}
