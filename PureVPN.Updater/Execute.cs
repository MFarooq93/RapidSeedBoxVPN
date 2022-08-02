using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tally;

namespace PureVPN.Updater
{
    public static class Execute
    {
        public static void UIOperation(Action action)
        {
            if (action == null)
                return;
            try
            {
                Application.Current.Dispatcher.Invoke(action);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static Task NewTask(Action action, bool isLongTask = true)
        {
            Task task = null;
            if (action != null)
            {
                try
                {
                    if (isLongTask)
                    {
                        task = Task.Factory.StartNew(() =>
                        {
                            try { action(); }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        },
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.Default);
                    }
                    else
                    {
                        task = Task.Factory.StartNew(() =>
                        {
                            try { action(); }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return task;
        }

        public static Task NewTask<T>(Func<T> action)
        {
            Task task = null;
            if (action != null)
            {
                try { task = Task.Factory.StartNew<T>(action); }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return task;
        }
    }
}
