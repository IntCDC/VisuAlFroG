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
        public class SciChartMetaData : GenericMetaData, IPointMetadata
        {

            // Wrapper required for IPointMetadata
            public bool IsSelected { get { return _Selected; } set { _Selected = value; } }

            public SciChartMetaData(uint index, bool is_selected, PropertyChangedEventHandler update_metadata_handler)
            {
                _Index = index;
                _Selected = is_selected;
                // Setting PropertyChanged after IsSelected prevents useless initial update of all meta data
                PropertyChanged += update_metadata_handler;
            }
        }
    }
}
