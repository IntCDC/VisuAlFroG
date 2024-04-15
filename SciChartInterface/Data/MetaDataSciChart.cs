using SciChart.Charting.Model.DataSeries;
using Core.Abstracts;
using Core.Data;
using System.ComponentModel;



/*
 * SciChart meta data wrapper
 */
namespace SciChartInterface
{
    namespace Data
    {
        public class MetaDataSciChart : MetaDataGeneric, IPointMetadata
        {

            public MetaDataSciChart(int index, bool is_selected, PropertyChangedEventHandler meta_data_update_handler)
            {

                Index = index;
                IsSelected = is_selected;
                // Intended: Setting PropertyChanged after IsSelected prevents useless update of all meta data
                PropertyChanged += meta_data_update_handler;
            }
        }
    }
}
