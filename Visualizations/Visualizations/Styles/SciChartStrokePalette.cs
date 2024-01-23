using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Core.GUI;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PaletteProviders;
using SciChart.Charting.Visuals.RenderableSeries;



/*
 * SciChart specific stroke palette for column plots
 * 
 */
namespace Visualizations
{
    namespace Styles
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
                object Color_StrokeSelected = new DynamicResourceExtension("Color_StrokeSelected");
                object Color_StrokeDefault = new DynamicResourceExtension("Color_StrokeDefault");
                return ((meta_data != null) && (meta_data.IsSelected)) ? (Color)Color_StrokeSelected : (Color)Color_StrokeDefault;
            }
        }
    }
}
