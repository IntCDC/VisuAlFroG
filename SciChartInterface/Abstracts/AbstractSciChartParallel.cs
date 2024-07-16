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
using Core.GUI;



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

                /// DEBUG_Content.MouseLeftButtonUp += data_point_mouse_event;


                // Modifiers ---------------------------------------
                var modifier_reorder_axes = new ParallelAxisReorderModifier()
                {
                    IsEnabled = true
                };
                modifier_reorder_axes.AxesReordered += event_axes_reordered;

                var modifier_selection = new SeriesSelectionModifier()
                {
                    IsEnabled = true
                };
                modifier_selection.SelectionChanged += series_selection_changed;

                _Content.ChartModifier = new ModifierGroup(
                    modifier_reorder_axes,
                    modifier_selection,
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
                        IsEnabled = false,
                        ShowTooltipOn = ShowTooltipOptions.MouseHover,
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
                menubar.AddMenu(MenubarWindow.PredefinedMenuOption.OPTIONS, option_hint);
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

            private void series_selection_changed(object sender, EventArgs e)
            {
                /// TODO
                Log.Default.Msg(Log.Level.Info, "series_selection_changed...");
            }

            #endregion
        }
    }
}
