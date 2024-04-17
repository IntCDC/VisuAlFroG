using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Core.GUI;
using System.Windows;
using System.Windows.Controls;
using System;
using SciChartInterface.Abstracts;



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

        public override bool ReCreate()
        {
            _timer.Start();

            if (!base.ReCreate())
            {
                return false;
            }


            // Style--------------------------------------------
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


            _timer.Stop();
            _created = true;
            return _created;
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
