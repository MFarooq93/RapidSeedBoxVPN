using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class SpeedMeasurementModel
    {
        public static NetworkSpeed DuringConnectionSpeedInMBs { get; set; }
        public static bool IsSpeedTestIsInProcess { get; set; }
        public static bool IsConnectionSpeedTestProcessStarted { get; set; }
        public static bool IsConnectionSpeedTestProcessStopped => !IsConnectionSpeedTestProcessStarted;
        public static bool IsDuringConnectionSpeedTestProcessStarted { get; set; }
        public static bool IsDuringConnectionSpeedTestProcessStopped => !IsDuringConnectionSpeedTestProcessStarted;
        public static double PreConnectionBaseSpeedOfUser { get; set; }
        public static DateTime PreConnectionSpeedMeasurementTime { get; set; }
        public static bool IsCacheTimeExpired {
            get 
            {
                if (PreConnectionSpeedMeasurementTime == default)
                    return true;

                int days = Convert.ToInt32((DateTime.UtcNow - PreConnectionSpeedMeasurementTime).TotalDays);

                if (days > 0)
                    return true;

                return false;
            } 
        }
    }
}
