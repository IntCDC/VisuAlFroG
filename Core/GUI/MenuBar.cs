﻿using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using Core.GUI;
using System.Windows.Documents;
using System;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Diagnostics.Tracing;



/*
 * Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class MenuBar
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows to trigger relaoding of the interface
            /// </summary>
            public delegate void CloseCallback();


            /* ------------------------------------------------------------------*/
            // public functions

            public void Create(Menu content_element)
            {
                content_element.Height = 20.0;
                content_element.Style = ColorTheme.MenuStyle();


                var item_file = new MenuItem();
                item_file.Header = "File";

                var item_close = new MenuItem();
                item_close.Header = "Close";
                item_close.Name = _item_id_close;
                item_close.Click += menuitem_click;
                item_close.Style = ColorTheme.MenuItemStyle();
                item_file.Items.Add(item_close);

                content_element.Items.Add(item_file);


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

                content_element.Items.Add(item_info);
            }


            public void RegisterCloseCallback(CloseCallback close_callback)
            {
                _close_callback = close_callback;
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
                    else
                    {
                        Log.Default.Msg(Log.Level.Warn, "Missing close callback");
                    }
                }
            }

            private void hyperlink_requestnavigate(object sender, RequestNavigateEventArgs e)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
                System.Diagnostics.Process.Start(e.Uri.ToString());
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private CloseCallback _close_callback = null;

            private readonly string _item_id_close = "item_close_" + UniqueStringID.Generate();
        }
    }
}
