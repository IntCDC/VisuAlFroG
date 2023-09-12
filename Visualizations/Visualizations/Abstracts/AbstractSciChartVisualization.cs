using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Core.Abstracts;
using Visualizations.SciChartInterface;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using Visualizations.Data;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartVisualization<SurfaceType, DataType> : AbstractVisualization
            where SurfaceType : SciChartSurface, new()
            where DataType : AbstractDataInterface, new()

        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return true; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected DataType DataInterface { get; set; }
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

                DataInterface = new DataType();
                DataInterface.RequestDataCallback = _request_callback;

                _content = new SurfaceType();
                _content.Name = ID;
                _content.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                _content.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);


                var clue_select = new MenuItem();
                clue_select.Header = "Select/Box-Select [Left Mouse]";
                clue_select.IsEnabled = false;

                var clue_zoom = new MenuItem();
                clue_zoom.Header = "Zoom [Mouse Wheel]";
                clue_zoom.IsEnabled = false;

                var clue_pan = new MenuItem();
                clue_pan.Header = "Pan [Right Mouse]";
                clue_pan.IsEnabled = false;

                var option_hint = new MenuItem();
                option_hint.Header = "Interaction Clues";
                option_hint.Items.Add(clue_select);
                option_hint.Items.Add(clue_zoom);
                option_hint.Items.Add(clue_pan);

                AddOption(option_hint);


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

                // Set data
                if (!DataInterface.Set(Content))
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to set data");
                }
                Content.ZoomExtents();

                _content.ChartModifier.IsAttached = true;

                AttachChildContent(_content);
                return base.Attach();
            }

            public sealed override bool Detach()
            {
                if (!_attached)
                {
                    /// TDOD Detach data, too?

                    // Required to release mouse handling
                    _content.ChartModifier.IsAttached = false;
                }
                return base.Detach();
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

            public override void UpdateCallback(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }
                // New data does not require any further update of the SciChart visualizations

                if (new_data)
                {
                    _content.ZoomExtents();
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SurfaceType _content = null;
        }
    }
}
