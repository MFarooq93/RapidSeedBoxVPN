using PureVPN.Entity.Delegates;
using PureVPN.Entity.Models;
using PureVPN.SpeedTest;
using PureVPN.SpeedTest.Enums;
using PureVPN.SpeedTest.ErrorHandler;
using PureVPN.SpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PureVPN.Entity.Delegates.SpeedMeasurementEventHandler;

namespace PureVPN.Service.ThirdPartyUtilities
{


    internal class SpeedMeasurementManager
    {
        private SpeedMeasurement _speedMeasurement;
        private static string _Identifier;
        private static SpeedMeasurementType _speedMeasurementType;
        internal SpeedMeasurementType SpeedMeasurementStatus;

        public event SpeedMeasurmentData SpeedMeasurmentData;
        public event SpeedMeasurementError SpeedMeasurementError;

        internal SpeedMeasurementManager()
        {
            _speedMeasurement = SpeedMeasurement.GetInstance();
            _speedMeasurement.DataReceived += SpeedMeasurement_DataReceived;
            _speedMeasurement.ErrorDataReceived += SpeedMeasurement_ErrorDataReceived;
        }

        internal void StartSpeedMeasurement(SpeedMeasurementType speedMeasurementType, bool shouldAddDelay = false, Server speedTestServer = null)
        {
            Task.Run(async () =>
            {
                if (shouldAddDelay)
                    await Task.Delay(5000);

                _speedMeasurementType = speedMeasurementType;
                _Identifier = Guid.NewGuid().ToString();
                _speedMeasurement.RunSpeedMeasurement(_Identifier, speedTestServer);
            });
        }

        internal void StopSpeedMeasurment()
        {
            _speedMeasurement.KillProcess();
        }

        private void SpeedMeasurement_DataReceived(object sender, SpeedTestResult speedTestResult, string identifier)
        {
            if (_Identifier == identifier)
            {
                _Identifier = null;

                var networkspeed = new NetworkSpeed()
                {
                    DownloadSpeed = _speedMeasurement.ConvertBytesToMBs(speedTestResult.Download.Bytes),
                    UploadSpeed = _speedMeasurement.ConvertBytesToMBs(speedTestResult.Upload.Bytes),
                    SpeedTestServer = speedTestResult.Server
                };

                switch (_speedMeasurementType)
                {
                    case SpeedMeasurementType.PreConnection:
                        this.SpeedMeasurementStatus = SpeedMeasurementType.PreConnection;
                        SpeedMeasurementModel.PreConnectionBaseSpeedOfUser = networkspeed.DownloadSpeed;
                        SpeedMeasurementModel.DuringConnectionSpeedInMBs = null;
                        SpeedMeasurementModel.PreConnectionSpeedMeasurementTime = DateTime.UtcNow;
                        break;

                    case SpeedMeasurementType.DuringConnection:
                        this.SpeedMeasurementStatus = SpeedMeasurementType.DuringConnection;
                        SpeedMeasurementModel.DuringConnectionSpeedInMBs = networkspeed;
                        break;

                    case SpeedMeasurementType.PostConnection:
                        this.SpeedMeasurementStatus = SpeedMeasurementType.PostConnection;
                        break;
                };

                SpeedMeasurmentData?.Invoke(_speedMeasurement, _speedMeasurementType);
            }
        }

        private void SpeedMeasurement_ErrorDataReceived(object sender, SpeedTestException exception, string identifier)
        {
            if (_Identifier == identifier)
            {
                _Identifier = null;

                if(_speedMeasurementType == SpeedMeasurementType.DuringConnection)
                    SpeedMeasurementModel.DuringConnectionSpeedInMBs = null;

                var errorMessage = exception.Message;

                if (exception?.InnerException != null)
                    errorMessage += ", " + exception.InnerException.Message;

                SpeedMeasurementError?.Invoke(errorMessage, _speedMeasurementType);
            }
        }
    }
}
