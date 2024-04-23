using System.ComponentModel;



/*
 * Generic meta data
 * 
 */
namespace Core
{
    namespace Data
    {

        public class MetaDataGeneric : IMetaData
        {

            /* ------------------------------------------------------------------*/
            // public events

            /// <summary>
            /// Event to indicated changed properties.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;


            /* ------------------------------------------------------------------*/
            // public properties

            /// <summary>
            /// Index of the data point.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Label of the data point.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Flag indicating data point selection
            /// </summary>
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
                    if (call_property_change && (PropertyChanged != null))
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                    }
                }
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private bool _selected = false;
        }
    }
}
