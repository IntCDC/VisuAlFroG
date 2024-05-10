using System;
using System.Collections.Generic;
using Core.Abstracts;
using System.ComponentModel;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;
using Core.Data;



/*
 *  SciChart data variety for fast lines
 * 
 */
using SciChartUniformDataType = SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double>;
using SciChartXYDataType = SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>;
using Core.GUI;
using SciChart.Charting.Visuals.PointMarkers;
using System.Windows;
using System.Windows.Media;
using SciChart.Charting.Model.DataSeries;
using System.Xml.Linq;

namespace SciChartInterface
{
    namespace Data
    {
        public class DataTypeSciChartSeries<DataType> : AbstractDataType<List<DataType>>
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override List<Dimension> _SupportedDimensions { get; }
                = new List<Dimension>() { Dimension.Uniform, Dimension.TwoDimensional };

            /// All numeric types that can be converted to double
            public sealed override List<Type> _SupportedValueTypes { get; }
                = new List<Type>() { typeof(double), typeof(float), typeof(int), typeof(uint), typeof(long), typeof(ulong) };


            /* ------------------------------------------------------------------*/
            // public functions

            public DataTypeSciChartSeries(PropertyChangedEventHandler update_metadata_handler, PropertyChangedEventHandler update_data_handler)
                : base(update_metadata_handler, update_data_handler) { }

            public override void UpdateData(GenericDataStructure data)
            {
                _loaded = false;
                if (_data != null)
                {
                    _data.Clear();
                }
                _data = null;

                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }

                if (!CompatibleDimensionality(data.Dimension()) || !CompatibleTypes(data.Types()))
                {
                    return;
                }

                if (_data == null)
                {
                    _data = new List<DataType>();
                }
                _data.Clear();

                // Convert and create required data
                init_data(data);
                if (_data.Count > 0)
                {
                    // Warn if series have different amount of values
                    int count = _data[0].DataSeries.Count;
                    for (int i = 1; i < _data.Count; i++)
                    {
                        if (count != _data[i].DataSeries.Count)
                        {
                            Log.Default.Msg(Log.Level.Warn, "Data series have different amount of values");
                            break;
                        }
                    }
                    _loaded = true;
                }
            }

            public override void UpdateMetaDataEntry(IMetaData updated_meta_data)
            {
                if (!_loaded)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }

                foreach (var data_series in _data)
                {
                    using (data_series.DataSeries.SuspendUpdates())
                    {
                        int series_count = data_series.DataSeries.Count;
                        for (int i = 0; i < series_count; i++)
                        {
                            if (updated_meta_data._Index == ((SciChartMetaData)data_series.DataSeries.Metadata[i])._Index)
                            {
                                ((SciChartMetaData)data_series.DataSeries.Metadata[i])._Selected = updated_meta_data._Selected;
                            }
                        }
                    }
                    data_series.InvalidateVisual();
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void init_data(GenericDataStructure branch)
            {
                // For each branch add all leafs to one data series
                if (branch._Entries.Count > 0)
                {
                    DataType data_series = new DataType();
                    data_series.AntiAliasing = true;
                    data_series.Style = renders_series_style();

                    if (branch.Dimension() == 1)
                    {
                        var series = new SciChartUniformDataType();
                        series.SeriesName = (branch._Label == "") ? (UniqueID.GenerateString()) : (branch._Label);

                        foreach (var entry in branch._Entries)
                        {
                            var meta_data = new SciChartMetaData(entry._Metadata._Index, entry._Metadata._Selected, _update_metadata_handler);
                            series.Append((double)entry._Values[0], meta_data);
                        }
                        data_series.DataSeries = series;

                    }
                    else if (branch.Dimension() == 2)
                    {
                        var series = new SciChartXYDataType();
                        series.SeriesName = (branch._Label == "") ? (UniqueID.GenerateString()) : (branch._Label);

                        foreach (var entry in branch._Entries)
                        {
                            var meta_data = new SciChartMetaData(entry._Metadata._Index, entry._Metadata._Selected, _update_metadata_handler);
                            ///series.AcceptsUnsortedData = true;
                            series.Append((double)entry._Values[0], (double)entry._Values[1], meta_data);
                        }
                        data_series.DataSeries = series;
                    }

                    _data.Add(data_series);
                }

                foreach (var b in branch._Branches)
                {
                    init_data(b);
                }
            }

            private System.Windows.Style renders_series_style()
            {
                var default_style = new System.Windows.Style();
                default_style.TargetType = typeof(DataType);

                var new_color = ColorTheme.RandomColor();

                if (typeof(DataType) == typeof(FastColumnRenderableSeries))
                {
                    Setter setter_stroke = new Setter();
                    setter_stroke.Property = FastColumnRenderableSeries.PaletteProviderProperty;
                    setter_stroke.Value = new StrokePalette();
                    default_style.Setters.Add(setter_stroke);

                    Setter setter_gradient = new Setter();
                    setter_gradient.Property = FastColumnRenderableSeries.FillProperty;
                    setter_gradient.Value = new SolidColorBrush(new_color); // gradient;
                    default_style.Setters.Add(setter_gradient);
                }
                else
                {
                    var pointmarker_default = new EllipsePointMarker()
                    {
                        Stroke = new_color,
                        Fill = new_color,
                        Width = 10.0,
                        Height = 10.0
                    };
                    var pointmarker_selected = new EllipsePointMarker()
                    {
                        StrokeThickness = 3,
                        Fill = new_color,
                        Width = 10.0,
                        Height = 10.0
                    };
                    pointmarker_selected.SetResourceReference(EllipsePointMarker.StrokeProperty, "Color_StrokeSelected");

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

                return default_style;
            }
        }
    }
}
