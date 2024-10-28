using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows;
using Core.GUI;
using Core.Data;
using System.Runtime.Remoting.Contexts;



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
            #region public static properties

            public static string TypeString_LogConsole { get; }    = "Visualizations.WPF_LogConsole";
            public static string TypeString_FilterEditor { get; }  = "Visualizations.WPF_FilterEditor";
            public static string TypeString_DataViewer { get; }    = "Visualizations.WPF_DataViewer";
            public static string TypeString_SciChartLines { get; } = "Visualizations.SciChart_Lines";
            public static string TypeString_SciChartPCP { get; }   = "Visualizations.SciChart_ParallelCoordinatesPlot";

            #endregion

            /* ------------------------------------------------------------------*/
            #region public delegate

            public delegate void AttachWindowMenu_Delegate(MenubarWindow menubar);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public int UID { get; set; }
                public string Type { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public int _UID { get; } = UniqueID.GenerateInt();
            public int _DataUID { get; set; } = UniqueID.InvalidInt;
            public abstract string _Name { get; }
            public abstract bool _MultipleInstances { get; }
            public abstract List<Type> _DependingServices { get; }

            public abstract Type _RequiredDataType { get; }
            public DataManager.GetSpecificDataCallback_Delegate _RequestDataCallback { get; private set; }
            public DataManager.GetDataMenuCallback_Delegate _RequestMenuCallback { get; private set; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            public AbstractVisualization(int uid)
            {
                if (uid != UniqueID.InvalidInt)
                {
                    _UID = uid;
                }
                _timer = new TimeBenchmark();
            }


            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)) ...
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            /// <exception cref="InvalidOperationException">...throw error when method of base class is called instead.</exception>
            public virtual bool Initialize(DataManager.GetSpecificDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }

                if ((request_data_callback == null) || (request_menu_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                    return false;
                }
                _RequestDataCallback = request_data_callback;
                _RequestMenuCallback = request_menu_callback;

                _content = new Grid();
                _initialized = true;

                return _initialized;
            }

            /* TEMPLATE
            {
               _timer.Start();

               if (base.Initialize(request_data_callback, request_menu_callback))
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
            public abstract bool CreateUI();
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
            public virtual UIElement GetUI()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }
                return _content;
            }

            /// <summary>
            /// Called when content should be terminated. Should implement counterpart to Initialize().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public virtual bool Terminate()
            {
                _created = false;
                _initialized = false;

                _content.Children.Clear();
                _content = null;

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

            /// <summary>
            /// Called when menu of content should be attached.
            /// </summary>
            /// <param name="menubar"></param>
            public virtual void AttachMenu(MenubarWindow menubar)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }

                // Request all menu items available for the data
                if (_DataUID != UniqueID.InvalidInt)
                {
                    var data_menu_items = _RequestMenuCallback(_DataUID);
                    foreach (var menu_control in data_menu_items)
                    {
                        MenuItem menu_item = menu_control as MenuItem;
                        if (menu_item != null)
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
                            menubar.AddMenu(MenubarWindow.PredefinedMenuOption.DATA, menu_item);
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected void attach_child_content(UIElement control)
            {
                _content.Children.Add(control);
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

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected bool _initialized = false;
            protected bool _created = false;

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _content = null;

            /// DEBUG
            protected TimeBenchmark _timer = null;

            #endregion
        }
    }
}
