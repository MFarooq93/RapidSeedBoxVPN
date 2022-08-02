using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Tally;

namespace PureVPN.Uninstaller.Helpers
{
    public static class IOHelper
    {
        public static string OpenFolderBrowser()
        {
            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        return fbd.SelectedPath;
                    }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
            return "";
        }

        internal static void OverwriteFile(string filePath, string text)
        {
            byte[] encodedText = Encoding.Default.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                sourceStream.Write(encodedText, 0, encodedText.Length);
            };
        }

        public static bool IsDirectoryPresent(string path)
        {
            return Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static string CombinePaths(params string[] paths)
        {
            return Path.Combine(paths);
        }
    }
}
