using System;
using System.Collections.Generic;
using Core.Abstracts;
using System.ComponentModel;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;
using Core.Data;
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

            public DataTypeSciChartSeries(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler, DataManager.GetSendOutputCallback_Delegate send_output_callback)
                : base(update_data_handler, update_metadata_handler, send_output_callback) { }

            public override void UpdateData(GenericDataStructure data)
            {
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }

                _data_specific = new List<DataType>();
                _data_generic = null;
                _loaded = false;

                _Dimension = data.GetDimension();
                _data_generic = data.DeepCopy();
                specific_data_conversion(data);
                _loaded = true;
            }

            public override void UpdateMetaDataEntry(IMetaData updated_meta_data)
            {
                if (!_loaded)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }

                foreach (var data_series in _data_specific)
                {
                    using (data_series.DataSeries.SuspendUpdates())
                    {
                        int values_count = data_series.DataSeries.Count;
                        for (int i = 0; i < values_count; i++)
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

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override List<Control> GetMenu()
            {
                return base.GetMenu();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void specific_data_conversion(GenericDataStructure data)
            {
                // For each branch add all leafs to one data series
                if (data._Entries.Count > 0)
                {
                    var dimension = data.GetDimension();

                    // Always take first values of first dimension as default for x-axis and values of second dimension for y-axis
                    // -> Adjustable via filter: Select value dimension from multi-dimensional data for each available axis individually
                    int value_index = 0; // (int)data.GetDimension() - 1;

                    var series = new SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>();
                    series.SeriesName = (data._Label == "") ? (UniqueID.GenerateString()) : (data._Label);
                    foreach (var entry in data._Entries)
                    {
                        double x = double.NaN;
                        double y = double.NaN;

                        if (dimension == 1)
                        {
                            x = (double)entry._Metadata._Index;
                            y = (double)entry._Values[value_index];
                        }
                        else if (dimension == 2)
                        {
                            x = (double)entry._Values[value_index];
                            y = (double)entry._Values[value_index + 1];

                            /// XXX Can result in much slower performance for large unsorted data
                            series.AcceptsUnsortedData = true; 
                        }
                        var meta_data = new SciChartMetaData(entry._Metadata._Index, entry._Metadata._Selected, _update_metadata_handler);
                        series.Append(x, y, meta_data);
                    }

                    DataType data_series = new DataType();
                    data_series.DataSeries = series;
                    _data_specific.Add(data_series);
                }

                foreach (var b in data._Branches)
                {
                    specific_data_conversion(b);
                }
            }

            #endregion
        }
    }
}
