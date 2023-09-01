using System;
using System.Windows.Controls;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using Core.Utilities;
using Core.Abstracts;
using System.Runtime.Remoting.Contexts;



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
                if (_initilized)
                {
                    Terminate();
                }

                // Paste your SciChart runtime license key in SciChartInterface\SciChartRuntimeLicenseKey.cs
                SciChartSurface.SetRuntimeLicenseKey(SciChartRuntimeLicense.Key);
                try
                {
                    // Loading SciChartSurface for the first time takes some time to initilaize.
                    Log.Default.Msg(Log.Level.Debug, "Loading SciChartSurface ...");
                    _timer.Start();

                    var load_scichart = new SciChartSurface();

                    _timer.Stop();
                    Log.Default.Msg(Log.Level.Debug, "... done.");
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }

                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initilized;
            }


            public override bool Terminate()
            {
                if (_initilized)
                {
                    _initilized = false;
                }
                return true;
            }
        }
    }
}
