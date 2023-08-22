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



/*
 * Test Visualization
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class TestVisualization : AbstractVisualization
        {

            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Test Visualization"; } }
            public override List<Type> DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Create()
            {
                _timer.Start();

                _content.Name = "SciChartTest"; // no spaces allowed
                _content.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                _content.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);

                var render_series = new FastColumnRenderableSeries();
                render_series.Name = "column_series";
                render_series.DataPointWidth = 0.7;
                render_series.Stroke = Color.FromArgb(0xFF, 0xE4, 0xF5, 0xFC);

                var gradient = new LinearGradientBrush();
                gradient.StartPoint = new Point(0.0, 0.0);
                gradient.EndPoint = new Point(1.0, 1.0);
                gradient.GradientStops = new GradientStopCollection();
                var gs1 = new GradientStop() { Color = Colors.Yellow, Offset = 0.2 };
                gradient.GradientStops.Add(gs1);
                var gs2 = new GradientStop() { Color = Colors.Green, Offset = 0.8 };

                gradient.GradientStops.Add(gs2);
                render_series.Fill = gradient;

                var wa = new WaveAnimation();
                wa.AnimationDelay = new TimeSpan(0, 0, 0, 0, 200);
                wa.Duration = new TimeSpan(0, 0, 1);
                wa.PointDurationFraction = 0.2;
                render_series.SeriesAnimation = wa;

                _content.RenderableSeries.Add(render_series);

                var xAxis = new NumericAxis() { AxisTitle = "Sample No", DrawMajorBands = false };
                _content.XAxis = xAxis;

                var yAxis = new NumericAxis() { AxisTitle = "Value", GrowBy = new SciChart.Data.Model.DoubleRange(0, 0.1), DrawMajorBands = false };
                _content.YAxis = yAxis;

                _content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier(),
                    new SciChart.Charting.ChartModifiers.ZoomExtentsModifier());


                var textAnnotation = new TextAnnotation()
                {
                    Text = "TEST DUMMY...",
                    X1 = 5.0,
                    Y1 = 5.0

                };
                _content.Annotations.Add(textAnnotation);

                // Add test data

                var dataSeries = new UniformXyDataSeries<double> { SeriesName = "Histogram" };
                var yValues = new[] { 0.1, 0.2, 0.4, 0.8, 1.1, 1.5, 2.4, 4.6, 8.1, 11.7, 14.4, 16.0, 13.7, 10.1, 6.4, 3.5, 2.5, 1.4, 0.4, 0.1 };

                using (_content.SuspendUpdates())
                {
                    for (int i = 0; i < yValues.Length; i++)
                    {
                        // DataSeries for appending data
                        dataSeries.Append(yValues[i]);
                    }

                    render_series.DataSeries = dataSeries;
                }

                _content.ZoomExtents();


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

            private SciChartSurface _content = new SciChartSurface();
        }
    }
}
