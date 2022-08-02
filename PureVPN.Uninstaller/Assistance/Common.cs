using System;
using System.IO;
using Tally;

namespace PureVPN.Uninstaller.Assistance
{
    public class Common
    {
        public static string AppDirectory { get; set; }

        public static string ProgramDataFolderPath
        {
            get
            {
                var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "purevpn");
                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex) { Logger.Log(ex); }
                return path;
            }
        }

        public static string ProgramFilesFolderPath
        {
            get
            {
                string pathProgramFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.Create), "GZ Systems\\PureVPN");

                if (!Directory.Exists(pathProgramFiles))
                    Directory.CreateDirectory(pathProgramFiles);

                return pathProgramFiles;
            }
        }

        public static string AppDataRootFolderPath
        {
            get
            {
                string AppDataFullPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string Drive = AppDataFullPath.Substring(0, AppDataFullPath.LastIndexOf("Users"));
                var path = Path.Combine(Drive, "Users");
                return path;
            }
        }

        public static string AppDataSubFolderPath
        {
            get
            {
                var path = System.IO.Path.Combine("AppData", "Local", "GZ_Systems");
                return path;
            }
        }

    }
}
