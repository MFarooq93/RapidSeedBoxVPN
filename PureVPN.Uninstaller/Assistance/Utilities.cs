using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Tally;

namespace PureVPN.Uninstaller.Assistance
{
    public class Utilities
    {
        public static void CreateScriptToDeleteAppDir(string appPath)
        {
            try
            {
                var scriptPath = Path.Combine(Path.GetTempPath(), "delete_uninstaller.bat");
                try { File.Delete(scriptPath); }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                List<string> commands = new List<string>();
                commands.Add("timeout /t 5");
                commands.Add("rd /s /q " + "\"" + appPath + "\"");
                File.WriteAllLines(scriptPath, commands);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void DeleteShortcuts(string shortcutName, string directory)
        {
            try
            {
                foreach (var item in Directory.GetFiles(directory))
                {
                    if (item.ToLower().Replace(".lnk", "").EndsWith(shortcutName.ToLower()))
                    {
                        try { File.Delete(item); }
                        catch (Exception ex) { Logger.Log(ex); }
                    }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public static void ExecuteCommand(string fileName, string workingDirectory = "", string arguments = "", bool waitForExit = true)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = fileName;
                if (!String.IsNullOrEmpty(arguments))
                    process.StartInfo.Arguments = arguments;
                process.StartInfo.Verb = "runas";
                if (!String.IsNullOrEmpty(workingDirectory))
                    process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.UseShellExecute = true;

                process.Start();
                if (waitForExit)
                    process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Logger.Log("Exit Code : " + process.ExitCode.ToString());
                }

                if (waitForExit)
                    process.Close();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static bool EraseDirectory(string folderPath, bool recursive)
        {
            //Safety check for directory existence.
            if (!Directory.Exists(folderPath))
                return false;

            foreach (string file in Directory.GetFiles(folderPath))
            {
                File.Delete(file);
            }

            //Iterate to sub directory only if required.
            if (recursive)
            {
                foreach (string dir in Directory.GetDirectories(folderPath))
                {
                    EraseDirectory(dir, recursive);
                }
            }
            //Delete the parent directory before leaving
            Thread.Sleep(1000);
            Directory.Delete(folderPath);
            return true;
        }

      

    }
}
