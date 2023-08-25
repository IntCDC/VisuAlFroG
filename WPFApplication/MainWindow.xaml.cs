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



/*
 * Main WPF Application
 * 
 * 
 * Interaction logic for MainWindow.xaml
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
            /// Function provided by the interface (= Grasshopper) which allows to trigger relaoding of the interface
            /// </summary>
            public delegate void ReloadInterface_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Used for detached execution
            /// that use the old ID will partially fail during loading.
            /// </summary>
            public MainWindow() : this("[detached] Visualization Framework for Grasshopper (VisFroG)", true) { }

            public MainWindow(string app_name, bool detached = false)
            {
                /// DEBUG get C# version, see compile output (C# Language Version 7.3)
                // #error version

                _soft_close = !detached;
                _detached = detached;
                initialize(app_name);
                create();
            }


            ~MainWindow()
            {
                _vismanager.Terminate();
                _winmanager.Terminate();
                _menubar.Terminate();
            }


            /// <summary>
            /// Callback to trigger reloading the interface (= Grasshopper).
            /// </summary>
            public void ReloadInterfaceCallback(ReloadInterface_Delegate reload_callback)
            {
                _reloadinterface_callback = reload_callback;
            }


            /// <summary>
            /// Callback to pass output data to the interface (= Grasshopper).
            /// </summary>
            public void OutputDataCallback(DataManager.OutputData_Delegate output_data_callback)
            {
                _vismanager.RegisterOutputDataCallback(output_data_callback);
            }


            /// <summary>
            /// Get input data from interface (= Grasshopper).
            /// </summary>
            public void InputData(ref XYData_Type input_data)
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

            public bool initialize(string app_name)
            {
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Warn, "Initialization should only be called once");
                    return false;
                }
                _timer = new TimeBenchmark();
                _timer.Start();


                // Window setup
                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;
                base.Icon = ImageHelper.ImageSourceFromFile(WorkingDirectory.Locations.Resource, "logo64.png");
                // base.Loaded += on_loaded;
                // CompositionTarget.Rendering += once_per_frame;


                // Initialize managers
                _vismanager = new VisualizationManager();
                _winmanager = new WindowManager();
                _menubar = new MenuBar();
                bool initilized = _vismanager.Initialize();
                initilized &= _winmanager.Initialize(_vismanager.GetContentCallbacks());
                initilized &= _menubar.Initialize(this.Close);

                // Get callbacks
                _inputdata_callback = _vismanager.GetInputDataCallback();


                _timer.Stop();
                _initilized = initilized;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }


            public bool create()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();

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


                /// DEBUG Load sample data in detached mode ...
                if (_detached)
                {
                    var sample_data = new XYData_Type();
                    sample_data.Add(new List<double>() { 1.0, 2.0, 5.0, 9.0, 6.0, 8.0, 2.0, 3.0, 3.0, 1.0, 5.0, 4.0, 6.0, 8.0, 7.0, 7.0, 2.0 });
                    InputData(ref sample_data);
                }


                _timer.Stop();
                return true;
            }


            /// <summary>
            /// Callback additionally invoked on loading of main window.
            /// </summary>
            private void on_loaded(object sender, RoutedEventArgs routedEventArgs)
            {
                // so far unused ...
            }


            /// <summary>
            /// Callback invoked once per frame
            /// </summary>
            private void once_per_frame(object sender, EventArgs args)
            {
                // so far unused ...
            }


            /* ------------------------------------------------------------------*/
            // local variables

            private bool _initilized = false;
            private bool _soft_close = false;
            private bool _detached = false;

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
