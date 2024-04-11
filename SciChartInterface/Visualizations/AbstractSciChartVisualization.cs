using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Visuals;
using System.Windows;
using Core.Data;
using SciChart.Charting.Visuals.RenderableSeries;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Visualizations
    {
        public abstract class AbstractSciChartVisualization<SurfaceType, DataType> : AbstractVisualization
            where SurfaceType : SciChartSurface, new()
            where DataType : class, new()

        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return true; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected SurfaceType Content { get { return _content_surface; } }

            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize(DataManager.RequestCallback_Delegate request_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                this.RequestDataCallback = request_callback;

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
                if (this.RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }

                // Set data
                if (!this.GetData(_content_surface))
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

            public override bool GetData(object data_parent)
            {
                if (data_parent as SciChartSurface != null)
                {
                    var parent = data_parent as SciChartSurface;
                    var data = (List<DataType>)RequestDataCallback(typeof(List<DataType>));
                    if (data == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                        return false;
                    }
                    if (data.Count == 0)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing data");
                        return false;
                    }

                    foreach (var data_series in data)
                    {
                        var renderable_series = data_series as BaseRenderableSeries;
                        if (renderable_series != null)
                        {
                            renderable_series.Name = UniqueID.Generate();
                            renderable_series.SelectionChanged += event_selection_changed;
                            parent.RenderableSeries.Add(renderable_series);
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Can not convert to BaseRenderableSeries...");
                        }
                    }
                }
                else if (data_parent as SciChartParallelCoordinateSurface != null)
                {
                    var parent = data_parent as SciChartParallelCoordinateSurface;
                    var data = (ParallelCoordinateDataSource<DataType>)RequestDataCallback(typeof(ParallelCoordinateDataSource<DataType>));
                    if (data == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                        return false;
                    }

                    parent.ParallelCoordinateDataSource = data;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Can not convert data parent parameter to required type");
                    return false;
                }
                return true;
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            /// <exception cref="NotImplementedException"></exception>
            private void event_selection_changed(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SurfaceType _content_surface = null;
        }
    }
}
