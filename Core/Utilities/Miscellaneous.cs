using System;
using System.Windows.Threading;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;



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

            public static Size MeasureButtonString(Button btn)
            {
                // Source: https://stackoverflow.com/questions/9264398/how-to-calculate-wpf-textblock-width-for-its-known-font-size-and-characters
                var formatted_text = new FormattedText(
                    (string)btn.Content,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(btn.FontFamily, btn.FontStyle, btn.FontWeight, btn.FontStretch),
                    btn.FontSize,
                    Brushes.Black,
                    null, //new NumberSubstitution(),
                    VisualTreeHelper.GetDpi(btn).PixelsPerDip);
                formatted_text.Trimming = TextTrimming.None;

                return new Size(formatted_text.Width, formatted_text.Height);
            }

            #endregion
        }
    }
}
