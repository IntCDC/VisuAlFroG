using System;
using System.Collections.Generic;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;
using System.Dynamic;
using System.Windows;
using SciChart.Charting.Visuals.Axes;
using System.Windows.Shapes;
using Core.Abstracts;
using Core.Data;
using System.ComponentModel;
using System.Windows.Controls;



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

            public DataTypeSciChartParallel(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler)
                : base(update_data_handler, update_metadata_handler) { }

            public override void UpdateData(GenericDataStructure data)
            {
                _loaded = false;
                _data = null;

                if ((data == null) || data.IsEmpty())
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }
                _Dimension = data.GetDimension();

                // Convert and create row data
                List<DataType> rows_list = new List<DataType>();
                create_data(data, ref rows_list);

                // Create property names for columns
                int index = 0;
                var tmp_row = rows_list[0] as IDictionary<string, object>;
                ParallelCoordinateDataItem<DataType, double>[] columns_list = new ParallelCoordinateDataItem<DataType, double>[tmp_row.Count];
                foreach (var kvp in tmp_row)
                {
                    string property_name = kvp.Key;
                    columns_list[index] = new ParallelCoordinateDataItem<DataType, double>(p =>
                        {
                            var p_dict = p as IDictionary<string, object>;
                            return (double)p_dict[property_name];
                        })
                    {
                        Title = kvp.Key,
                        AxisStyle = axes_style()
                    };
                    index++;
                    if (index >= tmp_row.Count)
                    {
                        break;
                    }
                }
                _data = new ParallelCoordinateDataSource<DataType>(columns_list);
                _data.SetValues(rows_list);

                _loaded = true;
            }

            public override void UpdateMetaDataEntry(IMetaData updated_meta_data)
            {
                if (!_loaded)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }

                /// TODO
            }

            /// <summary>
            ///  UNUSED XXX 
            /// </summary>
            /// <returns></returns>
            public override List<MenuItem> Menu()
            {
                /// TODO 
                return new List<MenuItem>();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void create_data(GenericDataStructure branch, ref List<DataType> value_list)
            {
                /// XXX TODO via filter
                int value_index = (int)branch.GetDimension() - 1;

                // For each branch add all entries to one row dictionary
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
                    create_data(b, ref value_list);
                }
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
                setter_boder_thickness.Value = new Thickness(1, 0, 0, 0);
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

            private string generate_property_name(int index)
            {
                return ("p" + index.ToString());
            }
        }

        #endregion
    }
}
