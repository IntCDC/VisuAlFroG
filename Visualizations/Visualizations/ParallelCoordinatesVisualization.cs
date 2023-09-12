using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using Visualizations.Abstracts;
using SciChart.Charting.ChartModifiers;
using Visualizations.Data;
using System.Dynamic;
using Core.GUI;
using SciChart.Charting.Visuals.Axes.LabelProviders;
using System.Windows.Controls;
using System.Windows.Media;
using Core.Utilities;



/*
 * Visualization: Parallel Coordinates Plot (2D)
 * 
 */
namespace Visualizations
{
    namespace Varieties
    {
        public class ParallelCoordinatesVisualization : AbstractSciChartVisualization<SciChartParallelCoordinateSurface, DataInterfaceSciChartParallel<ExpandoObject>>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Parallel Coordinates Plot (2D)"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool ReCreate()
            {
                if (!base.ReCreate())
                {
                    return false;
                }
                _timer.Start();


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

                Setter setter_fontsize = new Setter();
                setter_fontsize.Property = DefaultTickLabel.FontSizeProperty;
                setter_fontsize.Value = 11.0;
                label_style.Setters.Add(setter_fontsize);

                Setter setter_fontweight = new Setter();
                setter_fontweight.Property = DefaultTickLabel.FontWeightProperty;
                setter_fontweight.Value = FontWeights.Bold;
                label_style.Setters.Add(setter_fontweight);

                Content.LabelStyle = label_style;


                // Render Series Style
                var render_style = new System.Windows.Style();
                render_style.TargetType = typeof(BaseRenderableSeries);

                Setter setter_strokethickness = new Setter();
                setter_strokethickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
                setter_strokethickness.Value = 2;
                render_style.Setters.Add(setter_strokethickness);

                Setter setter_stroke = new Setter();
                setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
                setter_stroke.Value = ColorTheme.Color_LightForeground;
                render_style.Setters.Add(setter_stroke);

                Content.RenderableSeriesStyle = render_style;


                // Options --------------------------------------------
                var clue_select = new MenuItem();
                clue_select.Header = "[Left Mouse] Drag & Drop Axes";
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


                // Modifiers ---------------------------------------
                var reorder_modifier = new SciChart.Charting.ChartModifiers.ParallelAxisReorderModifier()
                {
                    IsEnabled = true
                };
                reorder_modifier.AxesReordered += parallelaxisreordermodifier_axesreordered;

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
                    reorder_modifier
                ) ;


            _timer.Stop();
                _created = true;
                return _created;
            }



            /* ------------------------------------------------------------------*/
            // private functions


            private void parallelaxisreordermodifier_axesreordered(object sender, ParallelAxisReorderArgs e)
            {
                var pcp_source = Content.ParallelCoordinateDataSource as ParallelCoordinateDataSource<ExpandoObject>;
                pcp_source.ReorderItems(e.OldIndex, e.NewIndex);
            }
        }
    }
}
