using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Documents;
using System;
using System.Windows.Navigation;
using Core.Abstracts;
using System.Collections.Generic;
using Core.Data;
using static Core.Data.DataManager;
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
            // public delegate

            public delegate bool MenuCallback_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            public bool Initialize(MenuCallback_Delegate app_close_callback, 
                                   MenuCallback_Delegate config_save_callback, MenuCallback_Delegate config_load_callback, 
                                   MenuCallback_Delegate data_save_callback, MenuCallback_Delegate data_load_callback, MenuCallback_Delegate data_send_callback,
                                   ColorTheme.SetColorStyle_Delegate theme_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();


                _item_data.Add(MenuItemData.APP_CLOSE, new Tuple<string, Func<bool>>("item_app_close_" + UniqueID.GenerateString(), app_close_callback.Invoke));
                _item_data.Add(MenuItemData.CONFIG_SAVE, new Tuple<string, Func<bool>>("item_config_save_" + UniqueID.GenerateString(), config_save_callback.Invoke));
                _item_data.Add(MenuItemData.CONFIG_LOAD, new Tuple<string, Func<bool>>("item_config_load_" + UniqueID.GenerateString(), config_load_callback.Invoke));
                _item_data.Add(MenuItemData.DATA_SAVE, new Tuple<string, Func<bool>>("item_data_save_" + UniqueID.GenerateString(), data_save_callback.Invoke));
                _item_data.Add(MenuItemData.DATA_LOAD, new Tuple<string, Func<bool>>("item_data_save_" + UniqueID.GenerateString(), data_load_callback.Invoke));
                _item_data.Add(MenuItemData.DATA_SEND, new Tuple<string, Func<bool>>("item_data_send_" + UniqueID.GenerateString(), data_send_callback.Invoke));
                _theme_callback = theme_callback;

                _content = new Menu();
                _content.Style = ColorTheme.MenuBarStyle();

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

                var menu_item = new MenuItem();
                menu_item.Header = "File";
                _content.Items.Add(menu_item);

                // -----------------

                var data_menu_item = new MenuItem();
                data_menu_item.Header = "Data";
                data_menu_item.Style = ColorTheme.MenuItemIconStyle();
                menu_item.Items.Add(data_menu_item);

                var sub_menu_item = new MenuItem();
                sub_menu_item.Header = "Save";
                sub_menu_item.Name = _item_data[MenuItemData.DATA_SAVE].Item1;
                sub_menu_item.Click += event_menumenu_item;
                sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
                data_menu_item.Items.Add(sub_menu_item);

                sub_menu_item = new MenuItem();
                sub_menu_item.Header = "Load";
                sub_menu_item.Name = _item_data[MenuItemData.DATA_LOAD].Item1;
                sub_menu_item.Click += event_menumenu_item;
                sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
                data_menu_item.Items.Add(sub_menu_item);

                // -----------------

                var config_menu_item = new MenuItem();
                config_menu_item.Header = "Configuration";
                config_menu_item.Style = ColorTheme.MenuItemIconStyle();
                menu_item.Items.Add(config_menu_item);

                sub_menu_item = new MenuItem();
                sub_menu_item.Header = "Save";
                sub_menu_item.Name = _item_data[MenuItemData.CONFIG_SAVE].Item1;
                sub_menu_item.Click += event_menumenu_item;
                sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
                config_menu_item.Items.Add(sub_menu_item);

                sub_menu_item = new MenuItem();
                sub_menu_item.Header = "Load";
                sub_menu_item.Name = _item_data[MenuItemData.DATA_LOAD].Item1;
                sub_menu_item.Click += event_menumenu_item;
                sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
                config_menu_item.Items.Add(sub_menu_item);

                // -----------------

                menu_item.Items.Add(new Separator());

                var exit_menu_item = new MenuItem();
                exit_menu_item.Header = "Exit";
                exit_menu_item.Name = _item_data[MenuItemData.APP_CLOSE].Item1;
                exit_menu_item.Click += event_menumenu_item;
                exit_menu_item.Style = ColorTheme.MenuItemIconStyle();
                menu_item.Items.Add(exit_menu_item);

                // ----------------------------------------

                var style_menu_item = new MenuItem();
                style_menu_item.Header = "Style";
                _content.Items.Add(style_menu_item);

                var theme_values = Enum.GetValues(typeof(ColorTheme.PredefinedThemes));
                foreach (ColorTheme.PredefinedThemes theme in theme_values)
                {
                    sub_menu_item = new MenuItem();
                    sub_menu_item.Header = Enum.GetName(theme.GetType(), theme);
                    sub_menu_item.Name = "theme_item_" + UniqueID.GenerateString();
                    sub_menu_item.Click += event_menumenu_item;
                    sub_menu_item.IsCheckable = true;
                    sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
                    style_menu_item.Items.Add(sub_menu_item);

                    _theme_data.Add(sub_menu_item.Name, new Tuple<MenuItem, ColorTheme.PredefinedThemes>(sub_menu_item, theme));
                }
                // Add manually since default theme in ColorTheme is set in Initialize when this menu is not yet available
                MarkColorTheme(ColorTheme._DefaultColorTheme);

                // ----------------------------------------

                var info_menu_item = new MenuItem();
                info_menu_item.Header = "Info";
                _content.Items.Add(info_menu_item);

                var hyper_link = new Hyperlink();
                hyper_link.NavigateUri = new Uri("https://github.com/IntCDC/VisuAlFroG");
                hyper_link.RequestNavigate += event_hyperlink_requestnavigate;
                hyper_link.Inlines.Add("GitHub Repository");
                hyper_link.Style = ColorTheme.HyperlinkStyle();

                sub_menu_item = new MenuItem();
                sub_menu_item.Style = ColorTheme.MenuItemIconStyle("github.png");
                sub_menu_item.Header = hyper_link;
                info_menu_item.Items.Add(sub_menu_item);

                // ----------------------------------------

                var send_data_menu_item = new MenuItem();
                send_data_menu_item.Header = "Send Output Data";
                send_data_menu_item.Name = _item_data[MenuItemData.DATA_SEND].Item1;
                send_data_menu_item.Click += event_menumenu_item;
                send_data_menu_item.Style = ColorTheme.MenuItemHighlightStyle();
                _content.Items.Add(send_data_menu_item);


                return _content;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _theme_callback = null;
                    _theme_data.Clear();
                    _item_data.Clear();

                    _initialized = false;
                }
                return true;
            }

            public void MarkColorTheme(ColorTheme.PredefinedThemes color_theme)
            {
                foreach (var theme in _theme_data)
                {
                    theme.Value.Item1.IsChecked = false;
                    if (color_theme == theme.Value.Item2)
                    {
                        theme.Value.Item1.IsChecked = true;
                    }
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Called when menu item is clicked. Same for all menu items.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The routed event arguments.</param>
            private void event_menumenu_item(object sender, RoutedEventArgs e)
            {
                var sender_content = sender as MenuItem;
                if (sender_content == null)
                {
                    return;
                }
                string content_id = sender_content.Name;

                bool found_menu_item = false;
                foreach (var item in _item_data)
                {
                    if (content_id == item.Value.Item1)
                    {
                        if (item.Value.Item2 == null)
                        {
                            Log.Default.Msg(Log.Level.Error, "Missing callback for menu item: " + item.Value.Item1);
                        }
                        else
                        {
                            item.Value.Item2();
                            found_menu_item = true;
                            break;
                        }
                    }
                }

                // Check color themes separately
                if (!found_menu_item)
                {
                    foreach (var theme in _theme_data)
                    {
                        theme.Value.Item1.IsChecked = false;
                        if ((content_id == theme.Key) && (_theme_callback != null))
                        {
                            _theme_callback(theme.Value.Item2);
                            theme.Value.Item1.IsChecked = true;
                        }
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

            private enum MenuItemData
            {
                APP_CLOSE,
                CONFIG_SAVE,
                CONFIG_LOAD,
                DATA_LOAD,
                DATA_SAVE,
                DATA_SEND,
            }
            private Dictionary<MenuItemData, Tuple<string, Func<bool>>> _item_data = new Dictionary<MenuItemData, Tuple<string, Func<bool>>>();

            private ColorTheme.SetColorStyle_Delegate _theme_callback = null;

            private Dictionary<string, Tuple<MenuItem, ColorTheme.PredefinedThemes>> _theme_data = new Dictionary<string, Tuple<MenuItem, ColorTheme.PredefinedThemes>>();
        }
    }
}
