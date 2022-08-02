using PureVPN.Installer.Helpers;
using System;
using System.Windows.Input;
using Tally;

namespace PureVPN.Installer.ViewModels
{
    public class AdditionalTasksViewModel : BaseViewModel
    {
        private bool isEnteredPathValid;

        public bool IsEnteredPathValid
        {
            get { return isEnteredPathValid; }
            set
            {
                isEnteredPathValid = value;
                NotifyOfPropertyChange(() => IsEnteredPathValid);
            }
        }

        private string enteredPath;

        public string EnteredPath
        {
            get { return enteredPath; }
            set
            {
                enteredPath = value;
                try
                {
                    IsEnteredPathValid = false;
                    IsEnteredPathValid = IOHelper.IsDirectoryPresent(value);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                NotifyOfPropertyChange(() => EnteredPath);
            }
        }

        private bool isLaunchOnStartupChecked;

        public bool IsLaunchOnStartupChecked
        {
            get { return isLaunchOnStartupChecked; }
            set
            {
                isLaunchOnStartupChecked = value;
                NotifyOfPropertyChange(() => IsLaunchOnStartupChecked);
            }
        }

        private bool isLaunchAfterSetupChecked;

        public bool IsLaunchAfterSetupChecked
        {
            get { return isLaunchAfterSetupChecked; }
            set
            {
                isLaunchAfterSetupChecked = value;
                NotifyOfPropertyChange(() => IsLaunchAfterSetupChecked);
            }
        }

        private ICommand fileDialogAction;

        public ICommand FileDialogAction
        {
            get
            {
                if (fileDialogAction == null)
                    fileDialogAction = new ActionHelper<object>(OpenFileDialog);
                return fileDialogAction;
            }
        }

        private void OpenFileDialog()
        {
            string path = IOHelper.OpenFolderBrowser();
            EnteredPath = String.IsNullOrEmpty(path) ? EnteredPath : path;
        }

        private string ProgramFilesPath { get { return Environment.GetFolderPath(Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles); } }

        protected override void OnViewLoaded(object view)
        {
            if (String.IsNullOrEmpty(EnteredPath))
            {
                var path = IOHelper.CombinePaths(ProgramFilesPath, "GZ Systems\\PureVPN");
                if (!IOHelper.IsDirectoryPresent(path))
                    IOHelper.CreateDirectory(path);
                EnteredPath = path;
            }

            HasLoadedOnce = true;
        }

        public bool HasLoadedOnce { get; set; }
    }
}
