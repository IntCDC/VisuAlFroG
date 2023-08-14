﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using Core.Utilities;
using Core.Management;



/*
 * SciChart Library
 * 
 */
namespace Visualizations
{
    namespace SciChartInterface
    {

        public class SciChartInterfaceService : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions


            public override bool Initialize()
            {
                _timer.Start();

                // Set this code once in App.xaml.cs or at application startup
                /// Paste your SciChart runtime license key in SciChartInterface\SciChartRuntimeLicenseKey.cs
                SciChartSurface.SetRuntimeLicenseKey(SciChartRuntimeLicense.Key);

                _timer.Stop(this.GetType().FullName);
                _initilized = true;
                return _initilized;
            }


            //public void declare_scichart_surface(object sender, RoutedEventArgs routedEventArgs)
            public override bool Execute()
            {
                if (!_initilized)
                {
                    return false;
                }
                _timer.Start();

                // Create the chart surface
                var sciChartSurface = new SciChartSurface();

                // Create the X and Y Axis
                var xAxis = new NumericAxis() { AxisTitle = "Number of Samples (per series)" };
                var yAxis = new NumericAxis() { AxisTitle = "Value" };

                sciChartSurface.XAxis = xAxis;
                sciChartSurface.YAxis = yAxis;

                // Specify Interactivity Modifiers
                sciChartSurface.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier(), new SciChart.Charting.ChartModifiers.ZoomExtentsModifier());

                // Add annotation hints to the user
                var textAnnotation = new TextAnnotation()
                {
                    Text = "Hello World!",
                    X1 = 5.0,
                    Y1 = 5.0
                };
                sciChartSurface.Annotations.Add(textAnnotation);

                if (_grid != null)
                {
                    _grid.Children.Add(sciChartSurface);
                }
                else
                {
                    // ERROR
                }

                _timer.Stop(this.GetType().FullName);
                return true;
            }

            public void SetGrid(Grid grid) { _grid = grid; }


            /* ------------------------------------------------------------------*/
            // private variables

            private Grid _grid = null;

        }
    }
}
