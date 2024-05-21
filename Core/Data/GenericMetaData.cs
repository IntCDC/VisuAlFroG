using System.ComponentModel;

using Core.Abstracts;



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
            #region public events

            /// <summary>
            /// Event called on change of _Selected property.
            /// Needs to be set by each Data type individually
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged = null;

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public uint _Index { get; set; } = uint.MaxValue;
            public string _Label { get; set; } = "";
            public uint _Dimension { get; set; } = uint.MaxValue;
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
                        PropertyChanged(this, new PropertyChangedEventArgs("_Selected"));
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private bool _selected = false;

            #endregion
        }
    }
}
