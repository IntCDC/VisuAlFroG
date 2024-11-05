using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;



/*
 *  Filter for transposing 2D data
 *  
 */
namespace Core
{
    namespace Filter
    {
        public class Filter_Transpose : AbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public Filter_Transpose()
            {
                _Name = "Transpose";
                _Description = "Transpose Tabular 2D Data";
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override UIElement create_update_ui(in GenericDataStructure in_data)
            {
                var info = new TextBlock();
                info.Text = _Description;
                info.FontWeight = FontWeights.Bold;
                info.Margin = new Thickness(_Margin);

                return info;
            }

            protected override void apply_filter(GenericDataStructure out_data)
            {
                out_data.Transpose();
            }

            #endregion
        }
    }
}
