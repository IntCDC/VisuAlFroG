using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;
using System;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.InteropServices.ComTypes;



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
                _Description = "Map data entry values to axes:";
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
                    _original_data = null;

                    var info = new TextBlock();
                    info.Text = "Select content to get filter options";
                    info.SetResourceReference(TextBlock.ForegroundProperty, "Brush_LogMessageWarn");
                    info.Margin = new Thickness(_Margin);

                    _ui_element.Children.Add(info);
                }
                else
                {
                    _original_data = in_data.DeepCopy();
                    int data_dim = (int)_original_data.GetDimension();

                    for (int i = 0; i < (data_dim + 2); i++) // data_dim + index + label column
                    {
                        var col = new ColumnDefinition();
                        col.Width = new GridLength(0.0, GridUnitType.Auto);
                        _ui_element.ColumnDefinitions.Add(col);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        var row = new RowDefinition();
                        row.Height = new GridLength(0.0, GridUnitType.Auto);
                        _ui_element.RowDefinitions.Add(row);
                    }

                    var info = new TextBlock();
                    info.Text = _Description;
                    info.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Foreground");
                    info.FontWeight = FontWeights.Bold;
                    info.Margin = new Thickness(_Margin, _Margin, _Margin, 0.0);
                    Grid.SetRow(info, 0);
                    Grid.SetColumn(info, 0);
                    Grid.SetColumnSpan(info, (data_dim + 2) - 2);
                    _ui_element.Children.Add(info);

                    var help = new HelpText();
                    help._HelpText = "Not all axes might be available in the selected content(s)";
                    Grid.SetRow(help, 0);
                    Grid.SetColumn(help, (data_dim + 2) - 1);
                    _ui_element.Children.Add(help);

                    var label_column = new Grid();
                    for (int r = 0; r < (data_dim + 1); r++) // data_dim + index
                    {
                        var row = new RowDefinition();
                        row.Height = new GridLength(0.0, GridUnitType.Auto);
                        label_column.RowDefinitions.Add(row);

                        var label_text = new TextBlock();
                        switch (r)
                        {
                            case 0: label_text.Text = "  X-Axis (1D/PCP) "; break;
                            case 1: label_text.Text = "  Y-Axis (2D) "; break;
                            //case 2: label_text.Text = "  Z-Axis (3D) "; break;
                            default: label_text.Text = "  unused"; break;
                        }
                        Grid.SetRow(label_text, r);
                        label_column.Children.Add(label_text);
                    }

                    var label_group = new GroupBox();
                    var header_label = new Label();
                    header_label.Content = "Axes";
                    header_label.SetResourceReference(Label.ForegroundProperty, "Brush_Foreground");
                    label_group.Header = header_label;
                    label_group.BorderThickness = new Thickness(0.25);
                    label_group.Padding = new Thickness(0.0);
                    label_group.Margin = new Thickness(_Margin, 0.0, 0.0, _Margin);
                    label_group.Content = label_column;
                    Grid.SetRow(label_group, 1);
                    Grid.SetColumn(label_group, 0);
                    _ui_element.Children.Add(label_group);

                    _radio_buttons = null;
                    _radio_buttons = new RadioButton[(data_dim + 1), (data_dim + 1)];
                    for (int r = 0; r < (data_dim + 1); r++) // data_dim + index
                    {
                        for (int c = 0; c < (data_dim + 1); c++) // data_dim + index
                        {
                            _radio_buttons[r, c] = new RadioButton();
                        }
                    }

                    for (int c = 0; c < (data_dim + 1); c++) // data_dim + index
                    {
                        var label = new Label();
                        label.Content = (c == 0) ? ("Idx") : (label.Content = (c).ToString());
                        label.SetResourceReference(Label.ForegroundProperty, "Brush_Foreground");

                        var group = new GroupBox();
                        group.Header = label;
                        group.BorderThickness = new Thickness(0.25);
                        group.SetResourceReference(Label.BorderBrushProperty, "Brush_Foreground");
                        group.Padding = new Thickness(0.0);
                        group.Margin = new Thickness(_Margin, 0.0, ((c < 3) ? (0.0) : (_Margin)), _Margin);

                        var dim_column = new Grid();
                        group.Content = dim_column;
                        Grid.SetRow(group, 1);
                        Grid.SetColumn(group, c + 1);
                        _ui_element.Children.Add(group);

                        for (int r = 0; r < (data_dim + 1); r++) // data_dim + index
                        {
                            _radio_buttons[r, c].SetResourceReference(RadioButton.ForegroundProperty, "Brush_Foreground");
                            _radio_buttons[r, c].HorizontalAlignment = HorizontalAlignment.Center;
                            _radio_buttons[r, c].Margin = new Thickness(0.0, 0.0, 0.0, 2.5); // Align with row labels

                            // Index = 0 is not used in default value axis mapping
                            if (c == r)
                            {
                                if (c == 0)
                                {
                                    _radio_buttons[data_dim, c].IsChecked = true;
                                }
                                else
                                {
                                    _radio_buttons[r - 1, c].IsChecked = true;
                                }
                            }
                            // Call after initial values are set -> Do not call event callback for initial setup
                            _radio_buttons[r, c].Checked += event_radio_button_checked;

                            var row = new RowDefinition();
                            row.Height = new GridLength(0.0, GridUnitType.Auto);
                            dim_column.RowDefinitions.Add(row);

                            Grid.SetRow(_radio_buttons[r, c], r);
                            dim_column.Children.Add(_radio_buttons[r, c]);
                        }
                    }
                }

                return _ui_element;
            }

            protected override void apply_filter(GenericDataStructure out_data)
            {
                // Copy values from original_data to out_data according to selection in radio button array
                for (int r = 0; r <= _radio_buttons.GetUpperBound(0); r++)
                {
                    for (int c = 0; c <= _radio_buttons.GetUpperBound(1); c++)
                    {
                        if ((bool)_radio_buttons[r, c].IsChecked && (r < out_data.GetDimension()))
                        {
                            for (int branch_index = 0; branch_index < _original_data._Branches.Count; branch_index++)
                            {
                                for (int entry_index = 0; entry_index < _original_data._Branches[branch_index]._Entries.Count; entry_index++)
                                {
                                    if (c == 0)
                                    {
                                        // XXX Check for valid conversion of index to double!
                                        out_data._Branches[branch_index]._Entries[entry_index]._Values[r] = (double)entry_index; // _original_data._Branches[branch_index]._Entries[entry_index]._Metadata._Index;
                                    }
                                    else
                                    {
                                        // c=0 is index so indices of value array starts at c-1
                                        out_data._Branches[branch_index]._Entries[entry_index]._Values[r] = _original_data._Branches[branch_index]._Entries[entry_index]._Values[c - 1];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void event_radio_button_checked(object sender, RoutedEventArgs e)
            {
                var radiobutton = sender as RadioButton;
                if (radiobutton == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }

                int row_index = -1;
                int column_index = -1;
                for (int r = 0; r <= _radio_buttons.GetUpperBound(0); r++)
                {
                    for (int c = 0; c <= _radio_buttons.GetUpperBound(1); c++)
                    {
                        if (_radio_buttons[r, c] == radiobutton)
                        {
                            row_index = r;
                            column_index = c;
                        }
                    }
                }
                if ((row_index < 0) || (column_index < 0))
                {
                    Log.Default.Msg(Log.Level.Error, "Could not find radio button");
                    return;
                }
                //Log.Default.Msg(Log.Level.Debug, "Row Index: " + row_index.ToString() + " - Column Index: " + column_index.ToString());

                // Check other indices of row and toggle selection of radio button with same selection in row
                var free_column_index = -1;
                for (int r = 0; r <= _radio_buttons.GetUpperBound(0); r++)
                {
                    if (r != row_index)
                    {
                        bool row_checked = false;
                        for (int c = 0; c <= _radio_buttons.GetUpperBound(1); c++)
                        {
                            row_checked |= (bool)_radio_buttons[r, c].IsChecked;
                        }
                        if (!row_checked)
                        {
                            for (int c = 0; c <= _radio_buttons.GetUpperBound(1); c++)
                            {
                                if ((c != column_index) && (bool)_radio_buttons[row_index, c].IsChecked)
                                {
                                    _radio_buttons[row_index, c].IsChecked = false;
                                    _radio_buttons[r, c].IsChecked = true;
                                    SetDirty();
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _ui_element = null;
            private RadioButton[,] _radio_buttons = null;
            private GenericDataStructure _original_data = null;

            #endregion
        }
    }
}
