using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using Core.Abstracts;
using System.Windows.Controls;
using System.Windows.Media;



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
            // public functions

            public TestVisualization() : base("Test Visualization")
            {
                setup_content();
            }


            public override bool AttachContent(Grid content_element)
            {
                _content.ChartModifier.IsAttached = true;

                content_element.Children.Add(_content);

                _attached = true;
                return true;
            }

            public override bool DetachContent()
            {
                _content.ChartModifier.IsAttached = false;

                _attached = false;
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void setup_content()
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
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private SciChartSurface _content = null;

        }
    }
}
