using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Documents;
using System;
using System.Windows.Navigation;
using System.Collections.Generic;



/*
 * Abstract Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public abstract class AbstractMenuBar<EnumOptionType> where EnumOptionType : Enum
        {
            /* ------------------------------------------------------------------*/
            #region public delegate

            public delegate bool MenuCallback_Delegate();

            #endregion

            /* ------------------------------------------------------------------*/
            #region static functions

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

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public virtual bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _initialized = false;

                _content = new Menu();
                _content.Style = ColorTheme.MainMenuBarStyle();

                _main_menu_items = new Dictionary<EnumOptionType, MenuItem>();

                _initialized = true;
                return _initialized;
            }

            public Control AttachUI()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                _content.Items.Clear();
                foreach (var main_menu_item in _main_menu_items)
                {
                    _content.Items.Add(main_menu_item.Value);
                }
                return _content;
            }

            public void AddSeparator(EnumOptionType main_option)
            {
                _main_menu_items[main_option].Items.Add(new Separator());
            }

            public void AddMenu(EnumOptionType main_option, MenuItem menu_item)
            {
                if (_main_menu_items.ContainsKey(main_option))
                {
                    _main_menu_items[main_option].Items.Add(menu_item);
                    _main_menu_items[main_option].IsEnabled = true;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Could not find main menu option: '" + main_option.ToString() + "'");
                }
            }

            public void Clear(EnumOptionType main_option)
            {
                if (_main_menu_items.ContainsKey(main_option))
                {
                    _main_menu_items[main_option].Items.Clear();
                }
            }

            public bool Terminate()
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

            public MenuItem FindMenuItemByName(string name)
            {
                foreach (var item in _content.Items)
                {
                    var menu_item = item as MenuItem;
                    if (menu_item != null)
                    {
                        var returned_item = recursive_find_menu_item_by_name(menu_item, name);
                        if (returned_item != null)
                        {
                            return returned_item;
                        }
                    }
                }
                Log.Default.Msg(Log.Level.Error, "Could not find menu item: '" + name + "'");
                return null;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected void add_main_menu(string name, EnumOptionType enum_option)
            {
                var main_menu_item = new MenuItem();
                main_menu_item.Header = name;
                main_menu_item.IsEnabled = false;
                _main_menu_items.Add(enum_option, main_menu_item);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected bool _initialized = false;

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private MenuItem recursive_find_menu_item_by_name(MenuItem parent, string name)
            {
                if (parent.Name == name)
                {
                    return parent;
                }
                foreach (var item in parent.Items)
                {
                    var menu_item = item as MenuItem;
                    if (menu_item != null)
                    {
                        var returned_item = recursive_find_menu_item_by_name(menu_item, name);
                        if (returned_item != null)
                        {
                            return returned_item;
                        }
                    }
                }
                return null;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Menu _content = null;
            private Dictionary<EnumOptionType, MenuItem> _main_menu_items = null;

            #endregion
        }
    }
}
