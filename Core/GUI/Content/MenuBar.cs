using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Documents;
using System;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Diagnostics.Tracing;
using Core.Abstracts;
using System.Text;
using System.Runtime.Remoting.Contexts;



/*
 * Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class MenuBar : AbstractService
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows to trigger relaoding of the interface
            /// </summary>
            public delegate void WindowClose_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            public bool Initialize(WindowClose_Delegate close_callback)
            {
                if (_initilized)
                {
                    Terminate();
                }
                if (close_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing content callbacks");
                    return false;
                }
                _timer.Start();

                _close_callback = close_callback;
                _content = new Menu();

                _timer.Stop();
                _initilized = true;
                return _initilized;
            }


            public Control Attach()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                var item_file = new MenuItem();
                item_file.Header = "File";

                var item_close = new MenuItem();
                item_close.Header = "Close";
                item_close.Name = _item_id_close;

                item_close.Style = ColorTheme.MenuItemStyle();
                item_file.Items.Add(item_close);

                if (_close_callback == null)
                {
                    item_close.IsEnabled = false;
                    Log.Default.Msg(Log.Level.Warn, "Missing window close callback, call RegisterCloseCallback beforehand");
                }
                else
                {
                    item_close.Click += menuitem_click;
                }

                var item_info = new MenuItem();
                item_info.Header = "Info";

                var hyper_link = new Hyperlink();
                hyper_link.NavigateUri = new Uri("https://github.com/IntCDC/VisFroG");
                hyper_link.RequestNavigate += hyperlink_requestnavigate;
                hyper_link.Inlines.Add("GitHub Repository");
                hyper_link.Style = ColorTheme.HyperlinkStyle();

                var item_github_link = new MenuItem();
                item_github_link.Style = ColorTheme.MenuItemStyle("github.png");
                item_github_link.Header = hyper_link;
                item_info.Items.Add(item_github_link);


                _content.Height = 20.0;
                _content.Style = ColorTheme.MenuStyle();

                _content.Items.Add(item_file);
                _content.Items.Add(item_info);

                return _content;
            }


            public override bool Terminate()
            {
                if (_initilized)
                {
                    _content = null;
                    _close_callback = null;

                    _initilized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void menuitem_click(object sender, RoutedEventArgs e)
            {
                var sender_content = sender as MenuItem;
                if (sender_content == null)
                {
                    return;
                }

                string content_id = sender_content.Name;
                if (content_id == _item_id_close)
                {
                    if (_close_callback != null)
                    {
                        _close_callback();
                    }
                }
            }


            private void hyperlink_requestnavigate(object sender, RequestNavigateEventArgs e)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                System.Diagnostics.Process.Start(e.Uri.ToString());
                e.Handled = true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Menu _content = null;
            private WindowClose_Delegate _close_callback = null;

            private readonly string _item_id_close = "item_close_" + UniqueID.Generate();
        }
    }
}
