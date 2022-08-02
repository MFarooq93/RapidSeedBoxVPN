using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PureVPN.SpeedTest.Helpers
{
    internal class ProcessHelper
    {
        private static List<string> _processOutput;
        internal static Process _process;
        internal static List<string> StartProcess(string directory, string exeName, string arguments)
        {
            if (_process is null)
                _process = new Process();

            _processOutput = new List<string>();

            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.FileName = Path.Combine(directory, exeName);
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.Arguments = arguments;
            _process.StartInfo.CreateNoWindow = true;
            //_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.ErrorDataReceived += Process_ErrorDataReceived;

            _process?.Start();

            _process?.BeginOutputReadLine();
            _process?.BeginErrorReadLine();
            _process?.StandardInput.Flush();
            //_process?.StandardInput.Close();

            _process?.WaitForExit();
            _process = null;

            return _processOutput;
        }

        internal static void KillProcess()
        {
            try
            {
                if (!(_process is null))
                    _process.Kill();
            }
            catch { }
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!(e.Data is null))
            {
                _processOutput.Add(e.Data);
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!(e.Data is null))
            {
                _processOutput.Add(e.Data);
            }
        }
    }
}
