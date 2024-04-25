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

            public sealed override Type _RequiredDataType { get; } = typeof(DataTypeSciChartParallel<DataType>);


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Create()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    // Log Console does not depend on data
                    Log.Default.Msg(Log.Level.Warn, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();


                // Options --------------------------------------------
                var clue_select = new MenuItem();
                clue_select.Header = "[Left Mouse] Select Series | Drag & Drop Axes";
                clue_select.IsEnabled = false;

                var clue_zoom = new MenuItem();
                clue_zoom.Header = "[Mouse Wheel] Zoom";
                clue_zoom.IsEnabled = false;

                var clue_pan = new MenuItem();
                clue_pan.Header = "[Right Mouse] Pan";
                clue_pan.IsEnabled = false;

                var option_hint = new MenuItem();
                option_hint.Header = "Interaction Clues";
                option_hint.Items.Add(clue_select);
                option_hint.Items.Add(clue_zoom);
                option_hint.Items.Add(clue_pan);

                AddOption(option_hint);


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

                Content.ZoomExtents();

                _timer.Stop();
                _created = true;
                return _created;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected override bool GetData(SciChartSurface data_parent)
            {
                var parent = data_parent as SciChartParallelCoordinateSurface;
                parent.ParallelCoordinateDataSource = null;

                var data = (ParallelCoordinateDataSource<DataType>)_RequestDataCallback(_RequiredDataType);
                if (data != null)
                {
                    parent.ParallelCoordinateDataSource = data;
                    return true;
                }
                /// Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(ParallelCoordinateDataSource<DataType>).FullName);
                return false;
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
