using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Visualizations.Abstracts;
using Visualizations.Data;
using Core.GUI;
using System.Windows.Media;
using System.Windows;
using Visualizations.Styles;
using System.Windows.Controls;
using Core.Utilities;
using System;



/*
 * Visualization: Columns (Bar Chart) (2D)
 * 
 */
namespace Visualizations
{
    namespace Varieties
    {
        public class ColumnsVisualization : AbstractSciChartVisualization<SciChartSurface, DataInterfaceSciChartSeries<FastColumnRenderableSeries>>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Columns (2D)"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// DEBUG
            /// </summary>
            ~ColumnsVisualization()
            {
                Console.WriteLine("DEBUG - DTOR: ColumnsVisualization");
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
                    default_style.TargetType = typeof(FastColumnRenderableSeries);

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
                    setter_stroke.Property = BaseRenderableSeries.PaletteProviderProperty;
                    setter_stroke.Value = new StrokePalette();
                    default_style.Setters.Add(setter_stroke);

                    /*var gradient = new LinearGradientBrush();
                    gradient.StartPoint = new Point(0.0, 0.0);
                    gradient.EndPoint = new Point(1.0, 1.0);
                    gradient.GradientStops = new GradientStopCollection();
                    var gs1 = new GradientStop() { Color = Colors.Blue, Offset = 0.2 };
                    gradient.GradientStops.Add(gs1);
                    var gs2 = new GradientStop() { Color = Colors.Green, Offset = 0.8 };
                    gradient.GradientStops.Add(gs2);*/

                    Setter setter_gradient = new Setter();
                    setter_gradient.Property = FastColumnRenderableSeries.FillProperty;
                    setter_gradient.Value = new SolidColorBrush(ColorTheme.RandomColor()); // gradient;
                    default_style.Setters.Add(setter_gradient);

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
                        IsEnabled = true
                    }
                );


                _timer.Stop();
                _created = true;
                return _created;
            }
        }
    }
}
