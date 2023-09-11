using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Data;
using Visualizations.Abstracts;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;



/*
 *  SciChart data variety for fast lines
 * 
 */
using SciChartUniformDataType = SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double>;
using SciChartXYDataType = SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>;
using System.Windows.Media;
using System.Windows;
using Visualizations.Interaction;

namespace Visualizations
{
    namespace Data
    {
        public class DataVarietySciChartSeries<DataType> : AbstractDataVariety<List<DataType>>
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override DataDimensionality SupportedDimensionality { get { return (DataDimensionality.Uniform | DataDimensionality.TwoDimensional); } }

            /// All numeric types that can be converted to double
            public sealed override List<Type> SupportedValueTypes { get { return new List<Type>() { typeof(double), typeof(float), typeof(int), typeof(uint), typeof(long), typeof(ulong) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override void Update(ref GenericDataStructure data, DataDimensionality dimensionality, List<Type> value_types)
            {
                if (!CompatibleDimensionality(dimensionality))
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible data dimensionality");
                    return;
                }
                if (!CompatibleValueTypes(value_types) || (value_types.Count > 1))
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible data value types");
                    return;
                }

                add_series(data, dimensionality);
            }

            public override void UpdateEntryAtIndex(int index, GenericDataEntry updated_entry)
            {
                if (_data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data series");
                    return;
                }
                if (_data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data series");
                    return;

                }
                
                /* Not required for meta data update
                if (dimensionality == DataDimensionality.Uniform)
                {
                    var dataseries = (SciChartUniformData_Type)_library_data[data_type.Key];
                    using (dataseries.SuspendUpdates())
                    {
                        dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                    }
                }
                else if (dimensionality == DataDimensionality.TwoDimensional)
                {

                    var dataseries = (SciChartData_Type)_library_data[data_type.Key];
                    using (dataseries.SuspendUpdates())
                    {
                        dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                    }
                }
                */
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void add_series(GenericDataStructure branch, DataDimensionality dimensionality)
            {
                foreach (var b in branch.Branches)
                {
                    add_series(b, dimensionality);
                }

                // For each branch add all leafs to one data series
                if (branch.Entries.Count > 0)
                {
                    DataType data_series = new DataType();
                    data_series.Name = UniqueID.Generate();

                    if (dimensionality == DataDimensionality.Uniform)
                    {
                        var series = new SciChartUniformDataType();
                        foreach (var l in branch.Entries)
                        {
                            series.Append((double)l.Values[0], l.MetaData);
                        }
                        data_series.DataSeries = series;

                    }
                    else if (dimensionality == DataDimensionality.TwoDimensional)
                    {
                        var series = new SciChartXYDataType();
                        foreach (var l in branch.Entries)
                        {
                            series.Append((double)l.Values[0], (double)l.Values[1], l.MetaData);
                        }
                        data_series.DataSeries = series;
                    }
                    _data.Add(data_series);
                }
            }
        }
    }
}
