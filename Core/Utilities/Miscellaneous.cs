using System;
using System.Windows.Threading;
using System.Windows;



/*
 * Miscellaneous helpers
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class Miscellaneous
        {
            /* ------------------------------------------------------------------*/
            #region static classes

            public static void AllowGlobalUIUpdate()
            {
                // Source: https://stackoverflow.com/questions/37787388/how-to-force-a-ui-update-during-a-lengthy-task-on-the-ui-thread
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
                {
                    frame.Continue = false;
                    return null;
                }), null);

                Dispatcher.PushFrame(frame);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }

            #endregion
        }
    }
}
