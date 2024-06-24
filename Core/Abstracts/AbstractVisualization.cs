using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows;
using Core.GUI;
using Core.Data;
using System.Windows.Documents;
using System.Windows.Data;
using System.Globalization;



/*
 * Abstract Visualization
 * 
 * Initialize -> Create -> (Update) -> Attach
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractVisualization : IAbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            #region public delegate

            public delegate void AttachDataMenu_Delegate(List<System.Windows.Controls.MenuItem> menu_item);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public string _ID { get; set; }
                public string _Type { get; set; }
                public string _Name { get; set; }
                /// TODO Add additional configuration information that should be saved here...
                /// and adjust de-/serialization methods in ContenManager accordingly
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public string _Name { get { return _edit_content_caption.Text;  } set { _edit_content_caption.Text = value; } }
            public abstract string _TypeName { get; }
            public abstract bool _MultipleInstances { get; }
            public abstract List<Type> _DependingServices { get; }
            public bool _Attached { get; protected set; } = false;
            public string _ID { get; set; } = UniqueID.InvalidString;
            public int _DataUID { get; set; } = UniqueID.InvalidInt;

            public abstract Type _RequiredDataType { get; }
            public DataManager.GetDataCallback_Delegate _RequestDataCallback { get; private set; }
            public DataManager.GetDataMenuCallback_Delegate _RequestMenuCallback { get; private set; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            public AbstractVisualization()
            {
                _timer = new TimeBenchmark();
            }

            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)) ...
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            /// <exception cref="InvalidOperationException">...throw error when method of base class is called instead.</exception>
            public virtual bool Initialize(DataManager.GetDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _initialized = false;

                /* TEMP
                if ((request_data_callback == null) || (request_menu_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                    return false;
                }
                */

                _ID = UniqueID.GenerateString();

                _RequestDataCallback = request_data_callback;
                _RequestMenuCallback = request_menu_callback;

                _menu = new MenubarContent();
                _initialized = _menu.Initialize();

                StackPanel stack = new StackPanel();
                stack.Children.Add(create_menu());
                DockPanel.SetDock(stack, System.Windows.Controls.Dock.Top);

                _content_child = new Grid();

                _content_parent = new DockPanel();
                _content_parent.Children.Add(stack);
                _content_parent.Children.Add(_content_child);

                return _initialized;
            }

            /* TEMPLATE
            {
               _timer.Start();

               if (base.Initialize(request_callback))
               {

                   /// PLACE YOUR STUFF HERE ...

                   _initialized = true;
                   if (_initialized)
                   {
                       Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                   }
               }


               _timer.Stop();
               return _initialized;
            }
            */

            /// <summary>
            /// Called to actually (re-)create the WPF content on any changes
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public abstract bool Create();
            /* TEMPLATE
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

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return _created;
            }
            */

            /// <summary>
            /// Called when content element is being attached to a parent element.
            /// </summary>
            /// <returns>The WPF control element holding the content.</returns>
            public virtual System.Windows.Controls.Panel Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                _Attached = true;
                return _content_parent;
            }
            /* TEMPLATE
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                /// PLACE YOUR STUFF HERE ...

                AttachChildContent(_content);
                return base.Attach();
            }
            */

            /// <summary>
            /// Called when content has been detached. 
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public virtual bool Detach()
            {
                if (!_Attached)
                {
                    if (_content_child != null)
                    {
                        _content_child.Children.Clear();
                    }
                    _Attached = false;
                }
                return true;
            }
            /* TEMPLATE
            {
                if (!_attached)
                {
                    /// PLACE YOUR STUFF HERE ...

                }
                return base.Detach();
            }
            */

            /// <summary>
            /// Called when content should be terminated. Should implement counterpart to Initialize().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public virtual bool Terminate()
            {
                _ID = UniqueID.InvalidString;

                _created = false;
                _initialized = false;
                _Attached = false;

                _content_parent = null;
                _content_child = null;
                _menu = null;
                _timer = null;

                _RequestDataCallback = null;
                _RequestMenuCallback = null;

                return true;
            }
            /* TEMPLATE
            {
                if (_initialized)
                {
                    /// PLACE YOUR STUFF HERE ...
                    
                    _initialized = false;
                }
                return base.Terminate();
            }
            */

            /// <summary>
            /// Called when (partially: new_data=false) updated data is available
            /// </summary>
            /// <param name="new_data">True if new data is available, false if existing data has been updated.</param>
            public abstract void Update(bool new_data);

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected void attach_child_content(UIElement control)
            {
                _content_child.Children.Add(control);
            }

            /// </summary>
            /// <param name="data_parent"></param>
            /// <returns></returns>
            protected virtual bool apply_data<DataParentType>(out DataParentType data_parent)
            {
                data_parent = default(DataParentType);

                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                try
                {
                    // This is the place where the visualization connects to the actual data residing within the data manager
                    var data = (DataParentType)_RequestDataCallback(_DataUID);
                    if (data != null)
                    {
                        data_parent = data;

                        return true;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                /// Log.Default.Msg(Log.Level.Error, "No data for: " + typeof(DataParentType).FullName);
                return false;
            }

            protected bool attach_data_menu()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                // Clear previous menu items 
                _menu.Clear(MenubarContent.PredefinedMenuOption.DATA);
                // Request all menu items available for the data
                var data_menu_items = _RequestMenuCallback(_DataUID);
                foreach (var menu_item in data_menu_items)
                {
                    // Add additional callback for updating the visualization after the data has been modified via the DATA menu
                    menu_item.Click += (object sender, RoutedEventArgs e) =>
                    {
                        var sender_content = sender as System.Windows.Controls.MenuItem;
                        if (sender_content == null)
                        {
                            return;
                        }
                        Update(true);
                    };
                    _menu.AddMenu(MenubarContent.PredefinedMenuOption.DATA, menu_item);
                }

                return true;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected bool _initialized = false;
            protected bool _created = false;

            protected MenubarContent _menu = null;

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            /// <summary>
            /// Create content menu.
            /// </summary>
            /// <returns>Return the content element holding the menu.</returns>
            private Grid create_menu()
            {
                // Set global menu items before creating menu

                _edit_content_caption.Text = _TypeName;
                _edit_content_caption.Width = 100.0;
                _edit_content_caption.Focusable = true;
                _edit_content_caption.Focus();

                var _menu_rename = MenubarMain.GetDefaultMenuItem("Rename");
                _menu_rename.Items.Add(_edit_content_caption);
                _menu.AddMenu(MenubarContent.PredefinedMenuOption.CONTENT, _menu_rename);

                var content_caption = new TextBlock();
                content_caption.Style = ColorTheme.ContentCaptionStyle();

                // Bind caption text block to editable text box
                var caption_binding = new Binding("Text");
                caption_binding.Mode = BindingMode.OneWay;
                caption_binding.Source = _edit_content_caption;
                content_caption.SetBinding(TextBlock.TextProperty, caption_binding);


                /// _menu.AddMenu(ContentMenuBar.PredefinedMenuOption.CONTENT, MainMenuBar.GetDefaultMenuItem("Filter", filter_content_click));


                var menu_grid = new Grid();
                menu_grid.Height = 20.0;
                menu_grid.SetResourceReference(Grid.BackgroundProperty, "Brush_MenuBarBackground");

                var column_label = new ColumnDefinition();
                column_label.Width = new GridLength(0.0, GridUnitType.Auto);
                menu_grid.ColumnDefinitions.Add(column_label);
                var column_menu = new ColumnDefinition();
                column_menu.Width = new GridLength(1.0, GridUnitType.Star);
                menu_grid.ColumnDefinitions.Add(column_menu);

                Grid.SetColumn(content_caption, 0);
                menu_grid.Children.Add(content_caption);

                var menu = _menu.Attach();
                Grid.SetColumn(menu, 1);
                menu.Style = ColorTheme.ContentMenuBarStyle();
                menu_grid.Children.Add(menu);

                return menu_grid;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private DockPanel _content_parent = null;
            private Grid _content_child = null;
            private TextBox _edit_content_caption = new TextBox();


            /// DEBUG
            protected TimeBenchmark _timer = null;

            #endregion
        }
    }
}
