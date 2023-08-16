using System.Windows.Controls;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using Core.Utilities;
using Core.Abstracts;



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

                _timer.Stop();
                _initilized = true;
                return _initilized;
            }


            //public void declare_scichart_surface(object sender, RoutedEventArgs routedEventArgs)
            public override bool Execute()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();






                _timer.Stop();
                return true;
            }

            /* ------------------------------------------------------------------*/
            // private variables


        }
    }
}
