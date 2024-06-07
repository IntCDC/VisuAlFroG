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

                _menu.Clear(ContentMenuBar.PredefinedMenuOption.OPTIONS);
                var option_hint = MainMenuBar.GetDefaultMenuItem("Interaction Clues");
                var clue_item = MainMenuBar.GetDefaultMenuItem("[Left Mouse] Select Series | Drag & Drop Axes", null);
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                clue_item = MainMenuBar.GetDefaultMenuItem("[Mouse Wheel] Zoom", null);
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                clue_item = MainMenuBar.GetDefaultMenuItem("[Right Mouse] Pan", null);
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                _menu.AddMenu(ContentMenuBar.PredefinedMenuOption.OPTIONS, option_hint);


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

                _Content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
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
                    /* new SciChart.Charting.ChartModifiers.RolloverModifier()
                    {
                        IsEnabled = true,
                        ShowTooltipOn = SciChart.Charting.ChartModifiers.ShowTooltipOptions.MouseHover,
                    }, */
                    /* new SciChart.Charting.ChartModifiers.LegendModifier()
                    {
                        IsEnabled = true,
                        ShowLegend = true,
                    }, */
                    new SciChart.Charting.ChartModifiers.ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    }
                );

                _Content.ZoomExtents();

                _timer.Stop();
                _created = true;
                return _created;
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

            private void event_selection_changed(object sender, EventArgs e)
            {
                /// TODO
                Log.Default.Msg(Log.Level.Info, "event_selection_changed...");
            }

            #endregion
        }
    }
}
