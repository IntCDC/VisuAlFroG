using System;
using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections.Generic;
using Core.Data;
using Core.Abstracts;
using Visualizations.WPFInterface;



/*
 * Log Window Content
 * 
 * Default window content set in WindowManager.CreateDefault()
 * Control Hierarchy: ScrollViewer(Content) -> TextBlick(_text_block)
 * 
 * XXX TODO Optimize performance for huge amount of messages -> render only visible lines (required full copy of all messages?)
 * 
 */
namespace Visualizations
{
    public class WPF_LogConsole : AbstractWPFVisualization<ScrollViewer>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Log Console (WPF)"; } }
        public override bool _MultipleInstances { get { return false; } }


        /* ------------------------------------------------------------------*/
        // public functions

        public override bool Create()
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


            _text_block = new TextBlock();
            _text_block.TextWrapping = TextWrapping.Wrap;
            _text_block.FontFamily = new FontFamily("Consolas"); // Use mono space font for better formatting
            _text_block.Width = Double.NaN; // = "Auto"
            _text_block.Height = Double.NaN; // = "Auto"

            Content.Name = _ID;
            Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
            Content.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
            Content.PreviewMouseWheel += event_scrollviewer_mousewheel;
            Content.SetResourceReference(StackPanel.BackgroundProperty, "Brush_Background");
            Content.Content = _text_block;

            var copy_option = new MenuItem();
            copy_option.Header = "Copy to Clipboard";
            copy_option.Click += event_option_click;
            AddMenuOption(copy_option);

            // Call after _text_block has been created
            Log.Default.RegisterListener(this.LogListener);


            _timer.Stop();
            _created = true;
            return _created;
        }

        public override void Update(bool new_data)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }
            // Nothing to do here...
        }

        public override bool Terminate()
        {
            Log.Default.UnRegisterListener(this.LogListener);
            return base.Terminate();
        }

        /// <summary>
        /// Log listener callback which should be called whenever new messages are available.
        /// </summary>
        /// <param name="msglist">Provides only the new log messages</param>
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
                    if (_text_block == null) {
                        Log.Default.Msg(Log.Level.Error, "Text Block element is null");
                        return;
                    }
                    _text_block.Inlines.Add(run);
                    scroll_bottom();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }


        /* ------------------------------------------------------------------*/
        // private functions

        private void event_option_click(object sender, RoutedEventArgs e)
        {
            string complete_log = "";
            foreach (var inline in _text_block.Inlines)
            {
                var text_range = new TextRange(inline.ContentStart, inline.ContentEnd);
                complete_log += text_range.Text;
            }
            Clipboard.SetText(complete_log);
        }

        private void event_scrollviewer_mousewheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            scv.UpdateLayout();
            e.Handled = true;
        }

        protected void set_scroll_background(string background_color_resource_name)
        {
            Content.SetResourceReference(ScrollViewer.BackgroundProperty, background_color_resource_name);
        }

        protected void scroll_bottom()
        {
            Content.ScrollToBottom();
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private TextBlock _text_block = null;

    }
}
