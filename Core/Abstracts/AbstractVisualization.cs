using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows;
using Core.Abstracts;
using Core.GUI;
using Core.Data;



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
                public string ID { get; set; }
                public string Type { get; set; }
                /// TODO Add additional configuration information that should be saved here...
            }


            /* ------------------------------------------------------------------*/
            // public properties

            public abstract string Name { get; }
            public abstract bool MultipleInstances { get; }
            public abstract List<Type> DependingServices { get; }
            public bool IsAttached { get { return _attached; } }
            public string ID { get { return _id; } set { _id = value; } }
            public DataManager.RequestCallback_Delegate RequestDataCallback { get; set; }

            /* ------------------------------------------------------------------*/
            // public functions


            /// <summary>
            ///  Get the type of the required data variety
            ///  TODO To be implemented by specific visualization
            /// </summary>
            /// <returns>Returns the data type</returns>
            public abstract Type GetDataType();


            /// <summary>
            /// Ctor.
            /// </summary>
            public AbstractVisualization()
            {
                _id = UniqueID.Generate();
                _timer = new TimeBenchmark();

                /// TODO Move somewhere else to prevent calling it whenever the context menu is opened
                
                StackPanel stack = new StackPanel();
                stack.Children.Add(create_menu());
                DockPanel.SetDock(stack, System.Windows.Controls.Dock.Top);

                _content_child = new Grid();

                _content_parent = new DockPanel();
                _content_parent.Children.Add(stack);
                _content_parent.Children.Add(_content_child);
            }

            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)) ...
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            /// <exception cref="InvalidOperationException">...throw error when method of base class is called instead.</exception>
            public virtual bool Initialize(DataManager.RequestCallback_Delegate request_callback)
            {
                throw new InvalidOperationException("Call Initialize() method of derived class");
            }
            /* TEMPLATE
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();


                /// PLACE YOUR STUFF HERE ...

                /// ! REQUIRED:
                _content.Name = ID;


                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initialized;
            }
            */

            /// <summary>
            /// Called to actually (re-)create the WPF content on any changes
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public abstract bool ReCreate();

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

                _attached = true;
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
                if (!_attached)
                {
                    if (_content_child != null)
                    {
                        _content_child.Children.Clear();
                    }
                    _attached = false;
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
                _id = UniqueID.Invalid;

                _initialized = false;
                _created = false;
                _attached = false;

                _content_parent = null;
                _options_menu = null;
                _content_child = null;

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
            /// TODO One variant of the following SetData method must be implemented
            /// </summary>
            public virtual bool GetData(object data_parent)
            {
                throw new InvalidOperationException("Call SetData() method of derived class");
            }
            public virtual bool GetData(ref GenericDataStructure data_parent)
            {
                throw new InvalidOperationException("Call SetData() method of derived class");
            }

            /// <summary>
            /// TODO Called when updated data is available
            /// </summary>
            /// <param name="new_data">True if new data is available, false if existing data has been updated.</param>
            public abstract void UpdateCallback(bool new_data);


            /* ------------------------------------------------------------------*/
            // protected functions

            /// <summary>
            /// Add new option to menu of visualization.
            /// </summary>
            protected void AddOption(MenuItem option)
            {
                option.Style = ColorTheme.MenuItemIconStyle();
                _options_menu.Items.Add(option);
                _options_menu.IsEnabled = true;
            }

            protected void AttachChildContent(Control control)
            {
                _content_child.Children.Add(control);
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected string _id = UniqueID.Invalid;

            protected bool _initialized = false;
            protected bool _created = false;
            protected bool _attached = false;


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
                text.Text = Name;
                text.Style = ColorTheme.ContentCaptionStyle();

                var menu = new Menu();
                Grid.SetColumn(menu, 1);
                menu_grid.Children.Add(menu);
                menu.Style = ColorTheme.ContentMenuStyle();

                _options_menu = new MenuItem();
                _options_menu.Header = "Options";
                _options_menu.IsEnabled = false;
                menu.Items.Add(_options_menu);

                return menu_grid;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private DockPanel _content_parent = null;
            private MenuItem _options_menu = null;
            private Grid _content_child = null;

            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
