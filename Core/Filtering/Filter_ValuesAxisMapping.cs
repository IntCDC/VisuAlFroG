using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;
using System;
using System.Reflection;
using System.Runtime.Remoting.Contexts;



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
                _UniqueContent = true;
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
                    info.Text = "Select content to get filter options";
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

                    /// Als Tablelle mit Trennstrichen?
                    /// 

                    RadioButton[,] radio_buttons = new RadioButton[3, 3];

                    _ui_element.ShowGridLines = true;
                    for (int i = 0; i < 4; i++)
                    {
                        var col = new ColumnDefinition();
                        col.Width = new GridLength(0.0, GridUnitType.Auto);
                        _ui_element.ColumnDefinitions.Add(col);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        var row = new RowDefinition();
                        row.Height = new GridLength(0.0, GridUnitType.Auto);
                        _ui_element.RowDefinitions.Add(row);
                    }

                    var dim_text_x = new TextBlock();
                    dim_text_x.Text = " X Dimension ";
                    Grid.SetColumn(dim_text_x, 0);
                    Grid.SetRow(dim_text_x, 0);
                    _ui_element.Children.Add(dim_text_x);

                    var dim_text_y = new TextBlock();
                    dim_text_y.Text = " Y Dimension ";
                    Grid.SetColumn(dim_text_y, 0);
                    Grid.SetRow(dim_text_y, 1);
                    _ui_element.Children.Add(dim_text_y);
                    var dim_text_z = new TextBlock();

                    dim_text_z.Text = " Z Dimension ";
                    Grid.SetColumn(dim_text_z, 0);
                    Grid.SetRow(dim_text_z, 2);
                    _ui_element.Children.Add(dim_text_z);


                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            radio_buttons[i, j] = new RadioButton();
                            radio_buttons[i, j].Content = " [" + i.ToString() + "," + j.ToString() + "] ";
                            radio_buttons[i, j].SetResourceReference(RadioButton.ForegroundProperty, "Brush_Foreground");
                            Grid.SetRow(radio_buttons[i, j], i);
                            Grid.SetColumn(radio_buttons[i, j], j+1);
                            _ui_element.Children.Add(radio_buttons[i, j]);
                        }
                    }

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
