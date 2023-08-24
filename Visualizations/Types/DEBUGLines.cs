﻿using System;
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



using SciChartData_Type = SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double>;
using SciChart.Data.Model;


/*
 * Test Visualization
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class DEBUGLines : AbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "DEBUG Lines"; } }
            public override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Create()
            {
                if (_request_data_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();

                // Loading SciChartSurface for the first time takes some time ...
                Log.Default.Msg(Log.Level.Debug, "Loading SciChartSurface ...");
                _content = new SciChartSurface();
                _content.Name = "SciChart_Lines_Test" + ID; // no spaces allowed
                _content.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                _content.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
                _content.ZoomExtents();


                // Data Series -------------------------------------
                var render_series = new FastLineRenderableSeries();
                render_series.Name = "line_series";
                render_series.Stroke = Colors.Aquamarine;
                render_series.StrokeThickness = 2;
                render_series.PointMarker = Selection_PointMarker.Default;
                render_series.SelectedPointMarker = Selection_PointMarker.Selected;

                /*
                var wa = new WaveAnimation();
                wa.AnimationDelay = new TimeSpan(0, 0, 0, 0, 200);
                wa.Duration = new TimeSpan(0, 0, 1);
                wa.PointDurationFraction = 0.2;
                render_series.SeriesAnimation = wa;
                */

                render_series.DataSeries = (SciChartData_Type)_request_data_callback(DataManager.Libraries.SciChart);

                _content.RenderableSeries.Add(render_series);


                // Axis --------------------------------------------
                var xAxis = new NumericAxis()
                {
                    AxisTitle = "Sample No",
                    DrawMajorBands = false
                };
                _content.XAxis = xAxis;

                var yAxis = new NumericAxis()
                {
                    AxisTitle = "Value",
                    GrowBy = new SciChart.Data.Model.DoubleRange(0.2, 0.2),
                    DrawMajorBands = false,
                };
                _content.YAxis = yAxis;


                // Modifiers ---------------------------------------
                var data_selection = new SciChart.Charting.ChartModifiers.DataPointSelectionModifier()
                {
                    Name = "PointMarkersSelectionModifier",
                    IsEnabled = true
                };
                var wheel_zoom = new SciChart.Charting.ChartModifiers.MouseWheelZoomModifier()
                {
                    ActionType = SciChart.Charting.ActionType.Zoom,
                    XyDirection = SciChart.Charting.XyDirection.XYDirection
                };
                var right_pan = new SciChart.Charting.ChartModifiers.ZoomPanModifier()
                {
                    ExecuteOn = SciChart.Charting.ChartModifiers.ExecuteOn.MouseRightButton,
                    ClipModeX = SciChart.Charting.ClipMode.None
                };

                _content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    //new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier(),
                    //new SciChart.Charting.ChartModifiers.ZoomExtentsModifier(),
                    data_selection,
                    wheel_zoom,
                    right_pan
                    );


                // Annotation --------------------------------------
                var textAnnotation = new TextAnnotation()
                {
                    Text = "Interaction:" + Environment.NewLine +
                            " Left Mouse -> Select / Box-Select" + Environment.NewLine +
                            "Mouse Wheel -> Zoom" + Environment.NewLine +
                            "Right Mouse -> Pan",
                    X1 = 2.0,
                    Y1 = 3.0
                };
                _content.Annotations.Add(textAnnotation);


                _timer.Stop();
                _created = true;
                return _created;
            }


            public override bool Attach(Grid content_element)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return false;
                }

                _content.ChartModifier.IsAttached = true;
                content_element.Children.Add(_content);

                _attached = true;
                return true;
            }


            public override bool Detach()
            {
                // Required to release mouse handling
                _content.ChartModifier.IsAttached = false;

                return true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private SciChartSurface _content = null;
        }
    }
}
