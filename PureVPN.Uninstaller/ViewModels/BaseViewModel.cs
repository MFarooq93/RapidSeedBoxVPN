using MvvmCore;
using PureVPN.Uninstaller.Helpers;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Input;
using Tally;

namespace PureVPN.Uninstaller.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        private void Continue()
        {
            if (CheckVPNStatus())
                ShowDisconnectMessageBox();
            else
                GoNext();
        }

        public void ShowDisconnectMessageBox()
        {
            ShowOverlay = true;
            var retry = IoC.Get<MessageBoxViewModel>().Show("Disconnect PureVPN",
            "Please disconnect PureVPN first and then retry.",
            true, "OK");
            ShowOverlay = false;
        }

        private bool CheckVPNStatus()
        {
            if (CheckOpenVPNConnection())
                return true;

            if (CheckRasDialConnection())
                return true;

            return false;
        }

        private bool CheckOpenVPNConnection()
        {
            bool IsConnected = false;
            try
            {
                var list = NetworkInterface.GetAllNetworkInterfaces();
                if (list != null && list.Count() > 0)
                {
                    var adapter = list.FirstOrDefault(p => p.Description != null && p.Description.Equals("tap-windows adapter v9", StringComparison.OrdinalIgnoreCase));

                    if (adapter != null && adapter.OperationalStatus == OperationalStatus.Up)
                        IsConnected = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return IsConnected;
        }

        private bool CheckRasDialConnection()
        {
            bool IsConnected = false;
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
                        }
                    }
                }
            }

            return IsConnected;
        }

        private void GoNext()
        {
            if (ActiveDialog == IoC.Get<ConfirmationViewModel>())
                ActiveDialog = IoC.Get<UnInstallerViewModel>();
        }

        private void GoBack()
        {

        }

        public void CancelClick()
        {
            ShowOverlay = true;
            var exitApp = IoC.Get<MessageBoxViewModel>().Show("Exiting PureVPN Setup",
            "Are you sure you want to exit PureVPN Setup and abort un-installation?",
            true, "YES", "NO");
            ShowOverlay = false;

            if (exitApp == true)
                CloseApp();
        }

        public bool ShowOverlay
        {
            get { return IoC.Get<MainViewModel>().ShowOverlay; }
            set { IoC.Get<MainViewModel>().ShowOverlay = value; }
        }

        public BaseViewModel ActiveDialog
        {
            get { return IoC.Get<MainViewModel>().ActiveViewModel; }
            set { IoC.Get<MainViewModel>().ActiveViewModel = value; }
        }

        protected override void OnViewUnloaded(object view)
        {

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

        private ICommand continueAction;
        public ICommand ContinueAction
        {
            get
            {
                if (continueAction == null)
                    continueAction = new ActionHelper<bool>(Continue);
                return continueAction;
            }
        }

        #endregion
    }
}
