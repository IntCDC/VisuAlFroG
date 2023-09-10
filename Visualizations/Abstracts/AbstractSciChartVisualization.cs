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
            where DataType : IDataInterface, new()

        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return true; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected DataType Data { get; set; }
            protected SurfaceType Content { get { return _content; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                Data = new DataType();
                Data.RequestDataCallback = _request_callback;

                _content = new SurfaceType();
                _content.Name = ID;

                Content.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                Content.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public sealed override Panel Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                if (!Data.Set(Content))
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to set data");
                }
                Content.ZoomExtents();

                _content.ChartModifier.IsAttached = true;

                AttachChildContent(_content);
                return base.Attach();
            }

            public sealed override bool Detach()
            {
                if (!_attached)
                {
                    /// TDOD Detach data, too?

                    // Required to release mouse handling
                    _content.ChartModifier.IsAttached = false;
                }
                return base.Detach();
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content.Dispose();
                    _content = null;

                    _initialized = false;
                }
                return base.Terminate();
            }

            public sealed override void UpdateCallback()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }
                /// TODO 

                _content.ZoomExtents();
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SurfaceType _content = null;
        }
    }
}
