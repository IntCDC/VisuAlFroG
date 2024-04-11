using System;
using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections.Generic;
using Core.Data;



/*
 * Log Window Content
 * 
 * TODO Optimize performance for huge amount of messages 
 * -> only add last x Inlines to fill screen and implement separate scrolling (with mouse wheel...)
 * 
 * Requested data is NOT used ...
 */
namespace Visualizations
{
    namespace Generic
    {
        public class LogConsole : AbstractGenericVisualization<System.Windows.Controls.TextBlock>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Log Console"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override Type GetDataType()
            {
                return typeof(GenericDataStructure);
            }

            public override bool Initialize(DataManager.RequestCallback_Delegate request_callback)
            {
                var init = base.Initialize(request_callback);
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


                SetScrollViewBackground("Brush_Background");

                Content.TextWrapping = TextWrapping.Wrap;
                Content.FontFamily = new FontFamily("Consolas");
                Content.Width = Double.NaN; // = "Auto"
                Content.Height = Double.NaN; // = "Auto"

                var copy_option = new MenuItem();
                copy_option.Header = "Copy to Clipboard";
                copy_option.Click += event_option_click;
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
                    var run = new Run(msg.message + Environment.NewLine);
                    run.SetResourceReference(Run.ForegroundProperty, "Brush_LogMessageInfo");
                    switch (msg.level)
                    {
                        case (Log.Level.Warn):
                            run.SetResourceReference(Run.ForegroundProperty, "Brush_LogMessageWarn");
                            break;
                        case (Log.Level.Error):
                            run.SetResourceReference(Run.ForegroundProperty, "Brush_LogMessageError");
                            break;
                        case (Log.Level.Debug):
                            run.SetResourceReference(Run.ForegroundProperty, "Brush_LogMessageDebug");
                            break;
                    }

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

            private void event_option_click(object sender, RoutedEventArgs e)
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
