using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PaletteProviders;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;



/*
 * Selection (SciChart)
 * 
 */
namespace Visualizations
{
    namespace Interaction
    {
        /// <summary>
        /// SciChart specific stroke palette for column plots.
        /// </summary>
        public class Selection_StrokePaletteProvider : IStrokePaletteProvider
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
            /// <param name="meta_data">The meta data of the data point.</param>
            /// <returns></returns>
            public Color? OverrideStrokeColor(IRenderableSeries rSeries, int index, IPointMetadata meta_data)
            {
                return ((meta_data != null) && (meta_data.IsSelected)) ? _selected_stroke : _default_stroke;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly Color _selected_stroke = Colors.Red;
            private readonly Color _default_stroke = Colors.White;
        }


        /// <summary>
        /// SciChart specific Point Marker.
        /// </summary>
        public class Selection_PointMarker
        {
            /* ------------------------------------------------------------------*/
            // static properties

            /// <summary>
            /// [STATIC] Default point marker for lines series.
            /// </summary>
            public static SciChart.Charting.Visuals.PointMarkers.EllipsePointMarker Default
            {
                get
                {
                    return new EllipsePointMarker()
                    {
                        Fill = Colors.Aquamarine,
                        Width = 5.0,
                        Height = 5.0
                    };
                }
            }

            /// <summary>
            /// [STATIC] Point marker for selected data points for lines series.
            /// </summary>
            public static SciChart.Charting.Visuals.PointMarkers.EllipsePointMarker Selected
            {
                get
                {
                    return new EllipsePointMarker()
                    {
                        Stroke = Colors.SteelBlue,
                        StrokeThickness = 5,
                        Fill = Colors.LightSteelBlue, // Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                        Width = 10.0,
                        Height = 10.0
                    };
                }
            }
        }
    }
}
