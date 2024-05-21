using System;
using System.Collections.Generic;
using Core.Abstracts;
using System.ComponentModel;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;
using Core.Data;
using Core.GUI;
using SciChart.Charting.Visuals.PointMarkers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;



/*
 *  SciChart data variety for fast lines
 * 
 */
namespace SciChartInterface
{
    namespace Data
    {
        public class DataTypeSciChartSeries<DataType> : AbstractDataType<List<DataType>>
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public DataTypeSciChartSeries(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler)
                : base(update_data_handler, update_metadata_handler) { }

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
                _Dimension = data.GetDimension();

                if (_data == null)
                {
                    _data = new List<DataType>();
                }
                _data.Clear();

                convert_data(data);
                _loaded = true;
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

            public override List<MenuItem> GetMenu()
            {
                var menu_items = new List<MenuItem>();

                for (uint a = 0; a < 2; a++)
                {
                    var menu_item = ContentMenuBar.GetDefaultMenuItem(((a == 0) ? ("[X]") : ("[Y]")) + " Axis Filter");
                    menu_items.Add(menu_item);

                    var radio_btn = new RadioButton();
                    radio_btn.Name = "radio_" + UniqueID.GenerateString();
                    radio_btn.Content = "Index";
                    radio_btn.Tag = new AxisFilterData(a, IndexIndex);
                    radio_btn.Checked += radio_btn_checked;
                    menu_item.Items.Add(radio_btn);

                    if (_axis_value_map.ContainsKey(a) && (_axis_value_map[a] == IndexIndex))
                    {
                        radio_btn.IsChecked = true;
                    }

                    for (uint d = 0; d < _Dimension; d++)
                    {
                        radio_btn = new RadioButton();
                        radio_btn.Name = "radio_" + UniqueID.GenerateString();
                        radio_btn.Content = "Value" + d.ToString();
                        radio_btn.Tag = new AxisFilterData(a, d); // Save index of values
                        radio_btn.Checked += radio_btn_checked;
                        menu_item.Items.Add(radio_btn);

                        if (_axis_value_map.ContainsKey(a) && (_axis_value_map[a] == d))
                        {
                            radio_btn.IsChecked = true;
                        }
                    }
                }
                return menu_items;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void convert_data(GenericDataStructure branch)
            {
                // For each branch add all leafs to one data series
                if (branch._Entries.Count > 0)
                {
                    var series = new SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>();
                    series.SeriesName = (branch._Label == "") ? (UniqueID.GenerateString()) : (branch._Label);
                    foreach (var entry in branch._Entries)
                    {
                        double x = double.NaN;
                        double y = double.NaN;
                        var dim = branch.GetDimension();
                        if (dim == 1)
                        {
                            x = (double)entry._Metadata._Index;
                            y = (double)entry._Values[0];
                            _axis_value_map[0] = IndexIndex;
                            _axis_value_map[1] = 0;
                        }
                        else if (dim == 2)
                        {
                            x = (double)entry._Values[0];
                            y = (double)entry._Values[1];
                            _axis_value_map[0] = 0;
                            _axis_value_map[1] = 1;
                            series.AcceptsUnsortedData = true; // XXX Can result in much slower performance for unsorted data
                        }
                        var meta_data = new SciChartMetaData(entry._Metadata._Index, entry._Metadata._Selected, _update_metadata_handler);
                        series.Append((double)entry._Values[0], (double)entry._Values[1], meta_data);
                    }

                    DataType data_series = new DataType();
                    data_series.AntiAliasing = true;
                    data_series.Style = renders_series_style();
                    data_series.DataSeries = series;
                    _data.Add(data_series);
                }

                foreach (var b in branch._Branches)
                {
                    convert_data(b);
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
                    setter_gradient.Value = new SolidColorBrush(new_color);
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

            private void radio_btn_checked(object sender, RoutedEventArgs e)
            {
                var sender_selection = sender as RadioButton;
                if (sender_selection == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown sender");
                    return;
                }

                var data = sender_selection.Tag as AxisFilterData;
                if (data != null)
                {
                    Log.Default.Msg(Log.Level.Warn, "Selected values at index " + data.value_index.ToString() + " for axis " + data.axis_index.ToString());
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to get data from radio button");
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            public class AxisFilterData
            {
                public AxisFilterData(uint ai, uint vi) { axis_index = ai; value_index = vi; }
                public uint axis_index;
                public uint value_index;
            }

            private Dictionary<uint, uint> _axis_value_map = new Dictionary<uint, uint>();

            private const uint IndexIndex = uint.MaxValue;

            #endregion
        }
    }
}
