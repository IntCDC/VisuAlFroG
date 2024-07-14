using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Core.GUI;
using System.Windows;
using System.Windows.Controls;
using System;
using SciChartInterface.Abstracts;
using Core.Utilities;
using SciChartInterface;
using System.Windows.Media;



/*
 * Visualization: Scatter Plot Matrices (SPLOM) (2D)
 * 
 */
namespace Visualizations
{
    public class SciChart_ScatterPlot : AbstractSciChartSeries<SciChartSurface, XyScatterRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Scatter Plot (SciChart)"; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public SciChart_ScatterPlot(int uid) : base(uid) { }

        #endregion
    }
}
