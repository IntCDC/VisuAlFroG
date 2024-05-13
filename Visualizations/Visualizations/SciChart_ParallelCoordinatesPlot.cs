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
using Core.GUI;
using SciChartInterface;
using System.Windows.Media;



/*
 * Visualization: Parallel Coordinates Plot (2D)
 * 
 */
namespace Visualizations
{
    public class SciChart_ParallelCoordinatesPlot : AbstractSciChartParallel<SciChartParallelCoordinateSurface, ExpandoObject>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Parallel Coordinates Plot (SciChart)"; } }


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
                GetData(_Content);

                // Data Style--------------------------------------------

                // Chart Title Style
                var title_style = new System.Windows.Style();
                title_style.TargetType = typeof(SciChartSurfaceBase);

                Setter setter_title = new Setter();
                setter_title.Property = SciChartSurfaceBase.ChartTitleProperty;
                setter_title.Value = "";
                title_style.Setters.Add(setter_title);

                _Content.Style = title_style;

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

                _Content.LabelStyle = label_style;

                // Render Series Style
                var render_style = new System.Windows.Style();
                render_style.TargetType = typeof(BaseRenderableSeries);

                var new_color = ColorTheme.RandomColor();
                Setter setter_strokethickness = new Setter();
                setter_strokethickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
                setter_strokethickness.Value = 2;
                render_style.Setters.Add(setter_strokethickness);   

                var setter_stroke = new Setter();
                setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
                /// XXX TODO Set different colors for different series
                setter_stroke.Value = new DynamicResourceExtension("Color_StrokeDefault"); // new_color
                render_style.Setters.Add(setter_stroke);

                var setter_palette = new Setter();
                setter_palette.Property = BaseRenderableSeries.PaletteProviderProperty;
                /// XXX TODO Set different colors for different series
                setter_palette.Value = new DynamicResourceExtension("Color_StrokeDefault"); // new_color
                render_style.Setters.Add(setter_stroke);

                Trigger trigger = new Trigger();
                trigger.Property = BaseRenderableSeries.IsSelectedProperty;
                trigger.Value = true;
                Setter setter_trigger = new Setter();
                setter_trigger.Property = BaseRenderableSeries.StrokeProperty;
                setter_trigger.Value = new DynamicResourceExtension("Color_StrokeSelected");
                trigger.Setters.Add(setter_trigger);
                render_style.Triggers.Add(trigger);

                _Content.RenderableSeriesStyle = render_style;

                _Content.ZoomExtents();
            }
        }
    }
}
