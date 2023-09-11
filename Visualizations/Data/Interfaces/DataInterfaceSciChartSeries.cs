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
using System.Runtime.Remoting.Contexts;
using SciChart.Core.Extensions;



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
                    ///data_series.PaletteProvider = new Selection_StrokePaletteProvider();

                    data_series.PointMarker = Selection_PointMarker.Default;
                    data_series.SelectedPointMarker = Selection_PointMarker.Selected;

                    data_series.Style = GetDataStyle();
                    parent.RenderableSeries.Add(data_series);
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected override Style GetDataStyle()
            {
                var default_style = new Style();
                default_style.TargetType = typeof(DataType);
                switch (DataStyle)
                {
                    case (DataStyles.Lines):
                        {
                            Setter setter_stroke = new Setter();
                            setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
                            var generator = new Random();
                            var color_value = generator.Next(1111, 9999).ToString();
                            var random_color = (Brush)new BrushConverter().ConvertFrom("#" + color_value);
                            Color color = random_color.ExtractColor();
                            setter_stroke.Value = color;
                            default_style.Setters.Add(setter_stroke);

                            Setter setter_thickness = new Setter();
                            setter_thickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
                            setter_thickness.Value = 3;
                            default_style.Setters.Add(setter_thickness);
                        }
                        break;
                    case (DataStyles.Columns):
                        {
                            /*
                            var gradient = new LinearGradientBrush();
                            gradient.StartPoint = new Point(0.0, 0.0);
                            gradient.EndPoint = new Point(1.0, 1.0);
                            gradient.GradientStops = new GradientStopCollection();
                            var gs1 = new GradientStop() { Color = Colors.Blue, Offset = 0.2 };
                            gradient.GradientStops.Add(gs1);
                            var gs2 = new GradientStop() { Color = Colors.Green, Offset = 0.8 };
                            gradient.GradientStops.Add(gs2);

                            Setter setter_gradient = new Setter();
                            /// FastColumnRenderableSeries specific ...
                            setter_gradient.Property = FastColumnRenderableSeries.FillProperty;
                            default_style.Setters.Add(setter_gradient);
                            */
                        }
                        break;
                    case (DataStyles.Points):
                        {
                            /*
                            Setter setter_datapoint = new Setter();
                            setter_datapoint.Property = BaseRenderableSeries.DataPointWidthProviderProperty;
                            setter_datapoint.Value = 0.5;
                            default_style.Setters.Add(setter_datapoint);
                            */
                        }
                        break;
                    default: break;
                }
                return default_style;
            }
        }
    }
}
