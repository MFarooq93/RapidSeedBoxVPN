using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PureVPN.SetupSigner
{
    class Program
    {
        static void Main(string[] args)
        {
            var setup_name = "purevpn_setup.exe";
            var exe = @"..\..\..\PureVPN.WixBootstrapper\bin\Release\" + setup_name;
           // var exe = @"D:\PureVPN 2.0\src\PureVPN.WixBootstrapper\bin\Release\"+setup_name;
            var engine = @"..\..\..\PureVPN.WixBootstrapper\bin\Release\engine.exe";
            var crt = @"DigiCertCA.crt";
            var pfx = @"Gz-System-Ltd-2020.pfx";

            string insignia = Path.Combine(Environment.GetFolderPath
            (Environment.Is64BitOperatingSystem
            ? Environment.SpecialFolder.ProgramFilesX86
            : Environment.SpecialFolder.ProgramFiles),
            "WiX Toolset v3.11", 
            "bin", 
            "insignia.exe");

            string winkits = Path.Combine(Environment.GetFolderPath
            (Environment.Is64BitOperatingSystem
            ? Environment.SpecialFolder.ProgramFilesX86
            : Environment.SpecialFolder.ProgramFiles),
            "Windows Kits",
            "10");

            //var signtool = Path.Combine(
            //winkits,
            //"bin",
            //"10.0.17134.0", 10.0.17763.0
            //Environment.Is64BitOperatingSystem ? "x64" : "x86",
            //"signtool.exe");

            var signtool = Path.Combine(
            winkits,
            "bin",
            "10.0.17763.0",
            Environment.Is64BitOperatingSystem ? "x64" : "x86",
            "signtool.exe");

            Process.Start(insignia, 
            string.Format("-ib {0} -o {1}", exe, engine)).WaitForExit();

            //Process.Start(signtool,
            //string.Format("sign /ac {0} /t http://timestamp.verisign.com/scripts/timstamp.dll /f {1} /p 123456789 {2}", crt, pfx, engine)
            //).WaitForExit();


            Process.Start(signtool,
            string.Format("sign /ac {0} /t http://timestamp.digicert.com /f {1} /p 123456789 {2}", crt, pfx, engine)
            ).WaitForExit();

            Process.Start(insignia,
            string.Format("-ab {0} {1} -o {1}", engine, exe)).WaitForExit();

            //Process.Start(signtool,
            //string.Format("sign /ac {0} /t http://timestamp.verisign.com/scripts/timstamp.dll /f {1} /p 123456789 {2}", crt, pfx, exe)
            //).WaitForExit();

            Process.Start(signtool,
            string.Format("sign /ac {0} /t http://timestamp.digicert.com /f {1} /p 123456789 {2}", crt, pfx, exe)
            ).WaitForExit();

        }
    }
}
