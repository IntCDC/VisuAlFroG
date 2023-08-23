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
        /// SciChart specific
        /// </summary>
        public class Selection_StrokePaletteProvider : IStrokePaletteProvider
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public void OnBeginSeriesDraw(IRenderableSeries rSeries) { }


            public Color? OverrideStrokeColor(IRenderableSeries rSeries, int index, IPointMetadata metadata)
            {
                return ((metadata != null) && (metadata.IsSelected)) ? _selected_stroke : _default_stroke;
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private readonly Color _selected_stroke = Colors.Red;
            private readonly Color _default_stroke = Colors.White;
        }



        /// <summary>
        /// SciChart specific
        /// </summary>
        public class Selection_PointMarker
        {
            /* ------------------------------------------------------------------*/
            // static properties

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

            public static SciChart.Charting.Visuals.PointMarkers.EllipsePointMarker Selected
            {
                get
                {
                    return new EllipsePointMarker()
                    {
                        Stroke = Colors.Red,
                        StrokeThickness = 5,
                        Fill = Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                        Width = 15.0,
                        Height = 15.0
                    };
                }
            }
        }
    }
}
