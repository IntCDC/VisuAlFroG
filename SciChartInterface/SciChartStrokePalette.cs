using System.Windows;
using System.Windows.Media;

using Core.Utilities;

using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PaletteProviders;
using SciChart.Charting.Visuals.RenderableSeries;



/*
 * SciChart specific stroke palette for column plots
 * 
 */

namespace SciChartInterface
{
    public class SciChart_StrokePalette : IStrokePaletteProvider
    {
        /* ------------------------------------------------------------------*/
        #region public functions

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
            var brush_default = Application.Current.Resources["Brush_StrokeDefault"] as SolidColorBrush;
            var brush_selected = Application.Current.Resources["Brush_StrokeSelected"] as SolidColorBrush;

            if ((brush_default == null) || (brush_selected == null))
            {
                Log.Default.Msg(Log.Level.Error, "Unable to find resource for stroke color(s).");
                return Colors.White;
            }
            else
            {
                return ((meta_data.IsSelected) ? (brush_selected.Color) : (brush_default.Color));
            }
        }

        #endregion
    }
}
