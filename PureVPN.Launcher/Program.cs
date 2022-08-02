using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace PureVPN.Launcher
{
    /*
     * Luanches the app 
     */
    class Program
    {
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int Handle, int showState);

        [DllImport("kernel32.dll")]
        public static extern int GetConsoleWindow();

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Launcher opened");
                Console.WriteLine($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");
                try
                {
                    int win = GetConsoleWindow();
                    ShowWindow(win, 0);
                }
                catch { }

                bool requireWait = false;
                var process = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileName("PureVPN"));
                if (process != null)
                {
                    foreach (var item in process)
                    {
                        try
                        {
                            item.Kill();
                        }
                        catch
                        {
                            requireWait = true;
                        }
                    }
                }

                if (requireWait)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                }

                string purevpnArgs = GetArguments(args);

                var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PureVPN.exe");

                Console.WriteLine($"Launching {filepath} {purevpnArgs}");
                System.Diagnostics.Process.Start(filepath, purevpnArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        /// <summary>
        /// Generate arguments to launch PureVPN app
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string GetArguments(string[] args)
        {
            string purevpnArgs = null;
            if (args?.FirstOrDefault() == "none")
            {
                purevpnArgs = null;
            }
            else if (args?.Length > default(int))
            {
                purevpnArgs = String.Join(" ", args);
            }
            else
            {
                purevpnArgs = "upgraded";
            }

            return purevpnArgs;
        }
    }
}