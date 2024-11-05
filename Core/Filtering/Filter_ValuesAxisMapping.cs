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
                _Name = "Values Axis Mapping";
                _Description = "Map values of certain dimension to axes";
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
                    // Hence, not all axis might be available in the selected content(s
                    //_ui_element.ShowGridLines = true;
                    //_Description

                    for (int i = 0; i < 5; i++)
                    {
                        var col = new ColumnDefinition();
                        col.Width = new GridLength(0.0, GridUnitType.Auto);
                        _ui_element.ColumnDefinitions.Add(col);
                    }

                    var label_column = new Grid();
                    for (int i = 0; i < 3; i++)
                    {
                        var row = new RowDefinition();
                        row.Height = new GridLength(0.0, GridUnitType.Auto);
                        label_column.RowDefinitions.Add(row);
                    }
                    var label_text_x = new TextBlock();
                    label_text_x.Text = "  X-Axis (1D/PCP) ";
                    Grid.SetRow(label_text_x, 0);
                    label_column.Children.Add(label_text_x);
                    var label_text_y = new TextBlock();
                    label_text_y.Text = "  Y-Axis (2D) ";
                    Grid.SetRow(label_text_y, 1);
                    label_column.Children.Add(label_text_y);
                    var label_text_z = new TextBlock();
                    label_text_z.Text = "  Z-Axis (3D) ";
                    Grid.SetRow(label_text_z, 2);
                    label_column.Children.Add(label_text_z);

                    var label_group = new GroupBox();
                    var header_label = new Label();
                    header_label.Content = "Axes";
                    header_label.SetResourceReference(Label.ForegroundProperty, "Brush_Foreground");
                    label_group.Header = header_label;
                    label_group.BorderThickness = new Thickness(0.25);
                    label_group.Content = label_column;
                    Grid.SetColumn(label_group, 0);
                    _ui_element.Children.Add(label_group);

                    RadioButton[,] radio_buttons = new RadioButton[4, 3];
                    for (int i = 0; i < 4; i++)
                    {
                        var group = new GroupBox();
                        var label = new Label();
                        switch (i)
                        {
                            case 0: label.Content = "Index"; break;
                            case 1: label.Content = "1"; break;
                            case 2: label.Content = "2"; break;
                            case 3: label.Content = "3"; break;
                        }

                        label.SetResourceReference(Label.ForegroundProperty, "Brush_Foreground");
                        group.Header = label;
                        group.BorderThickness = new Thickness(0.25);

                        var dim_column = new Grid();
                        group.Content = dim_column;
                        Grid.SetColumn(group, i + 1);
                        _ui_element.Children.Add(group);

                        for (int j = 0; j < 3; j++)
                        {
                            radio_buttons[i, j] = new RadioButton();
                            radio_buttons[i, j].Content = " [" + i.ToString() + "," + j.ToString() + "] ";
                            radio_buttons[i, j].SetResourceReference(RadioButton.ForegroundProperty, "Brush_Foreground");

                            var row = new RowDefinition();
                            row.Height = new GridLength(0.0, GridUnitType.Auto);
                            dim_column.RowDefinitions.Add(row);

                            Grid.SetRow(radio_buttons[i, j], j);
                            dim_column.Children.Add(radio_buttons[i, j]);
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
