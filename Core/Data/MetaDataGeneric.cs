using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Abstracts;



/*
 * Meta data for generic data
 * 
 */
namespace Core
{
    namespace Data
    {

        public class MetaDataGeneric
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
