using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Abstracts;


/*
 * Visualization: Columns (Bar Chart) (2D)
 * 
 */
namespace Visualizations
{
    public class SciChart_Columns : AbstractSciChartSeries<SciChartSurface, FastColumnRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Columns (SciChart)"; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public SciChart_Columns(int uid) : base(uid) { }

        #endregion
    }
}
