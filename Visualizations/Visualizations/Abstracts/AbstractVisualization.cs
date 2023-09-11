using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;
using System.Windows;
using Core.GUI;
using Visualizations.Data;



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


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            public AbstractVisualization()
            {
                _id = UniqueID.Generate();
                _timer = new TimeBenchmark();

                /// TODO Move somewhere else to prevent calling it whenever the context menu is opened
                StackPanel stack = new StackPanel();
                _content = new Grid();
                _content_parent = new DockPanel();
                stack.Children.Add(create_menu());
                DockPanel.SetDock(stack, System.Windows.Controls.Dock.Top);
                _content_parent.Children.Add(stack);
                _content_parent.Children.Add(_content);
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
            /// Called to actually (re-)create the WPF content on any changes
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public abstract bool ReCreate();
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
                    _content.Children.Clear();
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
                _content = null;

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
            /// TODO Called when updated data is available
            /// </summary>
            /// <param name="new_data">True if new data is available, false if existing data has been updated.</param>
            public abstract void UpdateCallback(bool new_data);

            /// <summary>
            /// TODO Call when visualization needs the actual data
            /// </summary>
            public void RequestCallback(DataManager.RequestCallback_Delegate request_callback)
            {
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Required to be called prior to initialization");
                }
                _request_callback = request_callback;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            /// <summary>
            /// Add new option to menu of visualization.
            /// </summary>
            protected void AddOption(MenuItem option)
            {
                option.Style = ColorTheme.MenuItemStyle();
                _options_menu.Items.Add(option);
                _options_menu.IsEnabled = true;
            }

            protected void AttachChildContent(Control control)
            {
                _content.Children.Add(control);
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
                _options_menu.IsEnabled = false;
                menu.Items.Add(_options_menu);

                return menu_grid;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private DockPanel _content_parent = null;
            private MenuItem _options_menu = null;
            private Grid _content = null;

            protected DataManager.RequestCallback_Delegate _request_callback;

            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
