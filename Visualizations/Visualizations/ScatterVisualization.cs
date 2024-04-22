using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Core.GUI;
using System.Windows;
using System.Windows.Controls;
using System;
using SciChartInterface.Abstracts;
using Core.Utilities;
using SciChartInterface;
using System.Windows.Media;



/*
 * Visualization: Scatter Plot Matrices (SPLOM) (2D)
 * 
 */
namespace Visualizations
{
    public class ScatterVisualization : AbstractSciChartSeries<SciChartSurface, XyScatterRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string Name { get { return "Scatter Plot (2D)"; } }


        /* ------------------------------------------------------------------*/
        // public functions

        public override void Update(bool new_data)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }

            if (new_data)
            {
                GetData(Content);

                // Data Style--------------------------------------------
                foreach (var rs in Content.RenderableSeries)
                {
                    var default_style = new System.Windows.Style();
                    default_style.TargetType = typeof(XyScatterRenderableSeries);

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
                        StrokeThickness = 3,
                        Fill = new_color,
                        Width = 10.0,
                        Height = 10.0
                    };
                    pointmarker_selected.SetResourceReference(EllipsePointMarker.StrokeProperty, "Color_StrokeSelected");

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
            }
            Content.ZoomExtents();
        }


        /// <summary>
        /// DEBUG
        /// </summary>
        ~ScatterVisualization()
        {
            Console.WriteLine("DEBUG - DTOR: ScatterVisualization");
        }
    }
}
