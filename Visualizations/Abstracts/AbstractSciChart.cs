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
 * Abstract Visualization 
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractSciChart : AbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            // properties

            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public sealed override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                _content = new SciChartSurface();
                _content.Name = ID;

                _timer.Stop();
                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
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
                if (_initilized)
                {
                    _content.Dispose();
                    _content = null;

                    _initilized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected SciChartSurface _content = null;
        }
    }
}
