using System;
using Tally;
using System.Windows.Input;
using PureVPN.Uninstaller.Helpers;

namespace PureVPN.Uninstaller.ViewModels
{
    public class MessageBoxViewModel : BaseViewModel
    {
        private string popupTitle = "PureVPN - Confirmation";

        public string PopupTitle
        {
            get { return popupTitle; }
            set
            {
                popupTitle = value;
                NotifyOfPropertyChange(() => PopupTitle);
            }
        }

        private bool showCheckbox;

        public bool ShowCheckbox
        {
            get { return showCheckbox; }
            set
            {
                showCheckbox = value;
                NotifyOfPropertyChange(() => ShowCheckbox);
            }
        }

        private bool isCheckboxChecked;

        public bool IsCheckboxChecked
        {
            get { return isCheckboxChecked; }
            set
            {
                isCheckboxChecked = value;
                NotifyOfPropertyChange(() => IsCheckboxChecked);
            }
        }

        private string caption;

        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        private bool showButtons;

        public bool ShowButtons
        {
            get { return showButtons; }
            set
            {
                showButtons = value;
                NotifyOfPropertyChange(() => ShowButtons);
            }
        }

        private string positiveButtonContent = "OK";

        public string PositiveButtonContent
        {
            get { return positiveButtonContent; }
            set
            {
                positiveButtonContent = value;
                NotifyOfPropertyChange(() => PositiveButtonContent);
            }
        }

        private string negativeButtonContent;

        public string NegativeButtonContent
        {
            get { return negativeButtonContent; }
            set
            {
                negativeButtonContent = value;
                NotifyOfPropertyChange(() => NegativeButtonContent);
            }
        }

        public System.Action<bool> PositiveAction { get; set; }
        public System.Action NegativeAction { get; set; }

        public bool? Show(string caption = "", string message = "", bool showButtons = true, string positiveButtonContent = "", string negativeButtonContent = "", System.Action<bool> positiveButtonAction = null, System.Action negativeButtonAction = null, bool showCheckbox = false, bool isCheckboxChecked = false)
        {
            if (!String.IsNullOrEmpty(popupTitle))
                PopupTitle = popupTitle;

            if (!String.IsNullOrEmpty(positiveButtonContent))
                PositiveButtonContent = positiveButtonContent;

            Caption = caption;
            Message = message;
            ShowButtons = showButtons && (!String.IsNullOrEmpty(Caption) || !String.IsNullOrEmpty(Message));
            NegativeButtonContent = negativeButtonContent;
            ShowCheckbox = showCheckbox;
            IsCheckboxChecked = isCheckboxChecked;

            PositiveAction = positiveButtonAction;
            NegativeAction = negativeButtonAction;

            return DialogHelper.ShowDialog(this);
        }

        public void ButtonClicked(bool isResponcePositive)
        {
            try
            {
                if (PositiveAction != null || NegativeAction != null)
                {
                    if (PositiveAction != null && isResponcePositive)
                        PositiveAction(ShowCheckbox && IsCheckboxChecked);
                    else if (NegativeAction != null && !isResponcePositive)
                        NegativeAction();
                    DialogHelper.CloseActiveDialog();
                }
                else
                    DialogHelper.CloseActiveDialog(isResponcePositive);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private ICommand buttonClickAction;

        public ICommand ButtonClickAction
        {
            get
            {
                if (buttonClickAction == null)
                    buttonClickAction = new ActionHelper<string>((r) => ButtonClicked(r));
                return buttonClickAction;
            }
        }

        private void ButtonClicked(string responce)
        {
            ButtonClicked(!String.IsNullOrEmpty(responce) && (responce.ToLower().Equals("true") || responce.ToLower().Equals("1")));
        }

    }
}
