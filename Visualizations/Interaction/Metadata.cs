using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.Model.DataSeries;



/*
 * Meta data
 * 
 */
namespace Visualizations
{

    namespace Interaction
    {

        public class Metadata : SciChart.Charting.Model.DataSeries.IPointMetadata
        {

            /* ------------------------------------------------------------------*/
            // public events

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


            /* ------------------------------------------------------------------*/
            // public properties

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
