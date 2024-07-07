using System.ComponentModel;
using System.Windows;
using Core.Utilities;
using Core.GUI;
using Core.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;



/*
 * Main WPF application
 * 
 */
namespace Frontend
{
    namespace Application
    {

        public partial class MainWindow : Window
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public MainWindow() : this(false) { }

            public MainWindow(bool called_from_interface)
            {
                _soft_close = called_from_interface;
                _standalone = !called_from_interface;
                initialize();
            }

            /// <summary>
            /// Allows to provide arguments as string 
            /// </summary>
            /// <param name="arguments">Command line arguments as string</param>
            public void Arguments(string arguments)
            {
                if (_standalone)
                {
                    Log.Default.Msg(Log.Level.Error, "Ignoring action. Application is defined as stand-alone but used via interface. Set appropriate flag in CTOR via MainWindow(true).");
                    return;
                }
                _arguments.Parse(arguments);
                _arguments.Evaluate();
            }

            /// <summary>
            /// Callback to pass output data back to the interface.
            /// </summary>
            /// <param name="output_data_callback">callback from the DataManager to pipe new output data to the interface.</param>
            public void SetOutputDataCallback(DataManager.SetDataCallback_Delegate output_data_callback)
            {
                if (_standalone)
                {
                    Log.Default.Msg(Log.Level.Error, "Ignoring action. Application is defined as stand-alone but used via interface. Set appropriate flag in CTOR via MainWindow(true).");
                    return;
                }
                _contentmanager.SetOutputDataCallback(output_data_callback);
            }

            /// <summary>
            /// Get input data from interface.
            /// </summary>
            /// <param name="input_data">Reference to the input data hold by the interface.</param>
            public void UpdateInputData(GenericDataStructure input_data)
            {
                if (_standalone)
                {
                    Log.Default.Msg(Log.Level.Error, "Ignoring action. Application is defined as stand-alone but used via interface. Set appropriate flag in CTOR via MainWindow(true).");
                    return;
                }
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to updating input data");
                    return;
                }
                _contentmanager.UpdateInputData(input_data);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

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

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            /// <summary>
            /// Initialize the main WPF window, the services and the managers...
            /// </summary>
            /// <returns>True on successful initialization, false otherwise.</returns>
            public bool initialize()
            {
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Warn, "Initialization should only be called once");
                    return false;
                }
                _timer = new TimeBenchmark();
                _timer.Start();

                ///Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                ///Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                // Window setup
                InitializeComponent();
                const string app_name = "Visual Analytics Framework for Grasshopper (VisuAlFroG)";
                base.Title = (_standalone) ? ("[stand-alone] " + app_name) : (app_name);
                base.Icon = ImageLoader.ImageSourceFromFile(ResourcePaths.Locations.LogoIcons, "logo64.png");
                base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                // Default window size in pixels
                base.Width = 1600;
                base.Height = 900;
                this.Show();

                LoadingProgressWindow loading_progress = new LoadingProgressWindow();

                // Explicitly disable debug messages
                Log.Default.DisableDebug = true;
                Log.Default.DumpFile = true;


                // Register and parse cmd line arguments
                _arguments.Register("config", "c", 1, (List<string> parameters) =>
                {
                    _configurationservice.LoadFile(parameters[0]);
                });
                _arguments.Register("help", "h", 0, (List<string> parameters) =>
                {
                    _arguments.PrintHelp();
                });
                // Proceeding if an error occurs during parsing arguments
                _arguments.Parse();

                loading_progress.SetValue(10);

                // Initialize managers and services
                _contentmanager = new Visualizations.ContentManager();
                _configurationservice = new ConfigurationService();
                _winmanager = new WindowManager();
                _colortheme = new ColorTheme();
                _menubar = new MenubarMain();

                loading_progress.SetValue(20);

                bool initialized = true;
                initialized &= _contentmanager.Initialize();
                initialized &= _configurationservice.Initialize();
                initialized &= _winmanager.Initialize(_contentmanager.GetContentCallbacks());
                initialized &= _colortheme.Initialize(App.Current.Resources);
                initialized &= _menubar.Initialize();

                loading_progress.SetValue(30);

                // Register configurations
                _configurationservice.RegisterConfiguration(_contentmanager._Name, _contentmanager.CollectConfigurations, _contentmanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_winmanager._Name, _winmanager.CollectConfigurations, _winmanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_colortheme._Name, _colortheme.CollectConfigurations, _colortheme.ApplyConfigurations);

                loading_progress.SetValue(40);

                // Attach all main menu options (order dependent!)
                _contentmanager.AttachMenu(_menubar);
                _configurationservice.AttachMenu(_menubar);
                _colortheme.AttachMenu(_menubar);
                _menubar.AddSeparator(MenubarMain.PredefinedMenuOption.FILE);
                _menubar.AddMenu(MenubarMain.PredefinedMenuOption.FILE, MenubarMain.GetDefaultMenuItem("Exit", exit_app_click));

                loading_progress.SetValue(50);

                // Attach main menu bar to content
                _menubar_element.Children.Clear();
                _menubar_element.Children.Add(_menubar.AttachUI());
                _menubar_element.UpdateLayout();
                // Attach window manager to content
                _subwindows_element.Children.Clear();
                _subwindows_element.Children.Add(_winmanager.AttachUI());
                _subwindows_element.UpdateLayout();

                loading_progress.SetValue(60);

                // Evaluate previously parsed command line arguments
                _arguments.Evaluate();

                loading_progress.SetValue(70);

                // Load default window configuration
                _winmanager.CreateDefault();

                loading_progress.SetValue(80);

                /// Provide example data for detached mode
                if (_standalone)
                {
                    var sample_data = TestData.Generate();
                    _contentmanager.UpdateInputData(sample_data);
                }

                loading_progress.SetValue(90);

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

                loading_progress.Close();
                return _initialized;
            }

            private bool exit_app_click()
            {
                this.Close();
                return true;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private bool _initialized = false;
            private bool _soft_close = false;
            private bool _standalone = false;

            private CmdLineArguments _arguments = new CmdLineArguments();

            private ConfigurationService _configurationservice = null;
            private Visualizations.ContentManager _contentmanager = null;
            private WindowManager _winmanager = null;
            private ColorTheme _colortheme = null;
            private MenubarMain _menubar = null;

            /// DEBUG
            private TimeBenchmark _timer = null;

            #endregion
        }
    }
}
