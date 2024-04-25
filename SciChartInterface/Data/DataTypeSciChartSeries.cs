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

            public DataTypeSciChartSeries(PropertyChangedEventHandler meta_data_update_handler) : base(meta_data_update_handler) { }

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

                if (!CompatibleDimensionality(data.DataDimension()) || !CompatibleValueTypes(data.ValueTypes()))
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
                            if (updated_meta_data._Index == ((MetaDataSciChart)data_series.DataSeries.Metadata[i])._Index)
                            {
                                ((MetaDataSciChart)data_series.DataSeries.Metadata[i])._Selected = updated_meta_data._Selected;
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
                    data_series.Name = UniqueID.Generate();
                    data_series.AntiAliasing = true;

                    if (branch.DataDimension() == 1)
                    {
                        var series = new SciChartUniformDataType();
                        foreach (var entry in branch._Entries)
                        {
                            var meta_data = new MetaDataSciChart(entry._Metadata._Index, entry._Metadata._Selected, _meta_data_update_handler);
                            series.Append((double)entry._Values[0], meta_data);
                        }
                        data_series.DataSeries = series;

                    }
                    else if (branch.DataDimension() == 2)
                    {
                        var series = new SciChartXYDataType();
                        foreach (var entry in branch._Entries)
                        {
                            var meta_data = new MetaDataSciChart(entry._Metadata._Index, entry._Metadata._Selected, _meta_data_update_handler);
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
        }
    }
}
