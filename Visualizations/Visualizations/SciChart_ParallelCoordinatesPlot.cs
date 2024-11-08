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
using System.Runtime.Remoting.Contexts;
using SciChart.Core.Extensions;
using SciChart.Charting.Visuals.PointMarkers;



/*
 * Visualization: Parallel Coordinates Plot (2D)
 * 
 */
namespace Visualizations
{
    public class SciChart_ParallelCoordinatesPlot : AbstractSciChartParallel<SciChartParallelCoordinateSurface, ExpandoObject>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Parallel Coordinates Plot (SciChart)"; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public SciChart_ParallelCoordinatesPlot(int uid) : base(uid) { }

        public override void Update(bool new_data)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }

            if (new_data)
            {
                // Apply style before new data is applied
                apply_style();

                apply_data(_Content);
                _Content.UpdateLayout();
                _Content.ZoomExtents();
            }
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        private void apply_style()
        {
            const int stroke_thickness = 2;
            const double marker_size = 8.0;

            // Requires _Content to be SciChartParallelCoordinateSurface and can not be set in abstract parent class
            _Content.ChartTitle = "";
            _Content.DrawSplines = false;

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

            var series_style = new System.Windows.Style();
            series_style.TargetType = typeof(BaseRenderableSeries);

            var setter_stroke = new Setter();
            setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
            var theme_color = ColorTheme.SinglePaletteColor(); // new DynamicResourceExtension("Color_StrokeDefault"); ColorTheme.RandomColor();
            setter_stroke.Value = theme_color; // new_color;
            series_style.Setters.Add(setter_stroke);

            Setter setter_strokethickness = new Setter();
            setter_strokethickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
            setter_strokethickness.Value = stroke_thickness;
            series_style.Setters.Add(setter_strokethickness);

            // SELECTION STYLE
            Trigger trigger = new Trigger();
            trigger.Property = BaseRenderableSeries.IsSelectedProperty;
            trigger.Value = true;
            Setter setter_trigger = new Setter();
            setter_trigger.Property = BaseRenderableSeries.StrokeProperty;
            setter_trigger.Value = new DynamicResourceExtension("Color_StrokeSelected");
            trigger.Setters.Add(setter_trigger);
            series_style.Triggers.Add(trigger);

            Setter setter_pointmarker = new Setter();
            setter_pointmarker.Property = BaseRenderableSeries.PointMarkerTemplateProperty;
            var marker_template = new ControlTemplate();
            marker_template.TargetType = typeof(PointMarker);
            var point_marker = new FrameworkElementFactory(typeof(EllipsePointMarker));
            point_marker.SetValue(EllipsePointMarker.FillProperty, theme_color);
            point_marker.SetValue(EllipsePointMarker.WidthProperty, marker_size);
            point_marker.SetValue(EllipsePointMarker.HeightProperty, marker_size);
            point_marker.SetValue(EllipsePointMarker.AntiAliasingProperty, true);
            marker_template.VisualTree = point_marker;
            setter_pointmarker.Value = marker_template;
            series_style.Setters.Add(setter_pointmarker);
            // SELECTION STYLE

            /// Must be set before new data is applied!
            _Content.RenderableSeriesStyle = series_style;
        }

        #endregion
    }
}
