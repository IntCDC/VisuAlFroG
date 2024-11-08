using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.ChartModifiers;
using System.Dynamic;
using System.Collections.Generic;
using Core.Utilities;
using System;
using SciChartInterface.Data;
using Core.GUI;
using Core.Data;
using System.Runtime.Remoting.Contexts;



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
            #region public properties

            public sealed override Type _RequiredDataType { get; } = typeof(DataTypeSciChartParallel<DataType>);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractSciChartParallel(int uid) : base(uid) { }

            public override bool CreateUI()
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


                // Interaction -------------------------------------

                /// DEBUG
                _Content.MouseLeftButtonUp += data_point_mouse_event;


                // Modifiers ---------------------------------------
                var modifier_reorder_axes = new ParallelAxisReorderModifier();
                modifier_reorder_axes.IsEnabled = true;
                modifier_reorder_axes.AxesReordered += event_axes_reordered;

                var modifier_selection_series = new SeriesSelectionModifier();
                modifier_selection_series.IsEnabled = true;
                modifier_selection_series.SelectionChanged += event_series_selection_changed;

                _Content.ChartModifier = new ModifierGroup(
                    modifier_reorder_axes,
                    modifier_selection_series,
                    new ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None,
                    },
                    new MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
                    },
                    new RubberBandXyZoomModifier()
                    {
                        IsEnabled = false,
                        IsXAxisOnly = true
                    },
                    new RolloverModifier()
                    {
                        IsEnabled = true,
                        ShowTooltipOn = ShowTooltipOptions.MouseOver,
                        HoverDelay = 0.0,
                        ShowAxisLabels = false,
                    },
                    new LegendModifier()
                    {
                        IsEnabled = false,
                        ShowLegend = true,
                    },
                    new ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    }
                );

                _Content.ZoomExtents();

                _timer.Stop();
                _created = true;
                return _created;
            }

            public override void AttachMenu(MenubarWindow menubar)
            {
                base.AttachMenu(menubar);

                var option_hint = MenubarMain.GetDefaultMenuItem("Interaction");

                var clue_item = MenubarMain.GetDefaultMenuItem("Select Series | Drag & Drop Axes");
                clue_item.InputGestureText = "Left Mouse";
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                clue_item = MenubarMain.GetDefaultMenuItem("Zoom");
                clue_item.InputGestureText = "Mouse Wheel";
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                clue_item = MenubarMain.GetDefaultMenuItem("Pan");
                clue_item.InputGestureText = "Right Mouse";
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                menubar.AddMenu(MenubarWindow.PredefinedMenuOption.VIEW, option_hint);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override bool apply_data(SciChartSurface data_parent)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                var parent = data_parent as SciChartParallelCoordinateSurface;
                parent.ParallelCoordinateDataSource = null;
                foreach (var data_series in _Content.RenderableSeries)
                {
                    data_series.DataSeries.Clear();
                }
                data_parent.RenderableSeries.Clear();

                try
                {
                    var data = (ParallelCoordinateDataSource<DataType>)_RequestDataCallback(_DataUID);
                    if (data != null)
                    {
                        parent.ParallelCoordinateDataSource = data;
                        parent.ParallelCoordinateDataSource.Invalidate();

                        return true;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                /// Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(ParallelCoordinateDataSource<DataType>).FullName);
                return false;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void event_axes_reordered(object sender, ParallelAxisReorderArgs e)
            {
                var parent = _Content as SciChartParallelCoordinateSurface;
                var pcp_source = parent.ParallelCoordinateDataSource as ParallelCoordinateDataSource<ExpandoObject>;
                pcp_source.ReorderItems(e.OldIndex, e.NewIndex);
            }

            #endregion
        }
    }
}
