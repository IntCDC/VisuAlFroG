using System.Windows.Controls;
using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using Core.GUI;
using System;
using SciChartInterface.Abstracts;
using Core.Utilities;
using SciChartInterface;
using System.Windows.Media;



/*
 * Visualization: Lines (2D)
 * 
 */
namespace Visualizations
{
    public class SciChart_Lines : AbstractSciChartSeries<SciChartSurface, FastLineRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Lines (SciChart)"; } }


        /* ------------------------------------------------------------------*/
        // public functions



    }
}
