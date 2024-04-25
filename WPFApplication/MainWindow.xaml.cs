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
            public MainWindow() : this("[detached] Visual Analytics Framework for Grasshopper (VisuAlFroG)", true) { }

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="app_name"></param>
            /// <param name="detached"></param>
            public MainWindow(string app_name, bool detached = false)
            {
                _soft_close = !detached;
                _detached = detached;
                initialize(app_name);
                create();
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
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to updating input data");
                    return;
                }
                _basemanager.UpdateInputData(ref input_data);
            }

            /// <summary>
            /// Allows to provide arguments as string 
            /// </summary>
            /// <param name="arguments">Command line arguments as string</param>
            public void Arguments(string arguments)
            {
                _arguments.Parse(arguments);
                _arguments.Evaluate();
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


                // Window setup
                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;
                base.Icon = ImageLoader.ImageSourceFromFile(ResourcePaths.Locations.LogoIcons, "logo64.png");


                // Explicitly disable debug messages
                Log.Default.DisableDebug = true;


                // Register and parse cmd line arguments
                _arguments.Register("config", "c", 1, (List<string> parameters) =>
                {
                    _configurationservice.Load(parameters[0]);
                });
                _arguments.Register("help", "h", 0, (List<string> parameters) =>
                {
                    _arguments.PrintHelp();
                });
                // Proceeding if an error occurs during parsing arguments
                _arguments.Parse();


                // Initialize managers and services
                _configurationservice = new ConfigurationService();
                _basemanager = new Visualizations.BaseManager();
                _winmanager = new WindowManager();
                _colortheme = new ColorTheme();
                _menubar = new MenuBar();

                bool initialized = true;
                initialized &= _configurationservice.Initialize();
                initialized &= _basemanager.Initialize();
                initialized &= _winmanager.Initialize(_basemanager.GetContentCallbacks());
                initialized &= _colortheme.Initialize(App.Current.Resources, _menubar.MarkColorTheme);
                initialized &= _menubar.Initialize(this.Close, _colortheme.SetColorStyle, _configurationservice.Save, _configurationservice.Load, _basemanager.GetSendOutputDataCallback());

                // Register configurations
                _configurationservice.RegisterConfiguration(_basemanager._Name, _basemanager.CollectConfigurations, _basemanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_winmanager._Name, _winmanager.CollectConfigurations, _winmanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_colortheme._Name, _colortheme.CollectConfigurations, _colortheme.ApplyConfigurations);


                _timer.Stop();
                _initialized = initialized;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Error during initialization of: " + this.GetType().FullName + " - check previous messages for more information.");
                }
                return _initialized;
            }

            /// <summary>
            /// Create the WPF content of the application.
            /// </summary>
            /// <returns>True on successful creation of the content, false otherwise</returns>
            public bool create()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to creation");
                    return false;
                }
                _timer.Start();

                // Attach main menu bar to content
                var menubar_content = _menubar.Attach();
                if (menubar_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid menu bar content");
                }
                _menubar_element.Children.Add(menubar_content);

                // Attach window manager to content
                var winmanager_content = _winmanager.Attach();
                if (winmanager_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid menu bar content");
                }
                _subwindows_element.Children.Add(winmanager_content);


                // Evaluate previously parsed command line arguments
                _arguments.Evaluate();


                /// DEBUG
                load_example_data();


                _timer.Stop();
                return true;
            }

            /// DEBUG Load sample data in detached mode ...
            private void load_example_data()
            {
                if (_detached)
                {
                    var generator = new Random();
                    var sample_data = new GenericDataStructure();

                    int value_index = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        var data_branch = new GenericDataStructure();
                        data_branch._Label = "labled_" + i.ToString();

                        for (int j = 0; j < 25; j++)
                        {
                            var value = generator.Next(0, 50);
                            var data_leaf = new GenericDataEntry();
                            data_leaf.AddValue((double)value);

                            data_leaf._Metadata._Index = value_index;
                            data_leaf._Metadata._Label = "entry_" + j.ToString();
                            value_index++;

                            data_branch.AddEntry(data_leaf);
                        }
                        sample_data.AddBranch(data_branch);
                    }

                    UpdateInputData(ref sample_data);
                }
            }


            /* ------------------------------------------------------------------*/
            // local variables

            private bool _initialized = false;
            private bool _soft_close = false;
            private bool _detached = false;

            private CmdLineArguments _arguments = new CmdLineArguments();

            private ConfigurationService _configurationservice = null;
            private Visualizations.BaseManager _basemanager = null;
            private WindowManager _winmanager = null;
            private ColorTheme _colortheme = null;
            private MenuBar _menubar = null;

            /// DEBUG
            private TimeBenchmark _timer = null;
        }
    }
}
