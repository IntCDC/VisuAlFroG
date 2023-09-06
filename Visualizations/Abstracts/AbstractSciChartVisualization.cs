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
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartVisualization<SurfaceType, DataType> : AbstractVisualization where SurfaceType : SciChartSurface, new()
        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return true; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected SurfaceType Content { get { return _content; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _content = new SurfaceType();
                _content.Name = ID;

                ContentChild.Children.Add(_content);

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public sealed override Panel Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }
                _content.ChartModifier.IsAttached = true;

                return base.Attach();
            }

            public sealed override bool Detach()
            {
                // Required to release mouse handling
                _content.ChartModifier.IsAttached = false;

                return true;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content.Dispose();
                    _content = null;

                    _initialized = false;
                }
                return base.Terminate();
            }

            public new DataType Data()
            {
                return (DataType)_request_data_callback(typeof(DataType));
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SurfaceType _content = null;
        }
    }
}
