using PureVPN.Installer.Interfaces;
using PureVPN.Installer.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System;
using Tally;
using System.Windows.Threading;

namespace PureVPN.Installer.Managers
{
    public class WindowManager : IWindowManager
    {
        private Window CurrentWindow { get; set; }

        private bool IsWindowModal
        {
            get
            {
                try { return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(CurrentWindow); }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                return false;
            }
        }

        public bool? ShowDialog(BaseViewModel rootViewModel, IDictionary<string, object> settings = null)
        {
            CurrentWindow = new Window() { Content = rootViewModel.View };
            ApplySettings(settings);
            return CurrentWindow.ShowDialog();
        }

        public void ShowWindow(BaseViewModel rootViewModel, IDictionary<string, object> settings = null)
        {
            CurrentWindow = new Window() { Content = rootViewModel.View };
            ApplySettings(settings);
            CurrentWindow.Show();
        }

        public void Close(bool result = false)
        {
            if (IsWindowModal)
                CurrentWindow.DialogResult = result;
            CurrentWindow.Close();
        }

        private void ApplySettings(IDictionary<string, object> settings)
        {
            try
            {
                if (settings != null)
                {
                    foreach (var item in settings)
                    {
                        try { CurrentWindow.GetType().GetProperty(item.Key).SetValue(CurrentWindow, item.Value, null); }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
