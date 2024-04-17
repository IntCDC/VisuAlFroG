using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.ChartModifiers;
using System.Dynamic;
using SciChart.Charting.Visuals.Axes.LabelProviders;
using System.Windows.Controls;
using Core.Utilities;
using System;
using SciChartInterface.Abstracts;
using SciChartInterface.Data;
using Core.Data;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartParallel<SurfaceType, DataType> : AbstractSciChartVisualization<SurfaceType>
            where SurfaceType : SciChartSurface, new()
            where DataType : IDynamicMetaObjectProvider, new()
        {

            /* ------------------------------------------------------------------*/
            // properties

            public sealed override Type RequiredDataType { get; } = typeof(DataTypeSciChartParallel<DataType>);


            /* ------------------------------------------------------------------*/
            // public functions

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


                // Modifiers ---------------------------------------
                var modifier_reorder_axes = new SciChart.Charting.ChartModifiers.ParallelAxisReorderModifier()
                {
                    IsEnabled = true
                };
                modifier_reorder_axes.AxesReordered += event_axes_reordered;

                var modifier_selection = new SciChart.Charting.ChartModifiers.SeriesSelectionModifier()
                {
                    IsEnabled = true
                };
                modifier_selection.SelectionChanged += event_selection_changed;

                Content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    modifier_reorder_axes,
                    modifier_selection,
                    new SciChart.Charting.ChartModifiers.ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = SciChart.Charting.ChartModifiers.ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None,
                    },
                    new SciChart.Charting.ChartModifiers.MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
                    },
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier()
                    {
                        IsEnabled = false,
                        IsXAxisOnly = true
                    },
                    new SciChart.Charting.ChartModifiers.ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    }
                );


                // Set data -----------------------------------------
                if (!GetData(Content))
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to set data");
                }


                Content.ZoomExtents();
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected override bool GetData(object data_parent)
            {
                var parent = data_parent as SciChartParallelCoordinateSurface;
                var data = (ParallelCoordinateDataSource<DataType>)RequestDataCallback(RequiredDataType);
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                    return false;
                }
                parent.ParallelCoordinateDataSource = data;
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void event_axes_reordered(object sender, ParallelAxisReorderArgs e)
            {
                var parent = Content as SciChartParallelCoordinateSurface;
                var pcp_source = parent.ParallelCoordinateDataSource as ParallelCoordinateDataSource<ExpandoObject>;
                pcp_source.ReorderItems(e.OldIndex, e.NewIndex);
            }

            private void event_selection_changed(object sender, EventArgs e)
            {
                /// TODO
                Log.Default.Msg(Log.Level.Info, "event_selection_changed...");
            }
        }
    }
}
