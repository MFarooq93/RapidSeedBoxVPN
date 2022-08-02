using PureVPN.Installer.ViewModels;
using System.Collections.Generic;

namespace PureVPN.Installer.Interfaces
{
    public interface IWindowManager
    {
        bool? ShowDialog(BaseViewModel rootViewModel, IDictionary<string, object> settings = null);
        void ShowWindow(BaseViewModel rootViewModel, IDictionary<string, object> settings = null);
        void Close(bool result = false);
    }
}
