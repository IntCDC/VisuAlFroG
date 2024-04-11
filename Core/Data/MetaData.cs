using SciChart.Charting.Model.DataSeries;



/*
 * Meta data
 * 
 * XXX Depends on SciChart.Charting.Model.DataSeries.IPointMetadata
 * TODO: How to avoid this?
 * 
 */
namespace Core
{
    namespace Data
    {

        public class MetaData : IPointMetadata
        {

            /* ------------------------------------------------------------------*/
            // public events

            /// <summary>
            /// Event to indicated changed properties.
            /// </summary>
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


            /* ------------------------------------------------------------------*/
            // public properties

            /// <summary>
            /// Index of the data point.
            /// </summary>
            public int Index { get; set; }

            public bool IsSelected
            {
                get
                {
                    return _selected;
                }
                set
                {
                    bool call_property_change = (_selected != value);
                    _selected = value;
                    if (call_property_change)
                    {
                        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("IsSelected"));
                    }
                }
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private bool _selected = false;
        }
    }
}
