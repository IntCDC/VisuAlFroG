using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Visualizations
{
    namespace Interaction
    {
        public class SelectedPointMetadata : SciChart.Charting.Model.DataSeries.IPointMetadata
        {
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

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
                        PropertyChanged(IsSelected, new System.ComponentModel.PropertyChangedEventArgs("IsSelected"));
                    }
                }
            }

            private bool _selected = false;
        }
    }

}
