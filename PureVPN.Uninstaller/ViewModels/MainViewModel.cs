using MvvmCore;
using PureVPN.Uninstaller.Helpers;
using System.Windows.Input;

namespace PureVPN.Uninstaller.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private bool isWindowClosable = true;

        public bool IsWindowClosable
        {
            get { return isWindowClosable; }
            set
            {
                isWindowClosable = value;
                NotifyOfPropertyChange(() => IsWindowClosable);
            }
        }

        private bool showOverlay;
        public bool ShowOverlay
        {
            get { return showOverlay; }
            set
            {
                showOverlay = value;
                NotifyOfPropertyChange(() => ShowOverlay);
            }
        }

        private BaseViewModel activeViewModel;

        public BaseViewModel ActiveViewModel
        {
            get { return activeViewModel; }
            set
            {
                activeViewModel = value;
                IsWindowClosable = value != IoC.Get<UnInstallerViewModel>();
                NotifyOfPropertyChange(() => ActiveViewModel);
            }
        }

        protected override void OnViewLoaded(object view)
        {
            ActiveViewModel = IoC.Get<ConfirmationViewModel>();
            App.MainWindow.Closing += ClosingApp;
        }

        private void ClosingApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        #region Actions
        private ICommand closeAction;

        public ICommand CloseAction
        {
            get
            {
                if (closeAction == null)
                    closeAction = new ActionHelper<object>(CloseApp);
                return closeAction;
            }
        }

        private ICommand minimizeAction;

        public ICommand MinimizeAction
        {
            get
            {
                if (minimizeAction == null)
                    minimizeAction = new ActionHelper<object>(MinimizeApp);
                return minimizeAction;
            }
        }

        private ICommand dragWindowAction;

        public ICommand DragWindowAction
        {
            get
            {
                if (dragWindowAction == null)
                    dragWindowAction = new ActionHelper<object>(DragApp);
                return dragWindowAction;
            }
        }
        #endregion
    }
}
