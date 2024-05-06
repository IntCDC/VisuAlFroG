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
    public class ColumnsVisualization : AbstractSciChartSeries<SciChartSurface, FastColumnRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Columns (1D)"; } }


        /* ------------------------------------------------------------------*/
        // public functions



        /// <summary>
        /// DEBUG
        /// </summary>
        ~ColumnsVisualization()
        {
            Console.WriteLine("DEBUG - DTOR: ColumnsVisualization");
        }
    }
}
