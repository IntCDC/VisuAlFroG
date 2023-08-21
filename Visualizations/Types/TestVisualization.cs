using System;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using Core.Abstracts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Visualizations.SciChartInterface;
using System.Windows;
using Core.Utilities;
using System.Runtime.Remoting.Contexts;



/*
 * Test Visualization
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class TestVisualization : AbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Test Visualization"; } }
            public override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }
            

            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Attach(Grid content_element)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return false;
                }

                _content.ChartModifier.IsAttached = true;

                content_element.Children.Add(_content);
                _attached = true;
                return true;
            }


            public override bool Detach()
            {
                // Required to release mouse handling
                _content.ChartModifier.IsAttached = false;
                return true;
            }


            public override bool Create()
            {
                _timer.Start();

                var xAxis = new NumericAxis() { AxisTitle = "Number of Samples (per series)" };
                var yAxis = new NumericAxis() { AxisTitle = "Value" };
                _content.XAxis = xAxis;
                _content.YAxis = yAxis;

                _content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier(), new SciChart.Charting.ChartModifiers.ZoomExtentsModifier());

                // Add annotation hints to the user
                var textAnnotation = new TextAnnotation()
                {
                    Text = "Hello World!",
                    X1 = 5.0,
                    Y1 = 5.0
                };
                _content.Annotations.Add(textAnnotation);

                _timer.Stop();

                _created = true;
                return _created;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SciChartSurface _content = new SciChartSurface();
        }
    }
}
