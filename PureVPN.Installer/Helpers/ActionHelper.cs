using System;
using System.Windows.Input;
using Tally;

namespace PureVPN.Installer.Helpers
{
    public class ActionHelper<T> : ICommand
    {
        public Action Command { get; set; }

        public Action<T> Command2 { get; set; }

        public bool CanExecuteCommand { get; set; }

        public ActionHelper(Action<T> action, bool canExecute = true)
        {
            Command2 = action;
            CanExecuteCommand = canExecute;
        }

        public ActionHelper(Action action, bool canExecute = true)
        {
            Command = action;
            CanExecuteCommand = canExecute;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecuteCommand;
        }

        void ICommand.Execute(object parameter)
        {
            try
            {
                if (Command != null)
                    Command();
                else if (Command2 != null)
                    Command2((T)parameter);
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public event EventHandler CanExecuteChanged;
    }
}
