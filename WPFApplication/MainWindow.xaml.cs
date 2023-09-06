using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Core.Utilities;
using Core.Abstracts;
using Visualizations;
using Core.GUI;
using Visualizations.Types;
using Visualizations.Management;
using System.Windows.Input;
using Visualizations.Interaction;



/*
 * Main WPF Application
 * 
 */
namespace Frontend
{
    namespace Application
    {

        public partial class MainWindow : Window
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows to trigger reloading of the interface
            /// </summary>
            public delegate void ReloadInterface_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Ctor. Used for detached execution.
            /// </summary>
            public MainWindow() : this("[detached] Visualization Framework for Grasshopper (VisFroG)", true) { }

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="app_name"></param>
            /// <param name="detached"></param>
            public MainWindow(string app_name, bool detached = false)
            {
                /// DEBUG get C# version, see compile output (C# Language Version 7.3)
                // #error version

                _soft_close = !detached;
                _detached = detached;
                initialize(app_name);
                create();
            }

            /// <summary>
            /// Dtor.
            /// </summary>
            ~MainWindow()
            {
                _configurationservice.Terminate();
                _vismanager.Terminate();
                _winmanager.Terminate();
                _menubar.Terminate();
            }

            /// <summary>
            /// Callback to trigger reloading the interface (= Grasshopper).
            /// </summary>
            /// <param name="reload_callback">Reload callback provided by the interface.</param>
            public void SetReloadInterface(ReloadInterface_Delegate reload_callback)
            {
                _reloadinterface_callback = reload_callback;
            }

            /// <summary>
            /// Callback to pass output data to the interface (= Grasshopper).
            /// </summary>
            /// <param name="output_data_callback">callback from the DataManager to pipe new output data to the interface.</param>
            public void SetOutputDataCallback(DataManager.OutputData_Delegate output_data_callback)
            {
                _vismanager.SetOutputDataCallback(output_data_callback);
            }

            /// <summary>
            /// Get input data from interface (= Grasshopper).
            /// </summary>
            /// <param name="input_data">Reference to the input data hold by the interface.</param>
            public void UpdateInputData(ref GenericDataBranch input_data)
            {
                if (_inputdata_callback != null)
                {
                    _inputdata_callback(ref input_data);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Missing input data callback");
                }
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            /// <summary>
            /// Soft close is used when called from interface (= Grasshopper), see CTOR
            /// Only change visibility and abort closing for being able to restore closed window
            /// </summary>
            /// <param name="cancel_args">Cancel event arguments</param>
            protected override void OnClosing(CancelEventArgs cancel_args)
            {
                if (_soft_close)
                {
                    base.Visibility = Visibility.Hidden;
                    cancel_args.Cancel = true;
                }
                else
                {
                    base.OnClosing(cancel_args);
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Initialize the main WPF window, the services and the managers...
            /// </summary>
            /// <param name="app_name">The name of the WPF application.</param>
            /// <returns>True on successful initialization, false otherwise.</returns>
            public bool initialize(string app_name)
            {
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Warn, "Initialization should only be called once");
                    return false;
                }
                _timer = new TimeBenchmark();
                _timer.Start();
                /// Cursor = Cursors.Wait;

                // Window setup
                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;
                base.Icon = ImageHelper.ImageSourceFromFile(WorkingDirectory.Locations.Resource, "logo64.png");
                // base.Loaded += on_loaded;
                // CompositionTarget.Rendering += once_per_frame;

                // Initialize managers and services
                _configurationservice = new ConfigurationService();
                _vismanager = new VisualizationManager();
                _winmanager = new WindowManager();
                _menubar = new MenuBar();

                bool initilized = _configurationservice.Initialize();
                initilized &= _vismanager.Initialize();
                initilized &= _winmanager.Initialize(_vismanager.GetContentCallbacks());
                initilized &= _menubar.Initialize(this.Close, _configurationservice.Save, _configurationservice.Load);

                // Register additional callbacks
                ///  Do not reorder since applying configuration might be order dependent!
                _configurationservice.RegisterConfiguration(_vismanager.Name, _vismanager.CollectConfigurations, _vismanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_winmanager.Name, _winmanager.CollectConfigurations, _winmanager.ApplyConfigurations);

                // Get callbacks
                _inputdata_callback = _vismanager.GetInputDataCallback();

                /// Cursor = Cursors.Arrow;
                _timer.Stop();
                _initilized = initilized;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initilized;
            }

            /// <summary>
            /// Create the WPF content of the application.
            /// </summary>
            /// <returns>True on successful creation of the content, false otherwise</returns>
            public bool create()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();
                /// Cursor = Cursors.Wait;

                var menubar_content = _menubar.Attach();
                if (menubar_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid menu bar content");
                }
                _menubar_element.Children.Add(menubar_content);

                var winmanager_content = _winmanager.Attach();
                if (winmanager_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid menu bar content");
                }
                _subwindows_element.Children.Add(winmanager_content);
                _winmanager.CreateDefault();


                /// DEBUG Load sample data in detached mode ...
                if (_detached)
                {
                    var sample_data = new GenericDataBranch();
                    var data_branch = new GenericDataBranch();
                    sample_data.AddBranch(data_branch);
                    var generator = new Random();
                    for (int i = 0; i < 20; i++) {
                        var value = generator.Next(2, 25);
                        var data_leaf = new GenericDataEntry();
                        data_leaf.AddValue((double)value);
                        data_leaf.MetaData.Index = i;
                        data_branch.AddLeaf(data_leaf);
                    }
                    UpdateInputData(ref sample_data);
                }

                /// Cursor = Cursors.Arrow;
                _timer.Stop();
                return true;
            }

            /// <summary>
            /// Callback additionally invoked on loading of main window.
            /// </summary>
            /// <param name="sender">Sender object.</param>
            /// <param name="routedEventArgs">Routed event arguments.</param>
            private void on_loaded(object sender, RoutedEventArgs routedEventArgs)
            {
                // so far unused ...
            }

            /// <summary>
            /// Callback invoked once per frame.
            /// </summary>
            /// <param name="sender">Sender object.</param>
            /// <param name="args">Event arguments.</param>
            private void once_per_frame(object sender, EventArgs args)
            {
                // so far unused ...
            }


            /* ------------------------------------------------------------------*/
            // local variables

            private bool _initilized = false;
            private bool _soft_close = false;
            private bool _detached = false;

            private ConfigurationService _configurationservice = null;
            private VisualizationManager _vismanager = null;
            private WindowManager _winmanager = null;
            private MenuBar _menubar = null;

            private ReloadInterface_Delegate _reloadinterface_callback = null;
            private DataManager.OutputData_Delegate _outputdata_callback = null;
            private DataManager.InputData_Delegate _inputdata_callback = null;

            /// DEBUG
            private TimeBenchmark _timer = null;
        }
    }
}
