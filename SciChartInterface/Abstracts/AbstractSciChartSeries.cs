using System;
using System.Collections.Generic;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;
using SciChart.Charting.Visuals.Axes;
using System.Windows.Controls;
using Core.GUI;
using System.Windows;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.Annotations;
using System.Windows.Media;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartSeries<SurfaceType, DataType> : AbstractSciChartVisualization<SurfaceType>
            where SurfaceType : SciChartSurface, new()
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            #region public properties

            public sealed override Type _RequiredDataType { get; } = typeof(DataTypeSciChartSeries<DataType>);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractSciChartSeries(int uid) : base(uid) { }

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


                // Axis --------------------------------------------
                var xAxis = new NumericAxis()
                {
                    AxisTitle = "X Axis",
                    DrawMajorBands = false
                };
                _Content.XAxis = xAxis;

                var yAxis = new NumericAxis()
                {
                    AxisTitle = "Y Axis",
                    GrowBy = new SciChart.Data.Model.DoubleRange(0.2, 0.2),
                    DrawMajorBands = false,
                };
                _Content.YAxis = yAxis;


                // Interaction -------------------------------------

                /// DEBUG _Content.MouseLeftButtonUp += data_point_mouse_event;


                // Modifiers ---------------------------------------

                var data_point_selection = new DataPointSelectionModifier();
                data_point_selection.IsEnabled = true;
                data_point_selection.AllowsMultiSelection = true;

                _Content.ChartModifier = new ModifierGroup(
                    data_point_selection,
                    new RubberBandXyZoomModifier()
                    {
                        IsEnabled = false
                    },
                    new ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    },
                    new ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None
                    },
                    new MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
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

            /// </summary>
            /// <param name="data_parent"></param>
            /// <returns></returns>
            protected override bool apply_data(SciChartSurface data_parent)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                data_parent.RenderableSeries.Clear();

                if (this._RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                try
                {
                    var data = (List<DataType>)_RequestDataCallback(_DataUID);
                    if (data != null)
                    {
                        foreach (var data_series in data)
                        {
                            data_parent.RenderableSeries.Add(data_series);
                        }
                        return true;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                ///Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(List<DataType>).FullName);
                return false;
            }

            #endregion
        }
    }
}
