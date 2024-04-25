using System;
using System.Collections.Generic;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;
using SciChart.Charting.Visuals.Axes;
using System.Windows.Controls;



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
            // properties

            public sealed override Type _RequiredDataType { get; } = typeof(DataTypeSciChartSeries<DataType>);


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
                clue_select.Header = "[Left Mouse] Point Select/Box-Select";
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


                // Axis --------------------------------------------
                var xAxis = new NumericAxis()
                {
                    AxisTitle = "Sample No",
                    DrawMajorBands = false
                };
                Content.XAxis = xAxis;

                var yAxis = new NumericAxis()
                {
                    AxisTitle = "Value",
                    GrowBy = new SciChart.Data.Model.DoubleRange(0.2, 0.2),
                    DrawMajorBands = false,
                };
                Content.YAxis = yAxis;


                // Modifiers ---------------------------------------
                var data_point_selection = new SciChart.Charting.ChartModifiers.DataPointSelectionModifier();
                data_point_selection.IsEnabled = true;
                data_point_selection.AllowsMultiSelection = true;

                Content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    data_point_selection,
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier()
                    {
                        IsEnabled = false
                    },
                    new SciChart.Charting.ChartModifiers.ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    },
                    new SciChart.Charting.ChartModifiers.ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = SciChart.Charting.ChartModifiers.ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None
                    },
                    new SciChart.Charting.ChartModifiers.MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
                    }
                );

                Content.ZoomExtents();

                _timer.Stop();
                _created = true;
                return _created;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            /// </summary>
            /// <param name="data_parent"></param>
            /// <returns></returns>
            protected override bool GetData(SciChartSurface data_parent)
            {
                data_parent.RenderableSeries.Clear();

                if (this._RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }

                var data = (List<DataType>)_RequestDataCallback(_RequiredDataType);
                if (data != null)
                {
                    foreach (var data_series in data)
                    {
                        data_parent.RenderableSeries.Add(data_series);
                    }
                    return true;
                }
                ///Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(List<DataType>).FullName);
                return false;
            }
        }
    }
}
