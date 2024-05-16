using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Visuals;
using System.Windows;
using Core.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartVisualization<SurfaceType> : AbstractVisualization
            where SurfaceType : SciChartSurface, new()

        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool _MultipleInstances { get { return true; } }
            public sealed override List<Type> _DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected SurfaceType _Content { get { return _content_surface; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize(DataManager.GetDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback)
            {
                _timer.Start();

                if (base.Initialize(request_data_callback, request_menu_callback))
                {
                    _content_surface = new SurfaceType();
                    _content_surface.Name = _ID;
                    _content_surface.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                    _content_surface.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);

                    attach_child_content(_content_surface);

                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }

                _timer.Stop();
                return _initialized;
            }

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
                if (!_Attached)
                {
                    // Required to release mouse handling
                    _content_surface.ChartModifier.IsAttached = false;
                }
                return base.Detach();
            }

            public override bool Terminate()
            {
                if (_created)
                {
                    _content_surface.Dispose();
                    _content_surface = null;

                    _created = false;
                }
                return base.Terminate();
            }

            public override void Update(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }

                if (new_data)
                {
                    apply_data(_Content);
                    attach_data_menu();
                    _Content.ZoomExtents();
                }
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected abstract bool apply_data(SciChartSurface data_parent);

            protected sealed override bool apply_data<DataParentType>(out DataParentType data_parent)
            {
                throw new InvalidOperationException("Call alternatively implemented GetData() method");
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SurfaceType _content_surface = null;
        }
    }
}
