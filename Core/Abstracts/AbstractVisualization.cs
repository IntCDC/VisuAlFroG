using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows;
using Core.Abstracts;
using Core.GUI;
using Core.Data;
using System.Runtime.InteropServices.WindowsRuntime;



/*
 * Abstract Visualization
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractVisualization : IAbstractContent
        {

            /* ------------------------------------------------------------------*/
            // public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public string _ID { get; set; }
                public string _Type { get; set; }
                /// TODO Add additional configuration information that should be saved here...
            }


            /* ------------------------------------------------------------------*/
            // public properties

            public abstract string _Name { get; }
            public abstract bool _MultipleInstances { get; }
            public abstract List<Type> _DependingServices { get; }
            public bool _Attached { get; protected set; } = false;
            public string _ID { get; set; } = UniqueID.InvalidString;
            public int _DataUID { get; set; } = UniqueID.InvalidInt;

            public abstract Type _RequiredDataType { get; }
            public DataManager.GetDataCallback_Delegate _RequestDataCallback { get; private set; }


            /* ------------------------------------------------------------------*/
            // public functions

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
            public virtual bool Initialize(DataManager.GetDataCallback_Delegate request_callback)
            {
                _ID = UniqueID.GenerateString();

                StackPanel stack = new StackPanel();
                stack.Children.Add(create_menu());
                DockPanel.SetDock(stack, System.Windows.Controls.Dock.Top);

                _content_child = new Grid();

                _content_parent = new DockPanel();
                _content_parent.Children.Add(stack);
                _content_parent.Children.Add(_content_child);

                _RequestDataCallback = request_callback;

                return true;
            }
            /* TEMPLATE
            {
                if (_initialized)
                {
                    Terminate();
                }
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

                _options_menu = null;
                _data_menu = null;

                _timer = null;

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


            /* ------------------------------------------------------------------*/
            // protected functions

            /// </summary>
            /// <param name="data_parent"></param>
            /// <returns></returns>
            protected virtual bool GetData<DataParentType>(out DataParentType data_parent)
            {
                data_parent = default(DataParentType);

                if (_RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }

                var data = (DataParentType)_RequestDataCallback(_DataUID);
                if (data != null)
                {
                    data_parent = data;
                    return true;
                }
                /// Log.Default.Msg(Log.Level.Error, "No data for: " + typeof(DataParentType).FullName);
                return false;
            }

            protected void AttachChildContent(UIElement control)
            {
                _content_child.Children.Add(control);
            }

            /// <summary>
            /// Add new option to 'options' menu of visualization.
            /// </summary>
            protected void AddOptionMenu(MenuItem option)
            {
                option.Style = ColorTheme.MenuItemIconStyle();
                _options_menu.Items.Add(option);
                _options_menu.IsEnabled = true;
            }

            /// <summary>
            /// Add new option to 'data' menu of visualization.
            /// </summary>
            protected void AddDataMenu(MenuItem option)
            {
                option.Style = ColorTheme.MenuItemIconStyle();
                _data_menu.Items.Add(option);
                _data_menu.IsEnabled = true;
            }

            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _initialized = false;
            protected bool _created = false;


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Create content menu.
            /// </summary>
            /// <returns>Return the content element holding the menu.</returns>
            private Grid create_menu()
            {
                var menu_grid = new Grid();
                menu_grid.Height = 20.0;
                menu_grid.SetResourceReference(Grid.BackgroundProperty, "Brush_MenuBarBackground");

                var column_label = new ColumnDefinition();
                column_label.Width = new GridLength(0.0, GridUnitType.Auto);
                menu_grid.ColumnDefinitions.Add(column_label);
                var column_menu = new ColumnDefinition();
                column_menu.Width = new GridLength(1.0, GridUnitType.Star);
                menu_grid.ColumnDefinitions.Add(column_menu);

                var text = new TextBlock();
                Grid.SetColumn(text, 0);
                menu_grid.Children.Add(text);
                text.Text = _Name;
                text.Style = ColorTheme.ContentCaptionStyle();

                var menu = new Menu();
                Grid.SetColumn(menu, 1);
                menu_grid.Children.Add(menu);
                menu.Style = ColorTheme.ContentMenuStyle();

                _options_menu = new MenuItem();
                _options_menu.Header = "Options";
                _options_menu.IsEnabled = false;
                menu.Items.Add(_options_menu);

                _data_menu = new MenuItem();
                _data_menu.Header = "Data";
                _data_menu.IsEnabled = false;
                menu.Items.Add(_data_menu);

                return menu_grid;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private DockPanel _content_parent = null;
            private Grid _content_child = null;

            private MenuItem _options_menu = null;
            private MenuItem _data_menu = null;

            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
