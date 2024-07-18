using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;


/*

INSTRUCTIONS for creating own custom data filter:

see https://github.com/IntCDC/VisuAlFroG/blob/main/docs/developer-guide.md

*/


/*
 *  Custom Row Selection Data Filter
 * 
 */
namespace Core
{
    namespace Filter
    {
        public class RowSelectionFilter : AbstractFilter
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

            public RowSelectionFilter()
            {
                _Name = "Row/Series Selection";
                _UniqueContent = true;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override UIElement create_update_ui(in GenericDataStructure in_data)
            {
                if (in_data == null)
                {
                    _ui_element.Text = "Select content to retrieve row information.";
                    return _ui_element;
                }
                else
                {
                    _ui_element.Text = "Select rows/series:";

                    // Call when value has changed and filter should be applied with changes
                    // set_apply_dirty();


                    return _ui_element;
                }                
            }

            protected override void apply_filter(GenericDataStructure out_data)
            {
                // Change in_out_data accordingly...

            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private TextBlock _ui_element = new TextBlock();


            #endregion
        }
    }
}