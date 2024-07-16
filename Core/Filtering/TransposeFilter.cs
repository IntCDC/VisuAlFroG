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
 *  
 *  
 */
namespace Core
{
    namespace Filter
    {
        public class TransposeFilter : AbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            /*
            public class Configuration : AbstractFilter.Configuration
            {
                /// XXX TODO Add additional information required to restore the filter
            }
*           */

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions


            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override UIElement create_ui()
            {
                _Name = "Transpose"; 

                var ui = new TextBlock();
                ui.Text = "Transpose tabular 2D data";


                // Call when filter option has changed and filter should be applied again
                // SetDirty();


                return ui;
            }

            protected override void apply_filter(GenericDataStructure in_out_data)
            {
                in_out_data.Transpose();
            }

            #endregion
        }
    }
}
