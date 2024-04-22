using System;
using System.ComponentModel;
using System.Windows;
using Core.Utilities;
using Core.GUI;
using Core.Data;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;



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
            public MainWindow() : this("[detached] Visual Analytics Framework for Grasshopper (VisuAlFroG)", "", true) { }

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="app_name"></param>
            /// <param name="detached"></param>
            public MainWindow(string app_name, string configuration_file = "", bool detached = false)
            {
                /// DEBUG get C# version, see compile output (C# Language Version 7.3)
                // #error version

                _soft_close = !detached;
                _detached = detached;
                initialize(app_name);
                create(configuration_file);
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
                _basemanager.SetOutputDataCallback(output_data_callback);
            }

            /// <summary>
            /// Get input data from interface (= Grasshopper).
            /// </summary>
            /// <param name="input_data">Reference to the input data hold by the interface.</param>
            public void UpdateInputData(ref GenericDataStructure input_data)
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
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Warn, "Initialization should only be called once");
                    return false;
                }
                _timer = new TimeBenchmark();
                _timer.Start();
                /// Cursor = Cursors.Wait;

                // Explicitly disable debug messages
                Log.Default.DisableDebug = false;

                // Window setup
                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;
                base.Icon = ImageLoader.ImageSourceFromFile(ResourcePaths.Locations.LogoIcons, "logo64.png");
                // base.Loaded += on_loaded;
                // CompositionTarget.Rendering += once_per_frame;

                // Read cmd line arguments
                bool initialized = cmdline_arguments();

                // Initialize managers and services
                _configurationservice = new ConfigurationService();
                _basemanager = new Visualizations.BaseManager();
                _winmanager = new WindowManager();
                _colortheme = new ColorTheme();
                _menubar = new MenuBar();

                initialized &= _configurationservice.Initialize();
                initialized &= _basemanager.Initialize();
                initialized &= _winmanager.Initialize(_basemanager.GetContentCallbacks());
                initialized &= _colortheme.Initialize(App.Current.Resources, _menubar.MarkColorTheme);
                initialized &= _menubar.Initialize(this.Close, _colortheme.SetColorStyle, _configurationservice.Save, _configurationservice.Load);

                // Register additional callbacks
                ///  Do not reorder since applying configuration might be order dependent!
                _configurationservice.RegisterConfiguration(_basemanager.Name, _basemanager.CollectConfigurations, _basemanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_winmanager.Name, _winmanager.CollectConfigurations, _winmanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_colortheme.Name, _colortheme.CollectConfigurations, _colortheme.ApplyConfigurations);

                // Get callbacks
                _inputdata_callback = _basemanager.GetInputDataCallback();


                /// Cursor = Cursors.Arrow;
                _timer.Stop();
                _initialized = initialized;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            /// <summary>
            /// Create the WPF content of the application.
            /// </summary>
            /// <param name="configuration_file">The configuration file loaded on startup.</param> 
            /// <returns>True on successful creation of the content, false otherwise</returns>
            public bool create(string configuration_file)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();
                ///Cursor = Cursors.Wait;


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


                // Load startup configuration from file
                string config_file = configuration_file;
                if (_cmdline_args.ContainsKey("config"))
                {
                    // Command line overwrites ctor parameter
                    config_file = _cmdline_args["config"];
                }
                if (config_file != "")
                {
                    _configurationservice.Load(config_file);
                }
                else
                {
                    _winmanager.CreateDefault();
                }


                /// DEBUG Load sample data in detached mode ...
                if (_detached)
                {
                    var generator = new Random();
                    var sample_data = new GenericDataStructure();

                    int value_index = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        var data_branch = new GenericDataStructure();
                        for (int j = 0; j < 25; j++)
                        {
                            var value = generator.Next(0, 25);
                            var data_leaf = new GenericDataEntry();
                            data_leaf.AddValue((double)value);

                            data_leaf.MetaData.Index = value_index;
                            value_index++;

                            data_branch.AddEntry(data_leaf);
                        }
                        sample_data.AddBranch(data_branch);
                    }

                    UpdateInputData(ref sample_data);
                }


                ///Cursor = Cursors.Arrow;
                _timer.Stop();
                return true;
            }

            public bool cmdline_arguments()
            {

                // Parse command line arguments
                string[] args = Environment.GetCommandLineArgs();
                for (int index = 1; index < args.Length; index += 2)
                {
                    /*
                     Supported command line arguments:
                     -h --help     Help text on valid commandline parameters
                     -c --config   Configuration file path
                     */

                    // TODO Proper check!
                    bool valid_arguments = true;

                    Log.Default.Msg(Log.Level.Warn, "Unexpected command line argument(s).");

                    string arg = args[index].Replace("-", "");
                    _cmdline_args.Add(arg, args[index + 1]);

                }


                return true;
            }

            void print_usage_inforamtion()
            {
                Log.Default.Msg(Log.Level.Info,
                    "VisualFroG.exe [[OPTION] | [OPTION] [PARAMETER]]...\n " +
                    "Options: " +
                    "    -c, -config CONFIGURATION_FILE_PATH    Provide the absolute or relative file path to a configuration file that should be loaded on startup." +
                    "    -h, --help                             Print this message."
                    );
            }

            /// <summary>
            /// Callback additionally invoked on loading of main window.
            /// </summary>
            /// <param name="sender">Sender object.</param>
            /// <param name="routedEventArgs">Routed event arguments.</param>
            private void event_loaded(object sender, RoutedEventArgs routedEventArgs)
            {
                // so far unused ...
            }

            /// <summary>
            /// Callback invoked once per frame.
            /// </summary>
            /// <param name="sender">Sender object.</param>
            /// <param name="args">Event arguments.</param>
            private void event_once_per_frame(object sender, EventArgs args)
            {
                // so far unused ...
            }


            /* ------------------------------------------------------------------*/
            // local variables

            private Dictionary<string, string> _cmdline_args = new Dictionary<string, string>();

            private bool _initialized = false;
            private bool _soft_close = false;
            private bool _detached = false;

            private ConfigurationService _configurationservice = null;
            private Visualizations.BaseManager _basemanager = null;
            private WindowManager _winmanager = null;
            private ColorTheme _colortheme = null;
            private MenuBar _menubar = null;

            private ReloadInterface_Delegate _reloadinterface_callback = null;
            private DataManager.OutputData_Delegate _outputdata_callback = null;
            private DataManager.InputData_Delegate _inputdata_callback = null;

            /// DEBUG
            private TimeBenchmark _timer = null;
        }
    }
}
