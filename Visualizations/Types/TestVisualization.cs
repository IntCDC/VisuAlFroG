using System;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using Core.Abstracts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Visualizations.SciChartInterface;



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
            // static variables

            public static readonly string name = "Test Visualization";
            public static readonly List<Type> depending_services = new List<Type>() { typeof(SciChartInterfaceService) };


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool AttachContent(Grid content_element)
            {
                if (!_setup)
                {
                    setup_content();
                }

                _content.ChartModifier.IsAttached = true;
                content_element.Children.Add(_content);

                _attached = true;
                return true;
            }


            public override bool DetachContent()
            {
                // Required to release mouse handling
                _content.ChartModifier.IsAttached = false;

                _attached = false;
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            protected override void setup_content()
            {
                // Create the chart surface
                _content = new SciChartSurface();

                // Create the X and Y Axis
                var xAxis = new NumericAxis() { AxisTitle = "Number of Samples (per series)" };
                var yAxis = new NumericAxis() { AxisTitle = "Value" };

                _content.XAxis = xAxis;
                _content.YAxis = yAxis;

                // Specify Interactivity Modifiers
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

                _setup = true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SciChartSurface _content = null;

            private readonly bool _multi = false;
        }
    }
}
