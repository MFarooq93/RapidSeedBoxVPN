using PureVPN.SpeedTest;
using PureVPN.SpeedTest.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Delegates
{
    public class SpeedMeasurementEventHandler
    {
        public delegate void SpeedMeasurmentData(SpeedMeasurement speedMeasurementData, SpeedMeasurementType speedMeasurementType);
        public delegate void SpeedMeasurementError(string errorMessage, SpeedMeasurementType speedMeasurementType);
    }
}
