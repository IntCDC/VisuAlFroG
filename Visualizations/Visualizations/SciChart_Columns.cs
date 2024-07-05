using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.GUI;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System;
using SciChartInterface.Abstracts;
using SciChartInterface;
using Core.Utilities;



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

        public override string _TypeName { get { return "Columns (SciChart)"; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public SciChart_Columns(int uid) : base(uid) { }

        #endregion
    }
}
