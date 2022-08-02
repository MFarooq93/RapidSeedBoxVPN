using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Tally
{
    public class Logger
    {
        private static string SettingsDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "purevpn");
            }
        }

        private static string logPath;
        public static string LogPath
        {
            get
            {
                if (String.IsNullOrEmpty(logPath))
                    logPath = System.IO.Path.Combine(SettingsDirectory, "debuglog.txt");
                try
                {
                    if (!System.IO.File.Exists(logPath))
                        System.IO.File.Create(logPath);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
                return logPath;
            }
        }

        private const string Identifier = "!@#";

        private static void WriteIdentifier(string[] logText)
        {
            try
            {
                var lastLogTime = DateTime.Now;

                if (File.Exists(LogPath) && logText != null && logText.Count() > 0)
                    DateTime.Parse(logText.Last().Split('\t').First());


                var currentTime = DateTime.Now;

                var dayChanged = (int)(currentTime - lastLogTime).TotalDays > 0;
                if (dayChanged || (logText != null && logText.Where(x => x == Identifier).Count() == 0))
                {
                    using (StreamWriter file = new StreamWriter(LogPath, true))
                    {
                        file.WriteLine(Identifier);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private static void ReduceNewLogs(string[] logText)
        {
            try
            {
                if (logText != null && logText.Length > 0)
                {
                    string logs = string.Join(Environment.NewLine, logText);
                    if (!String.IsNullOrEmpty(logs))
                    {
                        var dayWiseLogs = logs.Split(Identifier.ToArray(), StringSplitOptions.RemoveEmptyEntries);

                        if (dayWiseLogs != null && dayWiseLogs.Length > 7)
                        {
                            dayWiseLogs = dayWiseLogs.Reverse().ToArray();
                            dayWiseLogs = dayWiseLogs.Take(7).ToArray();
                            dayWiseLogs = dayWiseLogs.Reverse().Where(x => !String.IsNullOrEmpty(x)).ToArray();

                            string log = "";
                            foreach (var item in dayWiseLogs)
                                log += item + Environment.NewLine + Identifier + Environment.NewLine;

                            File.WriteAllText(LogPath, log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private static void ReduceOldLogs(string[] logText)
        {
            try
            {
                if (logText != null && logText.Length >= 500 && logText.Where(x => x.Contains(Identifier)).Count() == 0)
                {
                    logText = logText.Reverse().Take(500).Reverse().Where(x => !String.IsNullOrEmpty(x)).ToArray();
                    File.WriteAllLines(LogPath, logText);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public static void Log(string LogMessage, LogTag TAG = LogTag.INFO)
        {
            try
            {
                try
                {
                    WriteIdentifier(File.ReadAllLines(LogPath));

                    string messageToLog = string.Format("{0}\t|\t{1}\t|\t{2}\t{3}", DateTime.Now, "purevpn", TAG, LogMessage);

                    using (StreamWriter file = new StreamWriter(LogPath, true))
                    {
                        file.WriteLine(messageToLog);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public static void Log(Exception exception, LogTag TAG = LogTag.ERROR)
        {
            ThreadPool.QueueUserWorkItem((r) =>
            {
                try
                {
                    WriteIdentifier(File.ReadAllLines(LogPath));

                    string messageToLog = string.Format("{0}\t|\t{1}\t{2}\t{3}", DateTime.Now, TAG, exception.Message, exception.StackTrace);
                    try
                    {
                        using (StreamWriter file = new StreamWriter(LogPath, true))
                        {
                            file.WriteLine(messageToLog);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }

            });
        }

        public static void TestLog(string message)
        {
            ThreadPool.QueueUserWorkItem((r) =>
            {
                try
                {
                    try
                    {
                        using (StreamWriter file = new StreamWriter("testlog.txt", true))
                        {
                            file.WriteLine(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            });
        }

        private Logger() { }

    }
}
