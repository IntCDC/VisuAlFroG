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

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Used for detached execution
            /// that use the old ID will partially fail during loading.
            /// </summary>
            public MainWindow() : this("[detached] Visualization Framework for Grasshopper (VisFroG)", false) { }


            public MainWindow(string app_name, bool soft_close)
            {
                _soft_close = soft_close;
                initialize(app_name);
                execute();
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
            public void OutputDataCallback(OutputData_Delegate putput_data_callback)
            {
                _outputdata_callback = putput_data_callback;
            }


            /// <summary>
            /// Get input data from interface (= Grasshopper).
            /// </summary>
            public void InputData()
            {


                /// TODO


            }


            /* ------------------------------------------------------------------*/
            // protected functions

            /// <summary>
            /// Soft close is used when called from interface (= Grasshopper).
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
                _timer.Start();

                /// DEBUG get C# version, see compile output
                // #error version

                // Window setup
                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;
                base.Icon = ImageHelper.ImageSourceFromFile(WorkingDirectory.Locations.Resource, "logo64.png");
                // base.Loaded += on_loaded;
                // CompositionTarget.Rendering += once_per_frame;


                // Register required external data
                _menubar.RegisterCloseCallback(this.Close);
                _menubar.RegisterContentElement(_menubar_element);

                _winmanager.RegisterContentCallbacks(_vismanager.GetContentCallbacks());
                _winmanager.RegisterContentElement(_subwindows_element);


                // Initialize managers
                bool initilized = _vismanager.Initialize();
                initilized &= _winmanager.Initialize();


                _timer.Stop();
                _initilized = initilized;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }


            public bool execute()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();

                bool executed = _vismanager.Execute();
                executed &= _winmanager.Execute();
                executed &= _menubar.Execute();

                _timer.Stop();
                return executed;
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

            private VisualizationManager _vismanager = new VisualizationManager();

            private WindowManager _winmanager = new WindowManager();
            private MenuBar _menubar = new MenuBar();

            private ReloadInterface_Delegate _reloadinterface_callback = null;
            private OutputData_Delegate _outputdata_callback = null;

            private TimeBenchmark _timer = new TimeBenchmark();
        }
    }
}
