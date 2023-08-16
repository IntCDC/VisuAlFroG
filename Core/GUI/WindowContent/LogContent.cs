using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Core.Utilities;
using Core.Abstracts;
using System.Windows.Media;
using System.Windows.Documents;



/*
 * GUI Log Console
 * 
 */
namespace Core
{
    namespace GUI
    {
        public sealed class LogContent : AbstractContent
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public LogContent() : base("Log Console")
            {
                setup_content();
            }


            public override bool AttachContent(Grid content_grid)
            {
                content_grid.Background = Brushes.Black;
                content_grid.Children.Add(_content);

                _attached = true;
                return true;
            }


            public void LogListener(Log.MessageData msgdata)
            {
                string trace_file = "[" + Path.GetFileName(msgdata.caller_file) + "]";
                string trace_method = "[" + msgdata.caller_method + "]";
                string trace_line = "[" + msgdata.caller_line + "]";

                // Default for Level.Info
                var font_color = Brushes.White;
                string level_prefix = "<INFO> ";
                switch (msgdata.level)
                {
                    case (Log.Level.Warn):
                        font_color = Brushes.Yellow;
                        level_prefix = "<WARN>";
                        break;
                    case (Log.Level.Error):
                        font_color = Brushes.Red;
                        level_prefix = "<ERROR>";
                        break;
                    case (Log.Level.Debug):
                        font_color = Brushes.Gray;
                        level_prefix = "<DEBUG>";
                        break;
                }

                // Fixed padding
                string trace_meta = level_prefix + " " + trace_file + trace_method + trace_line;
                trace_meta = trace_meta.PadRight(60, ' ');
                string text = (trace_meta + " > " + msgdata.message + Environment.NewLine);

                var run = new Run(text);
                run.Foreground = font_color;
                _textblock.Inlines.Add(run);
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void setup_content()
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
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ScrollViewer _content = null;
            private TextBlock _textblock = null;
        }

    }
}
