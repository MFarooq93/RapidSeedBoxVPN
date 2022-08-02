using System;

namespace PureVPN.Installer.Assistance
{
    public class Constants
    {
        public const string BaseResourcePath = "pack://application:,,,/PureVPNInstaller;component/Resources/";
        public const string ProgramFileAssetsZip = BaseResourcePath + "ProgramFilesAssets.zip";
        public const string AtomSDKInstaller = BaseResourcePath + "AtomSDKInstaller.zip";
        public const string WebView2Installer = BaseResourcePath + "MicrosoftEdgeWebview2Setup.zip";
        public const string LocalDataZip = BaseResourcePath + "localdata.zip";
        public const string UninstallSetupZip = BaseResourcePath + "Uninstaller.zip";
        public const string EnquiryZip = BaseResourcePath + "enquiry@purevpn.com.zip";
        public const string TermsZip = BaseResourcePath + "Documents/Terms.zip";

        public const string VCRedistExe86 = "vcredist_x86.exe";
        public const string VCRedistExe64 = "vcredist_x64.exe";
        public const string VCRedistUrl86 = "https://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe";
        public const string VCRedistUrl64 = "https://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x64.exe";

        public static string OSDir { get { return "WIN" + OSLiteral; } }

        private static string OSLiteral
        {
            get
            {
                string OS = "7";
                string OSName = Common.OSName;
                if (!String.IsNullOrEmpty(OSName))
                {
                    OS = (OSName.ToLower().Contains("windows 8") || OSName.ToLower().Contains("windows 10")) ? "8" :
                        (OSName.ToLower().Contains("2008") || OSName.ToLower().Contains("2k8") || OSName.ToLower().Contains("vista")) ? "2K8" :
                        (OSName.ToLower().Contains("xp")) ? "XP" : "7";
                }
                return OS;
            }
        }


    }
}
