using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using Core.GUI;
using Core.Utilities;
using System.Windows.Media;
using Visualizations.Data;
using System.Runtime.Remoting.Contexts;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractGenericVisualization<ContentType, DataType> : AbstractVisualization
            where ContentType : System.Windows.FrameworkElement, new()
            where DataType : AbstractDataInterface, new()
        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return false; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { }; } }

            protected DataType DataInterface { get; set; }
            protected ContentType Content { get { return (ContentType)_scroll_view.Content; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                DataInterface = new DataType();
                DataInterface.RequestDataCallback = _request_callback;

                _scroll_view = new ScrollViewer();
                _scroll_view.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _scroll_view.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                _scroll_view.Content = new ContentType();
                _scroll_view.Name = ID;
                _scroll_view.Background = ColorTheme.LightBackground;
                _scroll_view.Foreground = ColorTheme.DarkForeground;

                _scroll_view.PreviewMouseWheel += scrollviewer_previewmousewheel;

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public sealed override System.Windows.Controls.Panel Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }
                AttachChildContent(_scroll_view);
                return base.Attach();
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _scroll_view = null;

                    _initialized = false;
                }
                return base.Terminate();
            }

            public override void UpdateCallback(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }
                if (new_data)
                {
                    ReCreate();
                }
                else {
                    Update();
                }
            }

            /// <summary>
            /// Called when existing data has been updated
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public virtual bool Update() 
            {
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected void SetScrollViewBackground(Brush background)
            {
                _scroll_view.Background = background;
            }

            protected void ScrollToBottom()
            {
                _scroll_view.ScrollToBottom();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void scrollviewer_previewmousewheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
            {
                ScrollViewer scv = (ScrollViewer)sender;
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                e.Handled = true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ScrollViewer _scroll_view = null;
        }
    }
}
