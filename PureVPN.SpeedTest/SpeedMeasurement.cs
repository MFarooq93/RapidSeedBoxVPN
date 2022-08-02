using Newtonsoft.Json;
using PureVPN.SpeedTest.Delegates;
using PureVPN.SpeedTest.Enums;
using PureVPN.SpeedTest.ErrorHandler;
using PureVPN.SpeedTest.Helpers;
using PureVPN.SpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest
{
    public class SpeedMeasurement
    {
        #region Events

        public event DataReceived DataReceived;
        public event ErrorDataReceived ErrorDataReceived;

        #endregion

        #region public properties

        public string DownloadSpeedMbs { get; internal set; }
        public string DownloadSpeedKbs { get; internal set; }
        public string DownloadSpeedBytes { get; internal set; }

        public string UploadSpeedMbs { get; internal set; }
        public string UploadSpeedKbs { get; internal set; }
        public string UploadSpeedBytes { get; internal set; }

        public double Latency { get; internal set; }
        public string ServerName { get; internal set; }
        public string Location { get; internal set; }
        public string ISP { get; internal set; }

        public Status CurrentStatus { get; internal set; } = Status.Idle;
        public SpeedTestResult SpeedTestResponse { get; internal set; }

        #endregion

        #region properties

        private static SpeedMeasurement _instance;
        private static Task _speedMeasurementTask;

        #endregion

        #region constructor

        private SpeedMeasurement() { }

        public static SpeedMeasurement GetInstance()
        {
            if (_instance is null)
            {
                _instance = new SpeedMeasurement();
            }

            return _instance;
        }

        #endregion

        #region public method

        public void RunSpeedMeasurement(string identifier, Server server = null)
        {
            if (CurrentStatus == Status.InProgress)
            {
                KillProcess();
            }

            _speedMeasurementTask = Task.Run(() =>
            {
                try
                {
                    CurrentStatus = Status.InProgress;
                    var args = GetProcessArguments(server);
                    var output = ProcessHelper.StartProcess(Constants.ExePathDirectory, Constants.SpeedTestExeName, args);

                    ProcessOutput(output, identifier);
                }
                catch (Exception exception)
                {
                    ErrorDataReceived?.Invoke(this,
                        new SpeedTestException(ErrorCode.SomethingWentWrong,
                        ErrorMessages.GetErrorMessage(ErrorCode.SomethingWentWrong), exception), identifier);
                }
            });
        }

        public void KillProcess()
        {
            try
            {
                ProcessHelper.KillProcess();
                _speedMeasurementTask.Dispose();
            }
            catch { }
        }

        public double ConvertBytesToMBs(double bytes)
        {
            return (bytes / 1024) / 1024;
        }


        #endregion

        #region Core methods

        private string GetProcessArguments(Server server)
        {
            if (server is null)
                return Constants.Arguments;
            else
                return $"-s {server.ID} {Constants.Arguments}";
        }

        private void ProcessOutput(List<string> output, string identifier)
        {
            List<SpeedTestResult> speedTestResults = new List<SpeedTestResult>();

            foreach (var item in output)
            {
                if (item.Contains("download") && item.Contains("upload") && item.Contains("bandwidth"))
                {
                    var speedTestResponse = DeserializeJson(item);
                    CurrentStatus = Status.Completed;
                    SetProperties(speedTestResponse);

                    DataReceived?.Invoke(this, speedTestResponse, identifier);
                    return;
                }
                else if (item.Contains("error"))
                {
                    SpeedTestResult speedTestResponse;

                    if (item.Contains("[error]"))
                    {
                        speedTestResponse = DeserializeJson(output[output.Count - 1]);

                        if (!string.IsNullOrEmpty(speedTestResponse?.Message))
                        {
                            CurrentStatus = Status.Idle;
                            ErrorDataReceived?.Invoke(this,
                                    new SpeedTestException(ErrorCode.SomethingWentWrong,
                                    ErrorMessages.GetErrorMessage(ErrorCode.SomethingWentWrong),
                                    new Exception(speedTestResponse?.Message)), identifier);
                            return;
                        }
                    }
                    else
                    {
                        speedTestResponse = DeserializeJson(item);

                        if (!string.IsNullOrEmpty(speedTestResponse?.Error))
                        {
                            CurrentStatus = Status.Idle;
                            ErrorDataReceived?.Invoke(this,
                                    new SpeedTestException(ErrorCode.SomethingWentWrong,
                                    ErrorMessages.GetErrorMessage(ErrorCode.SomethingWentWrong),
                                    new Exception(speedTestResponse.Error)), identifier);
                            return;
                        }
                    }
                }
            }

            CurrentStatus = Status.Completed;
        }

        private SpeedTestResult DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<SpeedTestResult>(json);
        }

        private void SetProperties(SpeedTestResult speedTestResult)
        {
            if (speedTestResult is null)
            {
                return;
            }

            SpeedTestResponse = speedTestResult;

            DownloadSpeedBytes = speedTestResult.Download.Bytes.ToString("00.00");
            DownloadSpeedKbs = (speedTestResult.Download.Bytes / 1024).ToString("00.00");
            DownloadSpeedMbs = ((speedTestResult.Download.Bytes / 1024) / 1024).ToString("00.00");

            UploadSpeedBytes = speedTestResult.Upload.Bytes.ToString("00.00");
            UploadSpeedKbs = (speedTestResult.Upload.Bytes / 1024).ToString("00.00");
            UploadSpeedMbs = ((speedTestResult.Upload.Bytes / 1024) / 1024).ToString("00.00");

            ServerName = speedTestResult.Server.Name;
            Location = speedTestResult.Server.Location;
            Latency = speedTestResult.Ping.Latency;
            ISP = speedTestResult.ISP;
        }

        #endregion
    }
}
