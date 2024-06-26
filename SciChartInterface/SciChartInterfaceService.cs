﻿using System;
using SciChart.Charting.Visuals;
using Core.Utilities;
using Core.Abstracts;



/*
 * SciChart Library
 * 
 */
namespace SciChartInterface
{
    public class SciChartInterfaceService : AbstractService
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public override bool Initialize()
        {
            if (_initialized)
            {
                Terminate();
            }

            // Paste your SciChart runtime license key in SciChartInterface\SciChartRuntimeLicenseKey.cs
            SciChartSurface.SetRuntimeLicenseKey(SciChartRuntimeLicense.Key);

            try
            {
                // Loading SciChartSurface for the first time takes some time to initialize.
                Log.Default.Msg(Log.Level.Debug, "Loading SciChartSurface ...");
                _timer.Start();

                var load_scichart = new SciChartSurface();

                _timer.Stop();
            }
            catch (Exception exc)
            {
                Log.Default.Msg(Log.Level.Error, exc.Message);
            }

            _initialized = true;
            if (_initialized)
            {
                Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
            }
            return _initialized;
        }

        public override bool Terminate()
        {
            if (_initialized)
            {
                _initialized = false;
            }
            return true;
        }

        #endregion
    }
}

