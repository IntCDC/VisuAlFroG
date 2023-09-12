using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Core.Utilities;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals;
using Visualizations.Abstracts;
using System.Windows.Controls;
using System.Xml.Linq;
using Core.GUI;
using System.Runtime.Remoting.Contexts;
using SciChart.Core.Extensions;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PaletteProviders;



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
                    data_series.Style = ApplyStyle();
                    
                    parent.RenderableSeries.Add(data_series);
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected override Style ApplyStyle()
            {
                var default_style = new Style();
                default_style.TargetType = typeof(DataType);

                var new_color = random_color();

                var pointmarker_default = new EllipsePointMarker()
                {
                    Stroke = new_color,
                    Fill = new_color,
                    Width = 10.0,
                    Height = 10.0
                };

                var pointmarker_selected = new EllipsePointMarker()
                {
                    Stroke = ColorTheme.StrokeSelected,
                    StrokeThickness = 3,
                    Fill = new_color,
                    Width = 10.0,
                    Height = 10.0
                };

                switch (DataStyle)
                {
                    case (DataStyles.Lines):
                        {
                            Setter setter_stroke = new Setter();
                            setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
                            setter_stroke.Value = new_color;
                            default_style.Setters.Add(setter_stroke);

                            Setter setter_thickness = new Setter();
                            setter_thickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
                            setter_thickness.Value = 3;
                            default_style.Setters.Add(setter_thickness);

                            Setter setter_point = new Setter();
                            setter_point.Property = BaseRenderableSeries.PointMarkerProperty;
                            setter_point.Value = pointmarker_default;
                            default_style.Setters.Add(setter_point);

                            Setter setter_point_selected = new Setter();
                            setter_point_selected.Property = BaseRenderableSeries.SelectedPointMarkerProperty;
                            setter_point_selected.Value = pointmarker_selected;
                            default_style.Setters.Add(setter_point_selected);
                        }
                        break;
                    case (DataStyles.Columns):
                        {
                            Setter setter_stroke = new Setter();
                            setter_stroke.Property = BaseRenderableSeries.PaletteProviderProperty;
                            setter_stroke.Value = new StrokePalette();
                            default_style.Setters.Add(setter_stroke);

                            /*var gradient = new LinearGradientBrush();
                            gradient.StartPoint = new Point(0.0, 0.0);
                            gradient.EndPoint = new Point(1.0, 1.0);
                            gradient.GradientStops = new GradientStopCollection();
                            var gs1 = new GradientStop() { Color = Colors.Blue, Offset = 0.2 };
                            gradient.GradientStops.Add(gs1);
                            var gs2 = new GradientStop() { Color = Colors.Green, Offset = 0.8 };
                            gradient.GradientStops.Add(gs2);*/
                            
                            Setter setter_gradient = new Setter();
                            setter_gradient.Property = FastColumnRenderableSeries.FillProperty;
                            setter_gradient.Value =  new SolidColorBrush(random_color()); // gradient;
                            default_style.Setters.Add(setter_gradient);
                        }
                        break;
                    case (DataStyles.Points):
                        {
                            Setter setter_point = new Setter();
                            setter_point.Property = BaseRenderableSeries.PointMarkerProperty;
                            setter_point.Value = pointmarker_default;
                            default_style.Setters.Add(setter_point);

                            Setter setter_point_selected = new Setter();
                            setter_point_selected.Property = BaseRenderableSeries.SelectedPointMarkerProperty;
                            setter_point_selected.Value = pointmarker_selected;
                            default_style.Setters.Add(setter_point_selected);
                        }
                        break;
                    default: break;
                }
                return default_style;
            }

            /* ------------------------------------------------------------------*/
            // private functions

            private Color random_color()
            {
                byte alpha = 0xAF;
                int size = 2048; // in bytes
                byte[] b = new byte[size];
                _random.NextBytes(b);
                var r_index = _random.Next(0, size - 1);
                var g_index = _random.Next(0, size - 1);
                var b_index = _random.Next(0, size - 1);
                var r_byte = 0x0F | b[r_index];
                var g_byte = 0x0F | b[g_index];
                var b_byte = 0x0F | b[b_index];
                return Color.FromArgb(alpha, (byte)r_byte, (byte)g_byte, (byte)b_byte);
            }


            /* ------------------------------------------------------------------*/
            // private classes

            /// <summary>
            /// SciChart specific stroke palette for column plots.
            /// </summary>
            private class StrokePalette : IStrokePaletteProvider
            {
                /* ------------------------------------------------------------------*/
                // public functions

                /// <summary>
                /// Callback called when drawing of series begins.
                /// </summary>
                /// <param name="rSeries">The renderable series.</param>
                public void OnBeginSeriesDraw(IRenderableSeries rSeries) { }

                /// <summary>
                /// Callback called when strike color should be overridden.
                /// </summary>
                /// <param name="rSeries">The renderable series.</param>
                /// <param name="index">The index of the data point.</param>
                /// <param name="meta_data">The meta data of the data point.</param>
                /// <returns></returns>
                public Color? OverrideStrokeColor(IRenderableSeries rSeries, int index, IPointMetadata meta_data)
                {
                    return ((meta_data != null) && (meta_data.IsSelected)) ? ColorTheme.StrokeSelected : ColorTheme.StrokeDefault;
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Random _random = new Random();
        }
    }
}
