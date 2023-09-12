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
                        {   var p_dict = p as IDictionary<string, object>; 
                            return (double)p_dict[property_name]; 
                        })
                    {
                        Title = kvp.Key
                        //AxisStyle = defaultAxisStyle
                    };
                    index++;
                    if (index >= item_count) {
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
                            data_entry_dict.Add(get_property_name(index), (double)value);
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


            /* ------------------------------------------------------------------*/
            // private functions

            private string get_property_name(int index)
            {
                return ("p" + index.ToString());
            }
        }
    }
}
