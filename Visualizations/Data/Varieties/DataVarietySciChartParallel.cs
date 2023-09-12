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

        public interface IPCPData
        {

            List<double> Values { get; set; }
        }

        public class GenericPCPData : IPCPData
        {

            public GenericPCPData() { }

            public List<double> Values { get; set; } = new List<double>();
        }



        public class DataVarietySciChartParallel<DataType> : AbstractDataVariety<ParallelCoordinateDataSource<DataType>>
            where DataType : IPCPData, new()
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


                var dyn_prop = new ExpandoObject();

                // Convert data
                List<DataType> value_list = new List<DataType>();
                convert_data(data, ref value_list);
                if (value_list.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing values...");
                    return;
                }

                /// TODO Require all value lists to have same amount of values?
                int item_count = int.MaxValue;
                foreach (var values in value_list)
                {
                    item_count = Math.Min(item_count, value_list[0].Values.Count - 1);
                }

                // Initialize data source
                ParallelCoordinateDataItem<DataType, double>[] item_list = new ParallelCoordinateDataItem<DataType, double>[item_count];
                for (int i = 0; i < item_count; i++)
                {
                    item_list[i] = new ParallelCoordinateDataItem<DataType, double>(p => p.Values.ElementAt(i))
                    {
                        Title = "P " + i.ToString(),
                        //AxisStyle = defaultAxisStyle
                    };
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



                _data.Invalidate();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void convert_data(GenericDataStructure branch, ref List<DataType> value_list)
            {
                // For each branch add all entries as one pcp value
                if (branch.Entries.Count > 0)
                {
                    DataType data_entry = new DataType();
                    data_entry.Values = new List<double>();

                    foreach (var entry in branch.Entries)
                    {
                        foreach (var value in entry.Values)
                        {
                            data_entry.Values.Add((double)value);
                        }
                    }
                    value_list.Add(data_entry);
                }

                foreach (var b in branch.Branches)
                {
                    convert_data(b, ref value_list);
                }
            }
        }
    }
}
