using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using Core.GUI;
using Core.Utilities;
using System.Windows.Media;
using Visualizations.Data;
using System.Runtime.Remoting.Contexts;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;



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
            protected ContentType Content { get { return (ContentType)_content_scrollview.Content; } }


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

                _content_scrollview = new ScrollViewer();
                _content_scrollview.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _content_scrollview.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                _content_scrollview.Content = new ContentType();
                _content_scrollview.Name = ID;
                _content_scrollview.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
                _content_scrollview.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");

                _content_scrollview.PreviewMouseWheel += event_scrollviewer_mousewheel;

                AttachChildContent(_content_scrollview);

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            /* TEMPLATE
            public override bool ReCreate()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    // Log Console does not depend on data
                    Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return _created;
            }
            */


            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content_scrollview = null;

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

            protected void SetScrollViewBackground(string background_color_resource_name)
            {
                _content_scrollview.SetResourceReference(ScrollViewer.BackgroundProperty, background_color_resource_name);
            }

            protected void ScrollToBottom()
            {
                _content_scrollview.ScrollToBottom();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void event_scrollviewer_mousewheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
            {
                ScrollViewer scv = (ScrollViewer)sender;
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                e.Handled = true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ScrollViewer _content_scrollview = null;
        }
    }
}
