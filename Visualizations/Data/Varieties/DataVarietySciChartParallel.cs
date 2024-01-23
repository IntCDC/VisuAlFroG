using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Data;
using Visualizations.Abstracts;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;
using Visualizations.Varieties;
using System.Dynamic;
using System.Windows.Controls;
using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using System.Windows.Media;
using System.Windows.Shapes;
using Core.GUI;



/*
 *  SciChart data variety for parallel coordinates 
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataVarietySciChartParallel<DataType> : AbstractDataVariety<ParallelCoordinateDataSource<DataType>>
            where DataType : IDynamicMetaObjectProvider, new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override List<Dimension> SupportedDimensions
            {
                get
                {
                    return new List<Dimension>() { Dimension.Uniform, Dimension.TwoDimensional, Dimension.ThreeDimensional, Dimension.Multidimensional };
                }
            }

            /// All numeric types that can be converted to double
            public sealed override List<Type> SupportedValueTypes
            {
                get
                {
                    return new List<Type>() { typeof(double), typeof(float), typeof(int), typeof(uint), typeof(long), typeof(ulong) };
                }
            }


            /* ------------------------------------------------------------------*/
            // public functions

            public override void Create(ref GenericDataStructure data, int data_dimension, List<Type> value_types)
            {
                _created = false;
                if (!CompatibleDimensionality(data_dimension) || !CompatibleValueTypes(value_types))
                {
                    return;
                }
                if (data == null)
                {
                    return;
                }

                dynamic tmp = new ExpandoObject();

                // Convert data
                List<DataType> value_list = new List<DataType>();
                convert_data(data, ref value_list);
                if (value_list.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing values...");
                    return;
                }

                // Warn if series have different amount of values
                var value_dict = value_list[0] as IDictionary<string, object>;
                int item_count = value_dict.Count;

                for (int i = 1; i < value_list.Count; i++)
                {
                    value_dict = value_list[i] as IDictionary<string, object>;
                    if (item_count != value_dict.Count)
                    {
                        Log.Default.Msg(Log.Level.Warn, "Data series have different amount of values");
                    }
                    item_count = Math.Min(item_count, value_dict.Count);
                }

                ParallelCoordinateDataItem<DataType, double>[] item_list = new ParallelCoordinateDataItem<DataType, double>[item_count];
                // Initialize data source
                int index = 0;
                foreach (var kvp in value_dict)
                {
                    string property_name = kvp.Key;
                    item_list[index] = new ParallelCoordinateDataItem<DataType, double>(p =>
                        {
                            var p_dict = p as IDictionary<string, object>;
                            return (double)p_dict[property_name];
                        })
                    {
                        Title = kvp.Key,
                        AxisStyle = axes_style()
                    };
                    index++;
                    if (index >= item_count)
                    {
                        break;
                    }
                }
                _data = new ParallelCoordinateDataSource<DataType>(item_list);
                _data.SetValues(value_list);

                _created = true;
            }

            public override void UpdateEntryAtIndex(GenericDataEntry updated_entry)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }

                ///_data.Invalidate();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="branch"></param>
            /// <param name="value_list"></param>
            private void convert_data(GenericDataStructure branch, ref List<DataType> value_list)
            {
                // For each branch add all entries as one pcp value
                if (branch.Entries.Count > 0)
                {
                    dynamic data_entry = new DataType();
                    var data_entry_dict = data_entry as IDictionary<string, object>;
                    int index = 0;
                    foreach (var entry in branch.Entries)
                    {
                        foreach (var value in entry.Values)
                        {
                            /// TODO The property names are hard coded and should be replaced by names provided in the data set (e.g. branch name?)
                            data_entry_dict.Add(generate_property_name(index), (double)value);
                        }
                        index++;
                    }
                    value_list.Add(data_entry);
                }

                foreach (var b in branch.Branches)
                {
                    convert_data(b, ref value_list);
                }
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <returns></returns>
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

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            private string generate_property_name(int index)
            {
                return ("p" + index.ToString());
            }
        }
    }
}
