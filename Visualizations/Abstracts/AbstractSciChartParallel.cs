using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Core.Abstracts;
using Visualizations.Management;
using Visualizations.SciChartInterface;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Model.DataSeries;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartParallelCoordinateSurface.
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartParallel : AbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            // properties

            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public sealed override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _content = new SciChartParallelCoordinateSurface();
                _content.Name = ID;

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public sealed override Control Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                _content.ChartModifier.IsAttached = true;

                _attached = true;
                return _content;
            }

            public sealed override bool Detach()
            {
                // Required to release mouse handling
                _content.ChartModifier.IsAttached = false;

                return true;
            }

            public sealed override bool Terminate()
            {
                if (_initialized)
                {
                    _content.Dispose();
                    _content = null;

                    _initialized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected SciChartParallelCoordinateSurface _content = null;
        }
    }
}
