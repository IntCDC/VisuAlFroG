﻿using SciChart.Charting.Visuals;
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
    public class ScatterVisualization : AbstractSciChartSeries<SciChartSurface, XyScatterRenderableSeries>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Scatter Plot (2D)"; } }


        /* ------------------------------------------------------------------*/
        // public functions



        /// <summary>
        /// DEBUG
        /// </summary>
        ~ScatterVisualization()
        {
            Console.WriteLine("DEBUG - DTOR: ScatterVisualization");
        }
    }
}
