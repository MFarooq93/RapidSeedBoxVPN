using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PureVPN.Uninstaller.Helpers
{
    public static class Behaviour
    {
        public static readonly DependencyProperty LeftMouseDownCommand = EventBehaviourFactory.CreateCommandExecutionEventBehaviour(Control.MouseLeftButtonDownEvent, "LeftMouseDownCommand", typeof(Behaviour));

        public static void SetLeftMouseDownCommand(DependencyObject o, ICommand value)
        {
            o.SetValue(LeftMouseDownCommand, value);
        }

        public static ICommand GetLeftMouseDownCommand(DependencyObject o)
        {
            return o.GetValue(LeftMouseDownCommand) as ICommand;
        }
    
    }
}
