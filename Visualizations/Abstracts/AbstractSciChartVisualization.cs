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



/*
 * Abstract Visualization 
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartVisualization : AbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            // properties

            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            protected AbstractSciChartVisualization()
            {
                // Loading SciChartSurface for the first time takes some time ...
                Log.Default.Msg(Log.Level.Debug, "Loading SciChartSurface ...");
                _timer.Start();

                _content = new SciChartSurface();
                _content.Name = ID;

                _timer.Stop();
                Log.Default.Msg(Log.Level.Debug, "done.");
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


            /* ------------------------------------------------------------------*/
            // protected variables

            protected SciChartSurface _content = null;
        }
    }
}
