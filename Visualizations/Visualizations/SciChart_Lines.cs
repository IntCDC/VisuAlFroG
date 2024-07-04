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
    public class SciChart_Lines : AbstractSciChartSeries<SciChartSurface, FastLineRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _TypeName { get { return "Lines (SciChart)"; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public SciChart_Lines(string uid) : base(uid) { }

        #endregion
    }
}
