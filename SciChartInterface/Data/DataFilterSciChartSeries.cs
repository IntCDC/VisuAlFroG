using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;

using SciChart.Charting.Model.Filters;


/*
 *  SciChart data filter for series data type
 *  
 */
namespace SciChartInterface
{
    namespace Data
    {
        public class DataFilterSciChartSeries : AbstractDataFilter
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public DataFilterSciChartSeries() : base(Filters.TRANSPOSE | Filters.AXIS_SELECTION) { } // base(Filters.NONE) { } //

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override bool Filter_TRANSPOSE()
            {
                var new_series = new List<SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>>();

/*
                bool init = false;
                int series_count = _.Count;
                for (int i = 0; i < series_count; i++)
                {
                    int values_count = _data[i].DataSeries.Count;
                    for (int j = 0; j < values_count; j++)
                    {
                        if (!init)
                        {
                            for (var s = 0; s < values_count; s++)
                            {
                                new_series.Add(new SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>());
                            }
                            init = true;
                        }
                        new_series[j].Append((double)_data[i].DataSeries.XValues[j], (double)_data[i].DataSeries.YValues[j], _data[i].DataSeries.Metadata[j]);
                    }
                }

                _data.Clear();
                foreach (var series in new_series)
                {
                    DataType data_series = new DataType();
                    data_series.AntiAliasing = true;
                    data_series.Style = renders_series_style();
                    data_series.DataSeries = series;
                    _data.Add(data_series);
                }
*/
                return true;
            }

            protected override bool Filter_AXIS_SELECTION()
            {


                return true;
            }

            #endregion
        }
    }
}
