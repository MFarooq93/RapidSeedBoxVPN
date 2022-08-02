using MvvmCore;
using System;
using System.Windows.Input;
using PureVPN.Installer.Helpers;
using PureVPN.Installer.Assistance;
using System.Net.NetworkInformation;
using System.Linq;
using Tally;
using System.Management;

namespace PureVPN.Installer.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        private void Install()
        {
            NetworkInterface connectedAdapter;
            if (CheckVPNStatus(out connectedAdapter)) // Check VPN Status
            {
                // VPN is connected, connectedAdapter is only set when Tap-Adapter is connected
                bool isAttemptedToDisconnect;
                ShowDisconnectMessageBox(connectedAdapter, out isAttemptedToDisconnect);
                if (isAttemptedToDisconnect)
                {
                    if (CheckVPNStatus(out _) == false)
                        GoNext();
                }
            }
            else
            {
                GoNext();
            }
        }

        public void ShowDisconnectMessageBox(NetworkInterface connectedAdapter, out bool isAttemptedToDisconnect)
        {
            isAttemptedToDisconnect = false;
            ShowOverlay = true;
            var retry = IoC.Get<MessageBoxViewModel>().Show("Disconnect PureVPN",
            "Please disconnect PureVPN first and then retry.",
            true, "OK", connectedAdapter == null ? "" : "Disconnect");

            if (retry != null && retry == false)
            {
                isAttemptedToDisconnect = true;
                DisconnectAdapter(connectedAdapter);
            }

            ShowOverlay = false;
        }

        private bool CheckVPNStatus(out NetworkInterface connectedAdapter)
        {

            if (CheckOpenVPNConnection(out connectedAdapter))
                return true;

            if (CheckRasDialConnection(out _)) // ConnectedAdapter not set for RAS connection to stop showing the "Disconnect" button on dilaog
                return true;

            return false;
        }

        private bool CheckOpenVPNConnection(out NetworkInterface connectedAdapter)
        {
            connectedAdapter = null;
            bool IsConnected = false;
            try
            {
                var list = NetworkInterface.GetAllNetworkInterfaces();
                if (list != null && list.Count() > 0)
                {
                    var adapter = list.FirstOrDefault(p => p.Description != null && p.Description.Equals("tap-windows adapter v9", StringComparison.OrdinalIgnoreCase));

                    if (adapter != null && adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        IsConnected = true;
                        connectedAdapter = adapter;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return IsConnected;
        }

        private bool CheckRasDialConnection(out NetworkInterface connectedAdapter)
        {
            connectedAdapter = null;
            bool IsConnected = false;
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (NetworkInterface Interface in interfaces)
                    {
                        if (Interface.OperationalStatus == OperationalStatus.Up)
                        {

                            if (Interface.NetworkInterfaceType == NetworkInterfaceType.Ppp &&
                                Interface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                Interface.OperationalStatus == OperationalStatus.Up &&
                                Interface.Name == "PureVPN")
                            {
                                IsConnected = true;
                                connectedAdapter = Interface;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return IsConnected;
        }

        /// <summary>
        /// Attempt to disconnect the <paramref name="connectedAdapter"/>
        /// </summary>
        /// <param name="connectedAdapter"></param>
        private void DisconnectAdapter(NetworkInterface connectedAdapter)
        {
            try
            {
                SelectQuery wmiQuery = new SelectQuery($"SELECT * FROM Win32_NetworkAdapter WHERE GUID = '{connectedAdapter.Id}'");
                ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
                foreach (ManagementObject item in searchProcedure.Get())
                {
                    if (item != null)
                    {
                        try
                        {
                            item.InvokeMethod("Disable", null);
                            break;
                        }
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

        private void GoNext()
        {
            if (ActiveDialog == IoC.Get<AgreementViewModel>())
                ActiveDialog = Common.IsAlreadyInstalled ? (BaseViewModel)IoC.Get<InstallerViewModel>() : (BaseViewModel)IoC.Get<AdditionalTasksViewModel>();
            else if (ActiveDialog == IoC.Get<AdditionalTasksViewModel>())
                ActiveDialog = IoC.Get<InstallerViewModel>();
        }

        private void GoBack()
        {
            if (ActiveDialog == IoC.Get<InstallerViewModel>())
                ActiveDialog = IoC.Get<AdditionalTasksViewModel>();
            else if (ActiveDialog == IoC.Get<AdditionalTasksViewModel>())
                ActiveDialog = IoC.Get<AgreementViewModel>();
        }

        public void CancelClick()
        {
            ShowOverlay = true;
            var exitApp = IoC.Get<MessageBoxViewModel>().Show("Exiting PureVPN Setup",
            "Are you sure you want to exit PureVPN Setup and abort the installation?",
            true, "YES", "NO");
            ShowOverlay = false;

            if (exitApp == true)
                CloseApp();
        }

        public BaseViewModel ActiveDialog
        {
            get { return IoC.Get<MainViewModel>().ActiveViewModel; }
            set { IoC.Get<MainViewModel>().ActiveViewModel = value; }
        }

        public bool ShowOverlay
        {
            get { return IoC.Get<MainViewModel>().ShowOverlay; }
            set { IoC.Get<MainViewModel>().ShowOverlay = value; }
        }

        public void CloseApp()
        {
            Environment.Exit(0);
        }

        public void MinimizeApp()
        {
            App.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        public void DragApp()
        {
            App.MainWindow.DragMove();
        }

        #region ICommands
        private ICommand cancelAction;
        public ICommand CancelAction
        {
            get
            {
                if (cancelAction == null)
                    cancelAction = new ActionHelper<object>(CancelClick);
                return cancelAction;
            }
        }

        private ICommand goNextAction;
        public ICommand GoNextAction
        {
            get
            {
                if (goNextAction == null)
                    goNextAction = new ActionHelper<bool>(GoNext);
                return goNextAction;
            }
        }

        private ICommand goBackAction;
        public ICommand GoBackAction
        {
            get
            {
                if (goBackAction == null)
                    goBackAction = new ActionHelper<bool>(GoBack);
                return goBackAction;
            }
        }

        private ICommand installAction;
        public ICommand InstallAction
        {
            get
            {
                if (installAction == null)
                    installAction = new ActionHelper<bool>(Install);
                return installAction;
            }
        }
        #endregion
    }
}
