using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Core.GUI;
using Core.Utilities;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractGenericVisualization<ContentType, DataType> : AbstractVisualization where ContentType : System.Windows.FrameworkElement, new()
        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return false; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { }; } }

            protected ContentType Content { get { return (ContentType)_content.Content; } }
            protected ScrollViewer ScrollView { get { return _content; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _content = new ScrollViewer();
                ScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                ScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                _content.Content = new ContentType();
                _content.Name = ID;
                _content.Background = ColorTheme.GenericBackground;
                _content.Foreground = ColorTheme.GenericForeground;

                ContentChild.Children.Add(_content);

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content = null;

                    _initialized = false;
                }
                return base.Terminate();
            }

            public new DataType Data()
            {
                return (DataType)_request_data_callback(typeof(DataType));
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            private ScrollViewer _content = null;
        }
    }
}
