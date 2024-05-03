using SciChart.Charting.Model.DataSeries;
using Core.Abstracts;
using Core.Data;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;



/*
 * SciChart meta data wrapper
 */
namespace SciChartInterface
{
    namespace Data
    {
        public class MetaDataSciChart : MetaDataGeneric, IPointMetadata
        {

            // Wrapper required for IPointMetadata
            public bool IsSelected { get { return _Selected; } set { _Selected = value; } }

            public MetaDataSciChart(uint index, bool is_selected, PropertyChangedEventHandler meta_data_update_handler)
            {
                _Index = index;
                _Selected = is_selected;
                // Setting PropertyChanged after IsSelected prevents useless initial update of all meta data
                PropertyChanged += meta_data_update_handler;
            }
        }
    }
}
