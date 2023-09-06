using System;
using Core.Abstracts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Visualizations.SciChartInterface;
using System.Windows;
using Core.Utilities;
using System.Runtime.Remoting.Contexts;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Model.DataSeries;
using SciChart.Drawing;
using SciChart.Core;
using SciChart.Data;
using System.Windows.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.RenderableSeries.Animations;
using SciChart.Charting.Model.ChartSeries;
using System.ComponentModel;
using System.Linq;
using SciChart.Charting.Visuals.PointMarkers;
using System.Windows.Input;
using Visualizations.Abstracts;
using Visualizations.Interaction;
using Visualizations.Management;
using SciChart.Charting.ChartModifiers;
using SciChart.Core.Utility.Mouse;



/*
 * Visualization: Parallel Coordinates Plot (2D)
 * 
 */
namespace Visualizations
{
    namespace Types
    {

        ///  DEBUG
        public class SciChartPCPData_Type
        {
            public DateTime Date { get; set; }
            public double MinTemp { get; set; }
            public double MaxTemp { get; set; }
        }


        public class ParallelCoordinatesVisualization : AbstractSciChartVisualization<SciChartParallelCoordinateSurface, ParallelCoordinateDataSource<SciChartPCPData_Type>>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Parallel Coordinates Plot (2D)"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Create()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Info, "Skipping re-creation of content");
                    return true;
                }
                if (_request_data_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();

                Content.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                Content.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
                Content.ZoomExtents();
                Content.ChartTitle = "Parallel Coordinates Plot TEST";
                //Content.DrawSplines = true;


                // Data Series -------------------------------------

                /*
                render_series.DataSeries = Data();
                Content.RenderableSeries.Add(render_series);
                Content.ZoomExtents();
                */
                _pcp_source = new ParallelCoordinateDataSource<SciChartPCPData_Type>(
                    new ParallelCoordinateDataItem<SciChartPCPData_Type, DateTime>(p => p.Date)
                    {
                        Title = "Time",
                        //AxisStyle = defaultAxisStyle
                    },
                    new ParallelCoordinateDataItem<SciChartPCPData_Type, double>(p => p.MinTemp)
                    {
                        Title = "Min Temp",
                    },
                    new ParallelCoordinateDataItem<SciChartPCPData_Type, double>(p => p.MaxTemp)
                    {
                        Title = "Max Temp",
                    }
                );

                //_pcp_source.SetValues(...);
                Content.ParallelCoordinateDataSource = _pcp_source;

                /*
                var data = (SciChartUniformData_Type)_request_data_callback(typeof(SciChartUniformData_Type));
                if (data != null)
                {
                    render_series.DataSeries = data;
                }
                Content.RenderableSeries.Add(render_series);
                Content.ZoomExtents();
                */


                // Modifiers ---------------------------------------
                Content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier()
                    {
                        IsEnabled = false
                    },
                    new SciChart.Charting.ChartModifiers.ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    },
                    new SciChart.Charting.ChartModifiers.ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = SciChart.Charting.ChartModifiers.ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None
                    },
                    new SciChart.Charting.ChartModifiers.MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
                    },
                    new SciChart.Charting.ChartModifiers.DataPointSelectionModifier()
                    {
                        IsEnabled = true
                    }
                );


                _timer.Stop();
                _created = true;
                return _created;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void on_axis_reordered(object sender, ParallelAxisReorderArgs args)
            {
                _pcp_source?.ReorderItems(args.OldIndex, args.NewIndex);
            }


            /* ------------------------------------------------------------------*/
            // private variables


            private ParallelCoordinateDataSource<SciChartPCPData_Type> _pcp_source;

        }
    }
}
