using System.Windows.Controls;
using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Visualizations.Abstracts;
using Visualizations.Data;
using Core.GUI;
using Core.Utilities;
using System;



/*
 * Visualization: Lines (2D)
 * 
 */
namespace Visualizations
{
    namespace Varieties
    {
        public class LinesVisualization : AbstractSciChartVisualization<SciChartSurface, DataInterfaceSciChartSeries<FastLineRenderableSeries>>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Lines (2D)"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// DEBUG
            /// </summary>
            ~LinesVisualization()
            {
                Console.WriteLine("DEBUG - DTOR: LinesVisualization");
            }

            public override bool ReCreate()
            {
                if (!base.ReCreate())
                {
                    return false;
                }
                _timer.Start();


                // Style--------------------------------------------
                foreach (var rs in Content.RenderableSeries)
                {
                    var default_style = new System.Windows.Style();
                    default_style.TargetType = typeof(FastLineRenderableSeries);

                    var new_color = ColorTheme.RandomColor();
                    var pointmarker_default = new EllipsePointMarker()
                    {
                        Stroke = new_color,
                        Fill = new_color,
                        Width = 10.0,
                        Height = 10.0
                    };
                    var pointmarker_selected = new EllipsePointMarker()
                    {
                        Stroke = ColorTheme.Color_StrokeSelected,
                        StrokeThickness = 3,
                        Fill = new_color,
                        Width = 10.0,
                        Height = 10.0
                    };

                    Setter setter_stroke = new Setter();
                    setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
                    setter_stroke.Value = new_color;
                    default_style.Setters.Add(setter_stroke);

                    Setter setter_thickness = new Setter();
                    setter_thickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
                    setter_thickness.Value = 3;
                    default_style.Setters.Add(setter_thickness);

                    Setter setter_point = new Setter();
                    setter_point.Property = BaseRenderableSeries.PointMarkerProperty;
                    setter_point.Value = pointmarker_default;
                    default_style.Setters.Add(setter_point);

                    Setter setter_point_selected = new Setter();
                    setter_point_selected.Property = BaseRenderableSeries.SelectedPointMarkerProperty;
                    setter_point_selected.Value = pointmarker_selected;
                    default_style.Setters.Add(setter_point_selected);

                    rs.Style = default_style;
                }


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


                _timer.Stop();
                _created = true;
                return _created;
            }
        }
    }
}
