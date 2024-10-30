using System.Windows.Controls;
using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Core.GUI;
using System;
using SciChartInterface.Abstracts;
using System.Windows.Markup;



/*
 * Visualization: Lines (2D)
 * 
 */
namespace Visualizations
{
    public class SciChart_LinesPlot : AbstractSciChartSeries<SciChartSurface, FastLineRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Lines Plot (SciChart)"; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public SciChart_LinesPlot(int uid) : base(uid) { }

        #endregion
    }
}
