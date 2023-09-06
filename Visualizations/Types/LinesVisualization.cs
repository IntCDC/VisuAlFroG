using System;
using Core.Abstracts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Visualizations.SciChartInterface;
using System.Windows;
using Core.Utilities;
using System.Runtime.Remoting.Contexts;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Model.DataSeries;
using SciChart.Drawing;
using SciChart.Core;
using SciChart.Data;
using System.Windows.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.RenderableSeries.Animations;
using SciChart.Charting.Model.ChartSeries;
using System.ComponentModel;
using System.Linq;
using SciChart.Charting.Visuals.PointMarkers;
using System.Windows.Input;
using Visualizations.Abstracts;
using Visualizations.Interaction;
using Visualizations.Management;
using SciChart.Charting.ChartModifiers;
using SciChart.Core.Utility.Mouse;



/*
 * Visualization: Lines (2D)
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class LinesVisualization : AbstractSciChartVisualization<SciChartSurface, SciChartUniformData>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Lines (2D)"; } }


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
                    Log.Default.Msg(Log.Level.Info, "Skipping re-creation of content");
                    return true;
                }
                if (_request_data_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();


                Content.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                Content.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);


                // Data Series -------------------------------------
                var render_series = new FastLineRenderableSeries();
                render_series.Name = "line_series_" + ID;
                render_series.Stroke = Colors.Aquamarine;
                render_series.StrokeThickness = 2;
                render_series.PointMarker = Selection_PointMarker.Default;
                render_series.SelectedPointMarker = Selection_PointMarker.Selected;
                render_series.DataSeries = Data();
                Content.RenderableSeries.Add(render_series);
                Content.ZoomExtents();

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
                Content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
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
                    },
                    new SciChart.Charting.ChartModifiers.DataPointSelectionModifier()
                    {
                        IsEnabled = true,
                    }
                );


                // Annotation --------------------------------------
                var textAnnotation = new TextAnnotation()
                {
                    Text = "|----------[Interaction]----------|" + Environment.NewLine +
                            "Left Mouse:  Select/Box-Select" + Environment.NewLine +
                            "Mouse Wheel: Zoom" + Environment.NewLine +
                            "Right Mouse: Pan",
                    X1 = 2.0,
                    Y1 = 2.0
                };
                Content.Annotations.Add(textAnnotation);

                _timer.Stop();
                _created = true;
                return _created;
            }
        }
    }
}
