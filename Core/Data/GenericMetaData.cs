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

        public class GenericMetaData : IMetaData
        {

            /* ------------------------------------------------------------------*/
            // public events

            /// <summary>
            /// Event called on change of _Selected property.
            /// Needs to be set by each Data type individually
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged = null;


            /* ------------------------------------------------------------------*/
            // public properties

            public uint   _Index { get; set; }      = uint.MaxValue;
            public string _Label { get; set; }      = "";
            public uint   _Dimension { get; set; }  = 0;
            public bool   _Selected
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
                        PropertyChanged(this, new PropertyChangedEventArgs("_Selected"));
                    }
                }
            } 


            /* ------------------------------------------------------------------*/
            // private variables

            private bool _selected = false;
        }
    }
}
