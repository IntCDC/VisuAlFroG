using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Core.Utilities;
using Core.GUI;
using Core.Abstracts;
using System.Windows.Media;
using System.Windows.Documents;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Visualizations.Abstracts;
using Visualizations.Data;



/*
 * Log Window Content
 * 
 * TODO Optimize performance for huge amount of messages 
 * -> only add last x Inlines to fill screen and 
 * implement separate scrolling (with mouse wheel...)
 */
namespace Visualizations
{
    namespace Miscellaneous
    {
        public class LogConsole : AbstractGenericVisualization<System.Windows.Controls.TextBlock, DataInterfaceGeneric<GenericDataStructure>>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Log Console"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                var init = base.Initialize();
                // ! Initialize base class before registering listener
                Log.Default.RegisterListener(this.LogListener);
                return init;
            }

            public override bool ReCreate()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    // Log Console does not depend on data
                    Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();


                SetScrollViewBackground(ColorTheme.Brush_DarkBackground);

                Content.TextWrapping = TextWrapping.Wrap;
                Content.FontFamily = new FontFamily("Consolas");
                Content.Width = Double.NaN; // = "Auto"
                Content.Height = Double.NaN; // = "Auto"

                var copy_option = new MenuItem();
                copy_option.Header = "Copy to Clipboard";
                copy_option.Click += copy_option_click;
                AddOption(copy_option);


                _timer.Stop();
                _created = true;
                return _created;
            }

            public override bool Terminate()
            {
                Log.Default.UnRegisterListener(this.LogListener);
                return base.Terminate();
            }

            /// <summary>
            /// Log listener callback which should be called whenever new messages are available.
            /// </summary>
            /// <param name="msglist">provide only the new log messages</param>
            public void LogListener(List<Log.MessageData> msglist)
            {
                // Is called before Initialize() therefore _textblock needs to be not null
                foreach (Log.MessageData msg in msglist)
                {
                    // Default for Level.Info
                    var font_color = ColorTheme.Brush_LogMessageInfo;
                    switch (msg.level)
                    {
                        case (Log.Level.Warn):
                            font_color = ColorTheme.Brush_LogMessageWarn;
                            break;
                        case (Log.Level.Error):
                            font_color = ColorTheme.Brush_LogMessageError;
                            break;
                        case (Log.Level.Debug):
                            font_color = ColorTheme.Brush_LogMessageDebug;
                            break;
                    }
                    var run = new Run(msg.message + Environment.NewLine);
                    run.Foreground = font_color;

                    try
                    {
                        /// XXX Throws System.InvalidOperationException 
                        /// The calling thread cannot access this object because a different thread owns it
                        /// Occurs during shutdown when there are
                        Content.Inlines.Add(run);
                        ScrollToBottom();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }
            }

            /// <summary>
            /// DEBUG
            /// </summary>
            ~LogConsole()
            {
                Console.WriteLine("DEBUG - DTOR: LogConsole");
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void copy_option_click(object sender, RoutedEventArgs e)
            {
                string complete_log = "";
                foreach (var inline in Content.Inlines)
                {
                    var text_range = new TextRange(inline.ContentStart, inline.ContentEnd);
                    complete_log += text_range.Text;
                }
                Clipboard.SetText(complete_log);
            }
        }
    }
}
