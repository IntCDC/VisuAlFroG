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
using Core.GUI;



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
            /// Callback provided by the main WPF application on closing. 
            /// </summary>
            public delegate void WindowClose_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            public bool Initialize(WindowClose_Delegate close_callback, ColorTheme.SetColorStyle_Delegate theme_callback, ConfigurationService.Save_Delegate save_callback, ConfigurationService.Load_Delegate load_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                if ((close_callback == null) || (save_callback == null) || (load_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                    return false;
                }
                _timer.Start();

                _close_callback = close_callback;
                _save_callback = save_callback;
                _load_callback = load_callback;
                _theme_callback = theme_callback;

                _content = new Menu();
                _content.Style = ColorTheme.MenuStyle();

                _timer.Stop();
                _initialized = true;
                return _initialized;
            }

            public Control Attach()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                // ----------------------------------------
                var item_file = new MenuItem();
                item_file.Header = "File";
                _content.Items.Add(item_file);

                var item_close = new MenuItem();
                item_close.Header = "Close";
                item_close.Name = _item_id_close;
                item_close.Click += event_menuitem_click;
                item_close.Style = ColorTheme.MenuItemStyle();
                item_file.Items.Add(item_close);

                var item_configurations = new MenuItem();
                item_configurations.Header = "Configuration";
                item_configurations.Style = ColorTheme.MenuItemStyle();
                item_file.Items.Add(item_configurations);

                var item_save = new MenuItem();
                item_save.Header = "Save";
                item_save.Name = _item_id_save;
                item_save.Click += event_menuitem_click;
                item_save.Style = ColorTheme.MenuItemStyle();
                item_configurations.Items.Add(item_save);

                var item_load = new MenuItem();
                item_load.Header = "Load";
                item_load.Name = _item_id_load;
                item_load.Click += event_menuitem_click;
                item_load.Style = ColorTheme.MenuItemStyle();
                item_configurations.Items.Add(item_load);

                // ----------------------------------------
                var item_style = new MenuItem();
                item_style.Header = "Style";
                _content.Items.Add(item_style);

                var item_theme1 = new MenuItem();
                item_theme1.Header = "Light Blue";
                item_theme1.Name = _item_id_theme1;
                item_theme1.Click += event_menuitem_click;
                item_theme1.Style = ColorTheme.MenuItemStyle();
                item_style.Items.Add(item_theme1);

                var item_theme2 = new MenuItem();
                item_theme2.Header = "Dark Contrast";
                item_theme2.Name = _item_id_theme2;
                item_theme2.Click += event_menuitem_click;
                item_theme2.Style = ColorTheme.MenuItemStyle();
                item_style.Items.Add(item_theme2);

                // ----------------------------------------
                var item_info = new MenuItem();
                item_info.Header = "Info";
                _content.Items.Add(item_info);

                var hyper_link = new Hyperlink();
                hyper_link.NavigateUri = new Uri("https://github.com/IntCDC/VisuAlFroG");
                hyper_link.RequestNavigate += event_hyperlink_requestnavigate;
                hyper_link.Inlines.Add("GitHub Repository");
                hyper_link.Style = ColorTheme.HyperlinkStyle();

                var item_github_link = new MenuItem();
                item_github_link.Style = ColorTheme.MenuItemStyle("github.png");
                item_github_link.Header = hyper_link;
                item_info.Items.Add(item_github_link);

                return _content;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content = null;
                    _close_callback = null;
                    _save_callback = null;
                    _load_callback = null;
                    _theme_callback = null;

                    _initialized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Called when menu item is clicked. Same for all menu items.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The routed event arguments.</param>
            private void event_menuitem_click(object sender, RoutedEventArgs e)
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
                else if (content_id == _item_id_save)
                {
                    if (_save_callback != null)
                    {
                        _save_callback();
                    }
                }
                else if (content_id == _item_id_load)
                {
                    if (_load_callback != null)
                    {
                        _load_callback();
                    }
                }
                else if (content_id == _item_id_theme1)
                {
                    if (_theme_callback != null)
                    {
                        _theme_callback(ColorTheme.ColorStyle.LightBlue);
                    }
                }
                else if (content_id == _item_id_theme2)
                {
                    if (_theme_callback != null)
                    {
                        _theme_callback(ColorTheme.ColorStyle.DarkContrast);
                    }
                }
            }

            /// <summary>
            /// Called to open URL in new browser tab.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The request navigate event arguments.</param>
            private void event_hyperlink_requestnavigate(object sender, RequestNavigateEventArgs e)
            {
                System.Diagnostics.Process.Start(e.Uri.ToString());
                /// or Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Menu _content = null;
            private WindowClose_Delegate _close_callback = null;
            private ConfigurationService.Save_Delegate _save_callback = null;
            private ConfigurationService.Load_Delegate _load_callback = null;
            private ColorTheme.SetColorStyle_Delegate _theme_callback = null;

            private readonly string _item_id_close = "item_close_" + UniqueID.Generate();
            private readonly string _item_id_save = "item_save_" + UniqueID.Generate();
            private readonly string _item_id_load = "item_load_" + UniqueID.Generate();

            private readonly string _item_id_theme1 = "item_theme1_" + UniqueID.Generate();
            private readonly string _item_id_theme2 = "item_theme2_" + UniqueID.Generate();
        }
    }
}
