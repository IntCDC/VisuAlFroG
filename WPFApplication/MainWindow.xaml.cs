using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChartInterface;
using Frontend.ChildWindows;
using Core.Utilities;
using Visualizations;
using Visualizations.SciChartInterface;
using Frontend.GUI;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Xml.Linq;
using static Frontend.ChildWindows.AbstractContent;



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
            // public functions

            // Used for stand-alone execution (DEBUG)
            public MainWindow() : this("[detached] Visualization Framework for Grasshopper (VisFroG)", false) { }


            public MainWindow(string app_name, bool soft_close)
            {
                _soft_close = soft_close;
                initialize(app_name);
                execute();
            }


            // Get reload function call from Grasshopper
            public void ReloadComponentFunction(ReloadFunctionCall func)
            {
                _reload_func = func;
            }


            // Soft close is used when called from Grasshopper.
            // Only change visibility and abort closing for being able to restore window on close.
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
            // public delegates

            public delegate void ReloadFunctionCall();


            /* ------------------------------------------------------------------*/
            // private functions

            public bool initialize(string app_name)
            {
                if (_initilized)
                {
                    return false;
                }
                _timer.Start();

                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;

                // Callback additionally invoked on loading of main window
                base.Loaded += on_loaded;
                // Callback invoked once per frame
                CompositionTarget.Rendering += once_per_frame;


                // Register child window content
                var log_content = new LogContent();
                Log.Default.RegisterListener(log_content.LogListener);
                window_contents.Add(log_content.Name(), log_content);


                // Initialize visualizations
                bool initilized = _vismanager.Initialize();
                bool setup = false;
                var scichart_service = _vismanager.GetService<SciChartInterfaceService>();
                if (scichart_service != null)
                {
                    scichart_service.SetGrid(_main_grid);
                    setup = true;
                }

                _timer.Stop(this.GetType().FullName);
                _initilized = initilized && setup;
                return _initilized;
            }


            public bool execute()
            {
                if (!_initilized)
                {
                    return false;
                }
                _timer.Start();

                //bool executed = _vismanager.Execute();

                draw_window();

                _timer.Stop(this.GetType().FullName);
                return true;
            }


            private void draw_window()
            {
                _windowtree.CreateRoot(_main_grid, windows_available_content, windows_request_content);
            }


            private void on_loaded(object sender, RoutedEventArgs routedEventArgs)
            {
                // so far unused ...
            }


            private void once_per_frame(object sender, EventArgs args)
            {
                // so far unused ...
            }


            // Provide names of available window content
            private List<Tuple<string, string, SetContentAvailableCall>> windows_available_content()
            {
                // Only show available
                var content_names = new List<Tuple<string, string, SetContentAvailableCall>>();
                foreach (var c in window_contents)
                {
                    if (c.Value.IsAvailable())
                    {
                        string header = c.Value.Header();
                        string name = c.Key;
                        // Provide info on content element: Header, Name (= unique ID), delegate to set availability of content element
                        content_names.Add(new Tuple<string, string, SetContentAvailableCall>(header, name, c.Value.SetAvailable));
                    }
                }
                return content_names;
            }


            public void windows_request_content(string content_name, Grid content_grid)
            {
                if (window_contents.ContainsKey(content_name))
                {
                    window_contents[content_name].ProvideContent(content_grid);
                    window_contents[content_name].SetAvailable(false);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "BUG: Invalid content name");
                }
            }


            /* ------------------------------------------------------------------*/
            // local variables

            private bool _initilized = false;
            private bool _soft_close = false;

            private VisualizationManager _vismanager = new VisualizationManager();
            private ChildBranch _windowtree = new ChildBranch();

            private ReloadFunctionCall _reload_func;
            private TimeBenchmark _timer = new TimeBenchmark();

            Dictionary<string, AbstractContent> window_contents = new Dictionary<string, AbstractContent>();

        }
    }
}
