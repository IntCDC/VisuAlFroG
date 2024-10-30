using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;
using System;
using System.Reflection;



/*
 *  Filter for selecting value dimension for x- or y-axis (PCP only y-axis)
 * 
 */
namespace Core
{
    namespace Filter
    {
        public class Filter_ValuesAxisMapping : AbstractFilter
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

            public Filter_ValuesAxisMapping()
            {
                _Name = "Map values of certain dimension to axes";
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override UIElement create_update_ui(in GenericDataStructure in_data)
            {
                if (_ui_element == null)
                {
                    _ui_element = new Grid();
                }
                _ui_element.Children.Clear();

                if (in_data == null)
                {
                    var info = new TextBlock();
                    info.Text = "Select content(s) to get filter options";
                    info.SetResourceReference(TextBlock.ForegroundProperty, "Brush_LogMessageWarn");
                    info.FontWeight = FontWeights.Bold;
                    info.Margin = new Thickness(_Margin);

                    _ui_element.Children.Add(info);
                }
                else
                {
                    /// TODO
                    /// - Always one value dimension has to be selected
                    /// - Each value dimension has always be selected (switch axis on value dimension change)
                    /// - Default and shown value dimensions depending on actually available value dimensions

                    // Hence, not all axis might be available in the selected content(s)
                    // ---
                    // Content: X-Axis (1D/BarPlot/ParallelCoordinatesPlot)
                    // Value Dimension: [] Index | [X] 1 | [] 2 | [] 3
                    // ----
                    // Content: Y-Axis (2D)
                    // Value Dimension: [] Index | [] 1 | [X] 2 | [] 3
                    // ---
                    // Content: Z-Axis (3D)
                    // Value Dimension: [] Index | [] 1 | [] 2 | [X] 3

                }
                return _ui_element;
            }

            protected override void apply_filter(GenericDataStructure out_data)
            {
                /// TODO
          




            }

            #endregion


            /* ------------------------------------------------------------------*/
            #region private functions



            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _ui_element = null;



            #endregion
        }
    }
}
