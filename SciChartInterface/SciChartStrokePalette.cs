using System.Windows;
using System.Windows.Media;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PaletteProviders;
using SciChart.Charting.Visuals.RenderableSeries;



/*
 * SciChart specific stroke palette for column plots
 * 
 */

namespace SciChartInterface
{
    public class StrokePalette : IStrokePaletteProvider
    {
        /* ------------------------------------------------------------------*/
        // public functions

        /// <summary>
        /// Callback called when drawing of series begins.
        /// </summary>
        /// <param name="rSeries">The renderable series.</param>
        public void OnBeginSeriesDraw(IRenderableSeries rSeries) { }

        /// <summary>
        /// Callback called when strike color should be overridden.
        /// </summary>
        /// <param name="rSeries">The renderable series.</param>
        /// <param name="index">The index of the data point.</param>
        /// <param name="meta_data">The meta data of the data point.</param
        /// <returns></returns>
        public Color? OverrideStrokeColor(IRenderableSeries rSeries, int index, IPointMetadata meta_data)
        {
            var color_name = "Color_StrokeDefault";
            if ((meta_data != null) && (meta_data.IsSelected)) {
                color_name = "Color_StrokeSelected";
            }
            /// XXX This is not working because DynamicResourceExtension does not reference 'hard' object ...?
            return null; // (Color)new DynamicResourceExtension(color_name);
        }
    }
}
