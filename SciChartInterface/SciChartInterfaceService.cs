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

                /// TODO Paste your SciChart runtime license key in SciChartInterface\SciChartRuntimeLicenseKey.cs
                SciChartSurface.SetRuntimeLicenseKey(SciChartRuntimeLicense.Key);

                _timer.Stop();
                _initilized = true;
                return _initilized;
            }


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
