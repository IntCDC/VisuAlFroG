using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Core.Utilities;
using SciChart.Charting.Visuals.RenderableSeries;
using Visualizations.Interaction;
using SciChart.Charting.Visuals;
using Visualizations.Abstracts;
using System.Windows.Controls;
using System.Xml.Linq;
using Core.GUI;



/*
 * SciChart renderable series interface
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataInterfaceSciChartSeries<DataType> : AbstractDataInterface
            where DataType : BaseRenderableSeries
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Set(object data_parent)
            {
                var parent = data_parent as SciChartSurface;
                if (parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Can not convert data parent parameter to required type");
                    return false;
                }

                var data = (List<DataType>)RequestDataCallback(typeof(List<DataType>));
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                    return false;
                }
                if (data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return false;
                }
                foreach (var data_series in data)
                {
                    data_series.Name = UniqueID.Generate();
                    data_series.PaletteProvider = new Selection_StrokePaletteProvider();
                    data_series.PointMarker = Selection_PointMarker.Default;
                    data_series.SelectedPointMarker = Selection_PointMarker.Selected;

                    /// TODO Allow more flexible styling 

                    data_series.Stroke = Colors.Aquamarine;
                    data_series.StrokeThickness = 2;

                    var fcrs = data_series as FastColumnRenderableSeries;
                    if (fcrs != null)
                    {
                        fcrs.DataPointWidth = 0.5;

                        var gradient = new LinearGradientBrush();
                        gradient.StartPoint = new Point(0.0, 0.0);
                        gradient.EndPoint = new Point(1.0, 1.0);
                        gradient.GradientStops = new GradientStopCollection();
                        var gs1 = new GradientStop() { Color = Colors.Blue, Offset = 0.2 };
                        gradient.GradientStops.Add(gs1);
                        var gs2 = new GradientStop() { Color = Colors.Green, Offset = 0.8 };
                        gradient.GradientStops.Add(gs2);
                        fcrs.Fill = gradient;
                    }

                    parent.RenderableSeries.Add(data_series);
                }
                return true;
            }
        }
    }
}
