using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.GUI;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System;
using SciChartInterface.Abstracts;
using SciChartInterface;
using Core.Utilities;



/*
 * Visualization: Columns (Bar Chart) (2D)
 * 
 */
namespace Visualizations
{
    public class ColumnsVisualization : AbstractSciChartSeries<SciChartSurface, FastColumnRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Columns (2D)"; } }


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
                    default_style.TargetType = typeof(FastColumnRenderableSeries);

                    var new_color = ColorTheme.RandomColor();

                    Setter setter_stroke = new Setter();
                    setter_stroke.Property = FastColumnRenderableSeries.PaletteProviderProperty;
                    setter_stroke.Value = new StrokePalette();
                    default_style.Setters.Add(setter_stroke);

                    Setter setter_gradient = new Setter();
                    setter_gradient.Property = FastColumnRenderableSeries.FillProperty;
                    setter_gradient.Value = new SolidColorBrush(ColorTheme.RandomColor()); // gradient;
                    default_style.Setters.Add(setter_gradient);

                    rs.Style = default_style;
                }
                Content.ZoomExtents();
            }
        }


        /// <summary>
        /// DEBUG
        /// </summary>
        ~ColumnsVisualization()
        {
            Console.WriteLine("DEBUG - DTOR: ColumnsVisualization");
        }
    }
}
