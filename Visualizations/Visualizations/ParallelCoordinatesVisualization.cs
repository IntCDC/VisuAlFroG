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



/*
 * Visualization: Parallel Coordinates Plot (2D)
 * 
 */
namespace Visualizations
{
    public class ParallelCoordinatesVisualization : AbstractSciChartParallel<SciChartParallelCoordinateSurface, ExpandoObject>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string Name { get { return "Parallel Coordinates Plot (2D)"; } }


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

            ///parent.DrawSplines = true;

            // Chart Title Style
            var title_style = new System.Windows.Style();
            title_style.TargetType = typeof(SciChartSurfaceBase);

            Setter setter_title = new Setter();
            setter_title.Property = SciChartSurfaceBase.ChartTitleProperty;
            setter_title.Value = "";
            title_style.Setters.Add(setter_title);

            Content.Style = title_style;

            // Label Style
            var label_style = new System.Windows.Style();
            label_style.TargetType = typeof(DefaultTickLabel);

            Setter setter_fontcolor = new Setter();
            setter_fontcolor.Property = DefaultTickLabel.ForegroundProperty;
            setter_fontcolor.Value = new DynamicResourceExtension("Brush_Foreground");
            label_style.Setters.Add(setter_fontcolor);

            Setter setter_fontsize = new Setter();
            setter_fontsize.Property = DefaultTickLabel.FontSizeProperty;
            setter_fontsize.Value = 11.0;
            label_style.Setters.Add(setter_fontsize);

            Setter setter_fontweight = new Setter();
            setter_fontweight.Property = DefaultTickLabel.FontWeightProperty;
            setter_fontweight.Value = FontWeights.SemiBold;
            label_style.Setters.Add(setter_fontweight);

            Content.LabelStyle = label_style;

            // Render Series Style
            var render_style = new System.Windows.Style();
            render_style.TargetType = typeof(BaseRenderableSeries);

            Setter setter_strokethickness = new Setter();
            setter_strokethickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
            setter_strokethickness.Value = 3;
            render_style.Setters.Add(setter_strokethickness);

            var setter_stroke = new Setter();
            setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
            setter_stroke.Value = new DynamicResourceExtension("Color_StrokeDefault");
            render_style.Setters.Add(setter_stroke);

            Trigger trigger = new Trigger();
            trigger.Property = BaseRenderableSeries.IsSelectedProperty;
            trigger.Value = true;
            Setter setter_trigger = new Setter();
            setter_trigger.Property = BaseRenderableSeries.StrokeProperty;
            setter_trigger.Value = new DynamicResourceExtension("Color_StrokeSelected");
            trigger.Setters.Add(setter_trigger);
            render_style.Triggers.Add(trigger);

            Content.RenderableSeriesStyle = render_style;


            // Options --------------------------------------------
            var clue_select = new MenuItem();
            clue_select.Header = "[Left Mouse] Select Series | Drag & Drop Axes";
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
        ~ParallelCoordinatesVisualization()
        {
            Console.WriteLine("DEBUG - DTOR: ParallelCoordinatesVisualization");
        }
    }
}
