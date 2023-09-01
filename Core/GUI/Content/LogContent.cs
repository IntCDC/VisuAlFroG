using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Core.Utilities;
using Core.Abstracts;
using System.Windows.Media;
using System.Windows.Documents;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;



/*
 * Log Window Content
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

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                _textblock = new TextBlock();
                _content = new ScrollViewer();
                _content.Name = ID;
                Log.Default.RegisterListener(this.LogListener);

                _timer.Stop();
                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }


            public override bool Create()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Content already created, skipping...");
                    return false;
                }
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


            public override Control Attach()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                _content.Background = ColorTheme.BackgroundBlack;

                _attached = true;
                return _content;
            }


            public override bool Terminate()
            {
                if (_initilized)
                {
                    _content = null;
                    _textblock = null;
                    Log.Default.UnRegisterListener(this.LogListener);

                    _initilized = false;
                }
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

            private ScrollViewer _content = null;
            private TextBlock _textblock = null;
        }
    }
}
