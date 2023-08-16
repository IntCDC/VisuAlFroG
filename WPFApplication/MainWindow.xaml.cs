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
using System.Windows.Media.Imaging;
using VisFroG_WPF.Properties;



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
            /// Get reload function call from Grasshopper
            /// </summary>
            public void ReloadComponentFunction(ReloadFunctionCall func)
            {
                _reload_func = func;
            }


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
            // public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows to trigger relaoding of the interface
            /// </summary>
            public delegate void ReloadFunctionCall();


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


                InitializeComponent();
                base.Title = app_name;
                base.Width = 1280;
                base.Height = 720;
                base.Icon = new BitmapImage(new Uri("visfrog.ico", UriKind.Relative));

                // Callback additionally invoked on loading of main window
                base.Loaded += on_loaded;
                // Callback invoked once per frame
                CompositionTarget.Rendering += once_per_frame;


                // Register window content
                var log_content = new LogContent();
                Log.Default.RegisterListener(log_content.LogListener);
                window_contents.Add(log_content.ID(), log_content);


                // Initialize visualizations (prior to registration)
                bool initilized = _vismanager.Initialize();

                // Register visualizations as window content
                var testvis = new TestVisualization();
                window_contents.Add(testvis.ID(), testvis);


                _timer.Stop();
                _initilized = initilized;
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

                draw_window();

                _timer.Stop();
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


            /// <summary>
            ///  Provide necessary information of available window content
            /// Called by child leaf in _windowtree
            /// </summary>
            private List<Tuple<string, string, AbstractContent.ContentAttachedCall>> windows_available_content()
            {
                var content_ids = new List<Tuple<string, string, AbstractContent.ContentAttachedCall>>();
                foreach (var c in window_contents)
                {
                    if (!c.Value.IsAttached())
                    {
                        string id = c.Key;
                        string name = c.Value.Header();
                        // Provide info on content element: ID, Name, and delegate to set availability of content element
                        content_ids.Add(new Tuple<string, string, AbstractContent.ContentAttachedCall>(id, name, c.Value.ContendAttached));
                    }
                }
                return content_ids;
            }


            /// <summary>
            /// Draw requested content to provided grid
            /// Called by child leaf in _windowtree
            /// </summary>
            public bool windows_request_content(string content_id, Grid content_grid)
            {
                if (window_contents.ContainsKey(content_id))
                {
                    return window_contents[content_id].AttachContent(content_grid);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown content id");
                }
                return false;
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
