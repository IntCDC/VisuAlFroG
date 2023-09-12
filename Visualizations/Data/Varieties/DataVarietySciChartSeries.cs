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

namespace Visualizations
{
    namespace Data
    {
        public class DataVarietySciChartSeries<DataType> : AbstractDataVariety<List<DataType>>
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override List<Dimension> SupportedDimensions
            {
                get
                {
                    return new List<Dimension>() { Dimension.Uniform, Dimension.TwoDimensional };
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
                if (!CompatibleDimensionality(data_dimension) || !CompatibleValueTypes(value_types) || (value_types.Count > 1))
                {
                    return;
                }
                if (data == null)
                {
                    return;
                }
                if (_data == null)
                {
                    _data = new List<DataType>();
                }

                convert_data(data, data_dimension);
                if (_data.Count > 0)
                {
                    _created = true;
                }
            }

            public override void UpdateEntryAtIndex(GenericDataEntry updated_entry)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }

                foreach (var d in _data)
                {
                    for (int i = 0; i < d.DataSeries.Count; i++)
                    {
                        if (updated_entry.MetaData.Index == ((MetaData)d.DataSeries.Metadata[i]).Index)
                        {
                            using (d.DataSeries.SuspendUpdates())
                            {
                                var uniform_series = d.DataSeries as SciChartUniformDataType;
                                if (uniform_series != null)
                                {
                                    uniform_series.Update(i, (double)d.DataSeries.YValues[i], updated_entry.MetaData);
                                    break;
                                }
                                var xy_series = d.DataSeries as SciChartXYDataType;
                                if (xy_series != null)
                                {
                                    xy_series.Update(i, (double)d.DataSeries.YValues[i], updated_entry.MetaData);
                                    break;
                                }
                                Log.Default.Msg(Log.Level.Error, "Unable to convert data series to known type");
                            }
                        }
                    }
                    d.InvalidateVisual();
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void convert_data(GenericDataStructure branch, int data_dimension)
            {
                // For each branch add all leafs to one data series
                if (branch.Entries.Count > 0)
                {
                    DataType data_series = new DataType();

                    if (data_dimension == 1)
                    {
                        var series = new SciChartUniformDataType();
                        foreach (var entry in branch.Entries)
                        {
                            series.Append((double)entry.Values[0], entry.MetaData);
                        }
                        data_series.DataSeries = series;

                    }
                    else if (data_dimension == 2)
                    {
                        var series = new SciChartXYDataType();
                        foreach (var entry in branch.Entries)
                        {
                            series.Append((double)entry.Values[0], (double)entry.Values[1], entry.MetaData);
                        }
                        data_series.DataSeries = series;
                    }
                    _data.Add(data_series);
                }

                foreach (var b in branch.Branches)
                {
                    convert_data(b, data_dimension);
                }
            }
        }
    }
}
