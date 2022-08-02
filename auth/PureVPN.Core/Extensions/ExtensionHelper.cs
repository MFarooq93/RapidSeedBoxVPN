using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Extensions
{
    internal static class ExtensionHelper
    {
        internal static bool IsStingNullorEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                return true;

            return false;
        }

        internal static bool IsStingNotNullorEmpty(this string str)
        {
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                return true;

            return false;
        }
    }
}
