using System;
using System.ComponentModel;
using System.Windows;
using Core.Utilities;
using Core.GUI;
using Core.Data;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Linq;
using Visualizations;
using System.Runtime.Remoting.Contexts;



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
            #region public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows to trigger reloading of the interface
            /// </summary>
            public delegate void ReloadInterface_Delegate();

            /// <summary>
            /// Callback to mark color theme menu item
            /// </summary>
            public delegate void MarkColorTheme_Delegate(ColorTheme.PredefinedThemes color_theme);

            #endregion

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
                _basemanager.SetOutputDataCallback(output_data_callback);
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
                _basemanager.UpdateInputData(input_data);
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


                // Window setup
                InitializeComponent();
                const string app_name = "Visual Analytics Framework for Grasshopper(VisuAlFroG)";
                base.Title = (_standalone) ? ("[stand-alone]" + app_name) : (app_name);
                base.Icon = ImageLoader.ImageSourceFromFile(ResourcePaths.Locations.LogoIcons, "logo64.png");
                base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                // Default window size in pixels
                base.Width = 1600;
                base.Height = 900;
                this.Show();

                LoadingProgressWindow loading_progress = new LoadingProgressWindow();

                // Explicitly disable debug messages
                Log.Default.DisableDebug = false;
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
                _basemanager = new Visualizations.BaseManager();
                _configurationservice = new ConfigurationService();
                _winmanager = new WindowManager();
                _colortheme = new ColorTheme();
                _menubar = new MainMenuBar();

                loading_progress.SetValue(20);

                bool initialized = true;
                initialized &= _basemanager.Initialize();
                initialized &= _configurationservice.Initialize();
                initialized &= _winmanager.Initialize(_basemanager.GetContentCallbacks());
                initialized &= _colortheme.Initialize(App.Current.Resources);
                initialized &= _menubar.Initialize();

                loading_progress.SetValue(30);

                // Register configurations
                _configurationservice.RegisterConfiguration(_basemanager._Name, _basemanager.CollectConfigurations, _basemanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_winmanager._Name, _winmanager.CollectConfigurations, _winmanager.ApplyConfigurations);
                _configurationservice.RegisterConfiguration(_colortheme._Name, _colortheme.CollectConfigurations, _colortheme.ApplyConfigurations);

                loading_progress.SetValue(40);

                // Attach all main menu options (order dependent!)
                _basemanager.AttachMenu(_menubar);
                _configurationservice.AttachMenu(_menubar);
                _colortheme.AttachMenu(_menubar);
                _menubar.AddSeparator(MainMenuBar.PredefinedMenuOption.FILE);
                _menubar.AddMenu(MainMenuBar.PredefinedMenuOption.FILE, MainMenuBar.GetDefaultMenuItem("Exit", close_callback_menu));

                loading_progress.SetValue(50);

                // Attach main menu bar to content
                _menubar_element.Children.Clear();
                _menubar_element.Children.Add(_menubar.Attach());
                _menubar_element.UpdateLayout();
                // Attach window manager to content
                _subwindows_element.Children.Clear();
                _subwindows_element.Children.Add(_winmanager.Attach());
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
                    _basemanager.UpdateInputData(sample_data);
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

            private bool close_callback_menu()
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
            private Visualizations.BaseManager _basemanager = null;
            private WindowManager _winmanager = null;
            private ColorTheme _colortheme = null;
            private MainMenuBar _menubar = null;

            private System.Windows.Controls.ProgressBar _progress_bar = new System.Windows.Controls.ProgressBar()
            {
                Minimum = 0,
                Maximum = 100,
                IsIndeterminate = false,
            };


            /// DEBUG
            private TimeBenchmark _timer = null;

            #endregion
        }
    }
}
