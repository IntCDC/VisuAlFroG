using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;
using Visualizations.Management;
using System.Windows;
using Core.GUI;



/*
 * Abstract Visualization
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractVisualization : IAbstractService, IAbstractContent
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

            protected Grid ContentChild { get { return _content_child; } }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            public AbstractVisualization()
            {
                _id = UniqueID.Generate();
                _timer = new TimeBenchmark();

                /// TODO Move somewhere else to prevent calling it whenever the context menu is opened and class is constructed
                StackPanel stack = new StackPanel();
                _content_child = new Grid();
                _content_parent = new DockPanel();
                stack.Children.Add(create_menu());
                DockPanel.SetDock(stack, System.Windows.Controls.Dock.Top);
                _content_parent.Children.Add(stack);
                _content_parent.Children.Add(_content_child);
            }

            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)) ...
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            /// <exception cref="InvalidOperationException">...throw error when method of base class is called instead.</exception>
            public virtual bool Initialize()
            {
                throw new InvalidOperationException("Call Initialize() method of derived class");
            }
            /* TEMPLATE
            public override bool Initialize()
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
            /// Called to actually (re-)create the WPF content.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public abstract bool Create();
            /*
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Skipping re-creation of content");
                    return false;
                }
                if (_request_data_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return true;
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
                    /// PLACE YOUR STUFF HERE ...

                    _attached = false;
                }
                return true;
            }

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

                _request_data_callback = null;

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
            /// Visualizations need access to data of specific type.
            /// </summary>
            /// <param name="request_data_callback">Callback from data manager to request data of specific type.</param>
            public void SetRequestDataCallback(DataManager.RequestDataCallback_Delegate request_data_callback)
            {
                _request_data_callback = request_data_callback;
            }

            /// <summary>
            /// Called from DataManager if updated input data is available (dummy).
            /// => Implement in derived class: public new DataType Data()
            /// </summary>
            /// <returns>Return data as requested type, null otherwise</returns>
            public virtual object Data() { return null; }


            /* ------------------------------------------------------------------*/
            // protected functions

            /// <summary>
            /// Add new option to menu of visualization.
            /// </summary>
            protected void AddOption(MenuItem option)
            {
                option.Style = ColorTheme.MenuItemStyle();
                _options_menu.Items.Add(option);
            }

            /// <summary>
            /// Create content menu.
            /// </summary>
            /// <returns>Return the content element holding the menu.</returns>
            protected Grid create_menu()
            {
                var menu_grid = new Grid();
                menu_grid.Style = ColorTheme.ContentMenuStyle();

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
                text.Style = ColorTheme.CaptionStyle();

                var menu = new Menu();
                Grid.SetColumn(menu, 1);
                menu_grid.Children.Add(menu);
                menu.Style = ColorTheme.MenuStyle();

                _options_menu = new MenuItem();
                _options_menu.Header = "Options";
                menu.Items.Add(_options_menu);

                return menu_grid;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected string _id = UniqueID.Invalid;

            protected bool _initialized = false;
            protected bool _created = false;
            protected bool _attached = false;

            protected DataManager.RequestDataCallback_Delegate _request_data_callback = null;


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
