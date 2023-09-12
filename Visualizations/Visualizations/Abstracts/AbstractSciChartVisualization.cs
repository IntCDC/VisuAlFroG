using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Core.Abstracts;
using Visualizations.SciChartInterface;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using Visualizations.Data;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartVisualization<SurfaceType, DataType> : AbstractVisualization
            where SurfaceType : SciChartSurface, new()
            where DataType : AbstractDataInterface, new()

        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return true; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected DataType DataInterface { get; set; }
            protected SurfaceType Content { get { return _content_surface; } }


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

                _content_surface = new SurfaceType();
                _content_surface.Name = ID;
                _content_surface.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                _content_surface.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);

                AttachChildContent(_content_surface);

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public override bool ReCreate()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Re-creating visualization");
                    _created = false;
                }
                if (DataInterface.RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }

                // Set data
                if (!DataInterface.Set(_content_surface))
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to set data");
                }
                _content_surface.ZoomExtents();

                return true;
            }
            /* TEMPLATE
            public override bool ReCreate()
            {
                if (!base.ReCreate())
                {
                return false;
                }
                _timer.Start();

                /// Add your stuff here

                _timer.Stop();
                _created = true;
                return _created;
            }
            */

            public sealed override Panel Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                _content_surface.ChartModifier.IsAttached = true;

                return base.Attach();
            }

            public sealed override bool Detach()
            {
                if (!_attached)
                {
                    // Required to release mouse handling
                    _content_surface.ChartModifier.IsAttached = false;
                }
                return base.Detach();
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content_surface.Dispose();
                    _content_surface = null;

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
                // New data does not require any further update of the SciChart visualizations

                if (new_data)
                {
                    _content_surface.ZoomExtents();
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SurfaceType _content_surface = null;
        }
    }
}
