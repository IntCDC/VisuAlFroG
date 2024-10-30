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
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override UIElement create_update_ui(in GenericDataStructure in_data)
            {
                var info = new TextBlock();
                /*if (in_data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing input data...");
                    return info;
                }*/
                info.Text = "Transpose Tabular 2D Data.";
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
