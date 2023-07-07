using System;
using System.Collections.Generic;
using System.Linq;
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


/*
 * SciChart Library
 * 
 */
namespace SciChartInterface
{
    /// <summary>
    /// Interaction logic for SciChart
    /// </summary>
    public class MainWindow
    {
        public void Init()
        {

            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("4Mp/qpwBPib0Le7wzWfBZmAPX00gKYqpDS0FqvAjKjmSICdOJlGdTZ80jjEcrJOOGyzf/LeYGe3oH5d4tlzDNxJ16qVEnvqlT1Ehp1iPdtllji5jJGsdity7sr3xP34cMh977mGctuqroj9bRahq5ZuTzxGu0Pd6WCA9J8HRf8wRs3DoycH84BG/v57WR263nu4zDJHo+apytqFWuNeMn/RG/FcAgFnwyFlDv53KsYUbuQYkMMXSLVqZRiPQHbvA8x9NFRFUPw6GGdd2bULHNkERdXQvg/Ch9OfA799bTuYNpyolyk6MgtNPCfNTE+jMdGHptMr0DM44NifQEXH0ijajMJHqKpETRlkKgJ4PI6IIPOR+brDr/3QDdISMp0o5Hrmn06BccE3ghuHka7pFSYvRKCfewT6YJIO2vHxOQkvK640GYnrqoQyMRp0V36iNpkl1HEtIqdT80LSMgrIwbidZBmSXCNpOO1FAezYOex/7UQ0Z9vzKxSuYqiE/JIEWOvahPn3AmqJT+TLXgZWIUIEWj6OQDw8ZqfFPER5Oq/EsKiUuM1daaMFhKorW6rF4MADVgBBzg2mJ0FiQ03mgTmNYv0yR");

        }

        //public void declare_scichart_surface(object sender, RoutedEventArgs routedEventArgs)
        public void DeclareSciChartSurface(ref Grid wpf_grid)
        {

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

            wpf_grid.Children.Add(sciChartSurface);
        }
    }
}
