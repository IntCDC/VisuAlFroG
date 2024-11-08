using System;
using System.Collections.Generic;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;
using System.Dynamic;
using Core.Abstracts;
using Core.Data;
using System.ComponentModel;
using System.Windows.Controls;
using SciChart.Core.Extensions;
using SciChart.Charting.Visuals.Axes;
using System.Windows;
using System.Windows.Shapes;



/*
 *  SciChart data variety for parallel coordinates 
 * 
 */
namespace SciChartInterface
{
    namespace Data
    {
        public class DataTypeSciChartParallel<DataType> : AbstractDataType<ParallelCoordinateDataSource<DataType>>
            where DataType : IDynamicMetaObjectProvider, new()
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public DataTypeSciChartParallel(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler, DataManager.GetSendOutputCallback_Delegate send_output_callback)
                : base(update_data_handler, update_metadata_handler, send_output_callback) { }

            public override void UpdateData(GenericDataStructure data)
            {
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }

                _data_specific = null;
                _data_generic = null;
                _loaded = false;


                _Dimension = data.GetDimension();
                _data_generic = data.DeepCopy();

                // Create value - property name pairs
                List<DataType> value_series = new List<DataType>();
                specific_data_conversion(data, ref value_series);
                if (value_series.IsEmpty())
                {
                    Log.Default.Msg(Log.Level.Error, "Missing values");
                    return;
                }

                // Create data item accessors for number each series
                int index = 0;
                var example_series = value_series[0] as IDictionary<string, object>;
                var value_count = example_series.Count;
                ParallelCoordinateDataItem<DataType, double>[] columns_list = new ParallelCoordinateDataItem<DataType, double>[value_count];
                foreach (var series in example_series)
                {
                    string property_name = series.Key;
                    columns_list[index] = new ParallelCoordinateDataItem<DataType, double>(p =>
                        {
                            var p_dict = p as IDictionary<string, object>;
                            return (double)p_dict[property_name];
                        })
                    {
                        Title = property_name,
                        AxisStyle = axes_style()
                    };

                    index++;
                    if (index >= value_count)
                    {
                        break;
                    }
                }

                try
                {
                    _data_specific = new ParallelCoordinateDataSource<DataType>(columns_list);
                    _data_specific.SetValues(value_series);
                    _loaded = true;
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
            }

            public override List<Control> GetDataMenu()
            {
                return base.GetDataMenu();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void specific_data_conversion(GenericDataStructure branch, ref List<DataType> value_list)
            {
                try
                {
                    // Always take first values of first dimension as default
                    // -> Adjustable via filter: Select value dimension from multi-dimensional data
                    // If changed, also change default mapping in respective filters!
                    int value_index = 0; // (int)branch.GetDimension() - 1;

                    // For each branch add all values to same series
                    if (branch._Entries.Count > 0)
                    {
                        dynamic data_entry = new DataType();
                        var data_entry_dict = data_entry as IDictionary<string, object>;
                        int index = 0;
                        foreach (var entry in branch._Entries)
                        {
                            string label = entry._Metadata._Label;
                            if (label == "")
                            {
                                label = generate_property_name(index);
                            }
                            data_entry_dict.Add(label, (double)entry._Values[value_index]);

                            index++;
                        }
                        value_list.Add(data_entry);
                    }

                    foreach (var b in branch._Branches)
                    {
                        specific_data_conversion(b, ref value_list);
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
            }

            private string generate_property_name(int index)
            {
                return ("p" + index.ToString());
            }

            private Style axes_style()
            {
                var axis_style = new System.Windows.Style();
                axis_style.TargetType = typeof(AxisBase);

                Setter setter_boder = new Setter();
                setter_boder.Property = AxisBase.BorderBrushProperty;
                setter_boder.Value = new DynamicResourceExtension("Brush_Foreground");
                axis_style.Setters.Add(setter_boder);

                Setter setter_boder_thickness = new Setter();
                setter_boder_thickness.Property = AxisBase.BorderThicknessProperty;
                setter_boder_thickness.Value = new Thickness(1, 0, 0, 0); // Left Top Right Bottom
                axis_style.Setters.Add(setter_boder_thickness);

                // Major Line
                Setter setter_major_line = new Setter();
                setter_major_line.Property = AxisBase.MajorTickLineStyleProperty;

                var line_style = new System.Windows.Style();
                line_style.TargetType = typeof(Line);

                Setter setter_stroke = new Setter();
                setter_stroke.Property = Line.StrokeProperty;
                setter_stroke.Value = new DynamicResourceExtension("Brush_Foreground");
                line_style.Setters.Add(setter_stroke);

                Setter setter_x2 = new Setter();
                setter_x2.Property = Line.X2Property;
                setter_x2.Value = 9.0;
                line_style.Setters.Add(setter_x2);

                setter_major_line.Value = line_style;
                axis_style.Setters.Add(setter_major_line);

                // Minor Line
                Setter setter_minor_line = new Setter();
                setter_minor_line.Property = AxisBase.MinorTickLineStyleProperty;

                line_style = new System.Windows.Style();
                line_style.TargetType = typeof(Line);

                setter_stroke = new Setter();
                setter_stroke.Property = Line.StrokeProperty;
                setter_stroke.Value = new DynamicResourceExtension("Brush_Foreground");
                line_style.Setters.Add(setter_stroke);

                setter_x2 = new Setter();
                setter_x2.Property = Line.X2Property;
                setter_x2.Value = 3.0;
                line_style.Setters.Add(setter_x2);

                setter_minor_line.Value = line_style;
                axis_style.Setters.Add(setter_minor_line);

                return axis_style;
            }

            #endregion
        }
    }
}
