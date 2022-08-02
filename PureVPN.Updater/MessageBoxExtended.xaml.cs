using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tally;

namespace PureVPN.Updater
{
    /// <summary>
    /// Interaction logic for MessageBoxExtended.xaml
    /// </summary>
    public partial class MessageBoxExtended : Window
    {
        private Action PositiveAction = null, NegativeAction = null;
        private object[] PositiveParams = null, NegativeParams = null;
        private MessageBoxResult MessageBoxExtendedResult = MessageBoxResult.OK;

        public MessageBoxExtended()
        {
            InitializeComponent();
        }

        public MessageBoxExtended(string Caption, string Message, MessageBoxButton Buttons = MessageBoxButton.OK, Action PositiveButtonAction = null, object[] PositiveButtonParams = null, Action NegativeButtonAction = null, object[] NegativeButtonParams = null, bool invertButtonColors = false)
        {
            InitializeComponent();
            this.SetProperties(Message, Caption, Buttons, invertButtonColors: invertButtonColors);

            PositiveAction = PositiveButtonAction;
            NegativeAction = NegativeButtonAction;
            PositiveParams = PositiveButtonParams;
            NegativeParams = NegativeButtonParams;
        }

        private void PositiveButton_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxExtendedResult = this.PositiveButton.Content.ToString() == Properties.Resources.Yes.ToString() ? MessageBoxResult.Yes : MessageBoxResult.OK;
            this.DialogResult = true;
            if (PositiveAction != null)
                PositiveAction.Method.Invoke(null, PositiveParams ?? null);
            this.Close();
        }

        private void NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxExtendedResult = this.NegativeButton.Content.ToString() ==Properties.Resources.No.ToString() ? MessageBoxResult.No : MessageBoxResult.Cancel;
            this.DialogResult = false;
            if (NegativeAction != null)
                NegativeAction.Method.Invoke(NegativeParams ?? null, null);
            this.Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            //No need to move Window
        }
        
        public static MessageBoxResult Show(string Message, string Caption, MessageBoxButton Buttons = MessageBoxButton.OK, string popTitle = "", bool invertButtonColors = false, string positiveButtonContent = "", string negativeButtonContent = "", bool EnableBackground = true, bool IsFromAppSettings=false, bool IsChangePassword = false, bool IsDontShowAgain = false)
        {
           

            MessageBoxExtended messageBox = new MessageBoxExtended();
            messageBox.SetProperties(Message, Caption, Buttons, popTitle, invertButtonColors, positiveButtonContent, negativeButtonContent, IsFromAppSettings, IsDontShowAgain);

            messageBox.ShowDialog();
            Application.Current.MainWindow.IsEnabled = EnableBackground;
          
            return messageBox.MessageBoxExtendedResult;
        }

        private void HandleDontShowMessageCheck(object sender, RoutedEventArgs e)
        {
            var a = sender as CheckBox;

            
        }

        private void SetProperties(string Message, string Caption, MessageBoxButton Buttons, string popTitle = "", bool invertButtonColors = false, string positiveButtonContent = "", string negativeButtonContent = "", bool IsFromAppSettings = false, bool IsDontShowAgain = false)
        {
            CaptionBlock.Text = Caption;
            MessageBlock.Text = Message;
            
            //PopupTitle.Text = "Titlr";
            //if (!String.IsNullOrEmpty(popTitle))
               // PopupTitle.Text = popTitle;

            NegativeButton.Visibility = Visibility.Collapsed;
            PositiveButton.Content = Properties.Resources.Yes;
        
            MessageBlock.FlowDirection = FlowDirection.LeftToRight;
            CaptionBlock.FlowDirection = FlowDirection.LeftToRight;
            CaptionBlock.HorizontalAlignment = HorizontalAlignment.Left;

            switch (Buttons)
            {
                case MessageBoxButton.OKCancel:
                {
                    NegativeButton.Visibility = Visibility.Visible;
                    NegativeButton.Content = Properties.Resources.Cancel;
                }
                break;

                case MessageBoxButton.YesNo:
                case MessageBoxButton.YesNoCancel:
                {
                    NegativeButton.Visibility = Visibility.Visible;
                    PositiveButton.Content = Properties.Resources.Yes;
                    NegativeButton.Content = Properties.Resources.No;
                }
                break;
            }

            if (!String.IsNullOrEmpty(positiveButtonContent))
                PositiveButton.Content = positiveButtonContent;

            if (!String.IsNullOrEmpty(negativeButtonContent))
                NegativeButton.Content = negativeButtonContent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application curApp = Application.Current;
                Window mainWindow = curApp.MainWindow;
                mainWindow.IsEnabled = false;

                if (!mainWindow.IsVisible)
                {
                    mainWindow.Width = 610.4;
                    mainWindow.Height = 515.2;
                }
                
                this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
                this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;

               // ContentBorder.Height = this.ActualHeight;
                //ContentBorder.Width = this.ActualWidth;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        
    }
}
