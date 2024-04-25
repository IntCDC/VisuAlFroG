using System.ComponentModel;

using Newtonsoft.Json.Linq;



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

            public int _Index { get; set; } = -1;
            public string _Label { get; set; } = "";
            public bool _Selected
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

            public uint _Dimension { get; set; } = 0;
            public double _Min { get; set; } = double.PositiveInfinity;
            public double _Max { get; set; } = double.NegativeInfinity;

            /* ------------------------------------------------------------------*/
            // private variables

            private bool _selected = false;
        }
    }
}
