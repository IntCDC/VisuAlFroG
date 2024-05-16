using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Documents;
using System;
using System.Windows.Navigation;
using Core.Abstracts;
using System.Collections.Generic;
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
            // public enum

            // Provide fixed main menu options
            public enum MainMenuOption
            {
                FILE,
                DATA,
                STYLE,
                HELP,
            }

            /* ------------------------------------------------------------------*/
            // static functions

            public static MenuItem GetDefaultMenuItem(string name, MenuCallback_Delegate callback = null)
            {
                var menu_item = new MenuItem();
                menu_item.Header = name;
                menu_item.Name = "menu_item_" + UniqueID.GenerateString();
                if (callback != null)
                {
                    menu_item.Click += (object sender, RoutedEventArgs e) =>
                    {
                        var sender_content = sender as MenuItem;
                        if (sender_content == null)
                        {
                            return;
                        }
                        callback();
                    };
                }
                menu_item.Style = ColorTheme.MenuItemIconStyle();
                return menu_item;
            }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();


                _content = new Menu();
                _content.Style = ColorTheme.MenuBarStyle();


                _main_menu_items = new Dictionary<MainMenuOption, MenuItem>();

                var main_menu_item = new MenuItem();
                main_menu_item.Header = "File";
                _main_menu_items.Add(MainMenuOption.FILE, main_menu_item);

                main_menu_item = new MenuItem();
                main_menu_item.Header = "Data";
                _main_menu_items.Add(MainMenuOption.DATA, main_menu_item);

                main_menu_item = new MenuItem();
                main_menu_item.Header = "Style";
                _main_menu_items.Add(MainMenuOption.STYLE, main_menu_item);

                main_menu_item = new MenuItem();
                main_menu_item.Header = "Help";
                _main_menu_items.Add(MainMenuOption.HELP, main_menu_item);

                add_repo_link();

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

                foreach (var main_menu_item in _main_menu_items)
                {
                    _content.Items.Add(main_menu_item.Value);
                }
                return _content;
            }

            public void AddSeparator(MainMenuOption main_option)
            {
                _main_menu_items[main_option].Items.Add(new Separator());
            }

            public void AddMenu(MainMenuOption main_option, MenuItem menu_item)
            {
                _main_menu_items[main_option].Items.Add(menu_item);
            }

            /*


            // -----------------

            var config_menu_item = new MenuItem();
            config_menu_item.Header = "Configuration";
            config_menu_item.Style = ColorTheme.MenuItemIconStyle();
            menu_item.Items.Add(config_menu_item);

            var sub_menu_item = new MenuItem();
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

            var send_data_menu_item = new MenuItem();
            send_data_menu_item.Header = "Send Output Data";
            send_data_menu_item.Name = _item_data[MenuItemData.DATA_SEND].Item1;
            send_data_menu_item.Click += event_menumenu_item;
            send_data_menu_item.Style = ColorTheme.MenuItemIconStyle();
            data_menu_item.Items.Add(send_data_menu_item);

            var csv_menu_item = new MenuItem();
            csv_menu_item.Header = "CSV (Comma Separated Values)";
            csv_menu_item.Style = ColorTheme.MenuItemIconStyle();
            data_menu_item.Items.Add(csv_menu_item);

            sub_menu_item = new MenuItem();
            sub_menu_item.Header = "Save";
            sub_menu_item.Name = _item_data[MenuItemData.DATA_SAVE].Item1;
            sub_menu_item.Click += event_menumenu_item;
            sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
            csv_menu_item.Items.Add(sub_menu_item);

            sub_menu_item = new MenuItem();
            sub_menu_item.Header = "Load";
            sub_menu_item.Name = _item_data[MenuItemData.DATA_LOAD].Item1;
            sub_menu_item.Click += event_menumenu_item;
            sub_menu_item.Style = ColorTheme.MenuItemIconStyle();
            csv_menu_item.Items.Add(sub_menu_item);

            // ----------------------------------------

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

            */

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _main_menu_items.Clear();
                    _main_menu_items = null;

                    _content.Items.Clear();
                    _content = null;

                    _initialized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void add_repo_link()
            {
                var hyper_link = new Hyperlink();
                hyper_link.NavigateUri = new Uri("https://github.com/IntCDC/VisuAlFroG");
                hyper_link.RequestNavigate += event_hyperlink_requestnavigate;
                hyper_link.Inlines.Add("GitHub Repository");
                hyper_link.Style = ColorTheme.HyperlinkStyle();
                var repo_menu_item = new MenuItem();
                repo_menu_item.Style = ColorTheme.MenuItemIconStyle("github.png");
                repo_menu_item.Header = hyper_link;
                AddMenu(MainMenuOption.HELP, repo_menu_item);
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
            private Dictionary<MainMenuOption, MenuItem> _main_menu_items = null;
        }
    }
}
