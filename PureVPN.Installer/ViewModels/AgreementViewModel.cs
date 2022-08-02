using PureVPN.Installer.Helpers;
using PureVPN.Installer.Assistance;
using System;
using System.Windows.Input;
using System.Windows;

namespace PureVPN.Installer.ViewModels
{
    public class AgreementViewModel : BaseViewModel
    {
        private string agreementFilePath;

        public string AgreementFilePath
        {
            get { return agreementFilePath; }
            set
            {
                agreementFilePath = value;
                NotifyOfPropertyChange(() => AgreementFilePath);
            }
        }

        private bool isAgreementInReadingMode;

        public bool IsAgreementInReadingMode
        {
            get { return isAgreementInReadingMode; }
            set
            {
                isAgreementInReadingMode = value;
                NotifyOfPropertyChange(() => IsAgreementInReadingMode);
            }
        }

        private ICommand changeAgreementViewAction;

        public ICommand ChangeAgreementViewAction
        {
            get
            {
                if (changeAgreementViewAction == null)
                    changeAgreementViewAction = new ActionHelper<object>(ChangeAgreementMode);
                return changeAgreementViewAction;
            }
        }

        private void ChangeAgreementMode()
        {
            IsAgreementInReadingMode = !IsAgreementInReadingMode;
        }

        protected override void OnViewLoaded(object view)
        {
            if (String.IsNullOrEmpty(AgreementFilePath))
            {
                Utilities.ExtractZippedResource(Constants.TermsZip, Common.ProgramDataFolderPath);
                AgreementFilePath = Common.AgreementFilePath;
            }
        }

   

    }
}
