using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Core.Utilities;
using Core.Abstracts;
using System.Windows.Media;
using System.Windows.Documents;
using System.Runtime.CompilerServices;
using static Core.Utilities.Log;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;



/*
 * Log Content
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class LogContent : AbstractContent
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Log"; } }
            public override bool MultipleIntances { get { return false; } }
            public override List<Type> DependingServices { get { return new List<Type>() { }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public LogContent()
            {
                Log.Default.RegisterListener(this.LogListener);
            }

            ~LogContent()
            {
                Log.Default.UnRegisterListener(this.LogListener);
            }


            public override bool Create()
            {
                _timer.Start();

                _content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _content.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                _textblock.TextWrapping = TextWrapping.Wrap;
                _textblock.FontFamily = new FontFamily("Consolas");
                _textblock.Width = Double.NaN; // = "Auto"
                _textblock.Height = Double.NaN; // = "Auto"

                _content.Content = _textblock;

                _timer.Stop();

                _created = true;
                return _created;
            }


            public override bool Attach(Grid content_element)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return false;
                }

                content_element.Background = ColorTheme.BackgroundBlack;

                content_element.Children.Add(_content);
                _attached = true;
                return true;
            }


            public void LogListener(List<Log.MessageData> msglist)
            {
                // Is called before Initialize() therefore _textblock needs to be not null
                foreach (Log.MessageData msg in msglist)
                {
                    // Default for Level.Info
                    var font_color = ColorTheme.LogMessageInfo;
                    switch (msg.level)
                    {
                        case (Log.Level.Warn):
                            font_color = ColorTheme.LogMessageWarn;
                            break;
                        case (Log.Level.Error):
                            font_color = ColorTheme.LogMessageError;
                            break;
                        case (Log.Level.Debug):
                            font_color = ColorTheme.LogMessageDebug;
                            break;
                    }
                    var run = new Run(msg.message + Environment.NewLine);
                    run.Foreground = font_color;
                    _textblock.Inlines.Add(run);
                    _content.ScrollToBottom();
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ScrollViewer _content = new ScrollViewer();
            private TextBlock _textblock = new TextBlock();
        }
    }
}
