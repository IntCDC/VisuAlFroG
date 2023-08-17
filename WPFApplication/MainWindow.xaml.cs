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



/*
 * Main WPF Application
 * 
 * 
 * Interaction logic for MainWindow.xaml
 * 
 */

using ContentDataType = System.Tuple<string, string, Core.Abstracts.AbstractContent.DetachContentCallback>;
using ContentDataListType = System.Collections.Generic.List<System.Tuple<string, string, Core.Abstracts.AbstractContent.DetachContentCallback>>;

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
            public delegate void ReloadCallback();


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
            public void RegisterReloadCallback(ReloadCallback reloadCallback)
            {
                _reload_func = reloadCallback;
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
                base.Icon = new BitmapImage(new Uri("resources/logo64.png", UriKind.Relative));

                // Callback additionally invoked on loading of main window
                base.Loaded += on_loaded;
                // Callback invoked once per frame
                CompositionTarget.Rendering += once_per_frame;

                // Start logging
                var log_content = new LogContent();
                Log.Default.RegisterListener(log_content.LogListener);

                // Initialize visualizations (prior to registration)
                bool initilized = _vismanager.Initialize();

                // Create visualizations
                var testvis = new TestVisualization();

                _menubar.RegisterCloseCallback(this.Close);

                // Register window content
                window_contents.Add(log_content.ID(), log_content);
                window_contents.Add(testvis.ID(), testvis);

                Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + base.Title);
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

                // Draw window content
                _menubar.Create(_menubar_element);
                _subwindows.CreateRoot(_subwindows_element, windows_available_content, windows_request_content);

                _timer.Stop();
                return true;
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
            /// >> Called by child leaf in _subwindows
            /// </summary>
            private ContentDataListType windows_available_content()
            {
                var content_ids = new ContentDataListType();
                foreach (var c in window_contents)
                {
                    if (!c.Value.IsAttached())
                    {
                        string id = c.Key;
                        string name = c.Value.Header();
                        // Provide info on content element: ID, Name, and delegate to set availability of content element
                        content_ids.Add(new ContentDataType(id, name, c.Value.DetachContent));
                    }
                }
                return content_ids;
            }


            /// <summary>
            /// Draw requested content to provided parent content element.
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public bool windows_request_content(string content_id, Grid content_element)
            {
                if (window_contents.ContainsKey(content_id))
                {
                    return window_contents[content_id].AttachContent(content_element);
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

            private WindowBranch _subwindows = new WindowBranch();
            private MenuBar _menubar = new MenuBar();

            private ReloadCallback _reload_func;
            private TimeBenchmark _timer = new TimeBenchmark();

            Dictionary<string, AbstractContent> window_contents = new Dictionary<string, AbstractContent>();
        }
    }
}
