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



/*
 * Log Console
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class LogContent : AbstractContent
        {
            /* ------------------------------------------------------------------*/
            // static variables

            // Set for derived classes
            public static new readonly string name = "Log Console";
            public static new readonly bool multiple_instances = false;


            /* ------------------------------------------------------------------*/
            // public functions

            public LogContent()
            {
                Log.Default.RegisterListener(this.LogListener);
            }


            public override bool AttachContent(Grid content_element)
            {
                if (!_setup)
                {
                    setup_content();
                }
                content_element.Background = ColorTheme.BackgroundBlack;
                content_element.Children.Add(_content);

                _attached = true;
                return true;
            }


            public void LogListener(List<Log.MessageData> msglist)
            {
                if (!_setup)
                {
                    setup_content();
                }
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
                }
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected override void setup_content()
            {
                _content = new ScrollViewer();
                _content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _content.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                _textblock = new TextBlock();
                _textblock.TextWrapping = TextWrapping.Wrap;
                _textblock.FontFamily = new FontFamily("Consolas");
                _textblock.Width = Double.NaN; // = "Auto"
                _textblock.Height = Double.NaN; // = "Auto"

                _content.Content = _textblock;

                _setup = true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ScrollViewer _content = null;
            private TextBlock _textblock = null;
        }
    }
}
