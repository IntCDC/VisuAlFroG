using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;



/*
 *  Filter for custom column selection
 *  
 */
namespace Core
{
    namespace Filter
    {
        public class Filter_ColumnSelection : AbstractFilter
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

            public Filter_ColumnSelection()
            {
                _Name = "Select Data Parameters (Columns)";
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
                    if (_checkable_content_list == null)
                    {
                        _checkable_content_list = new StackPanel();
                    }
                    _checkable_content_list.Children.Clear();

                    var info = new TextBlock();
                    info.Text = "Select columns:";
                    info.FontWeight = FontWeights.Bold;
                    info.Margin = new Thickness(_Margin);

                    var deselect_button = new Button();
                    deselect_button.Content = "Clear Selection";
                    deselect_button.Click += _event_deselect_button;
                    deselect_button.Margin = new Thickness(_Margin);
                    deselect_button.HorizontalAlignment = HorizontalAlignment.Left;
                    deselect_button.Width = Miscellaneous.MeasureButtonString(deselect_button).Width + (2 * _Margin);
                    _deselect = true;

                    _checkable_content_list.Children.Clear();
                    if (in_data._Branches.Count < 1)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing rows...");
                    }
                    else
                    {
                        foreach (var column in in_data._Branches[0]._Entries)
                        {
                            var check = new CheckBox();
                            check.Margin = new Thickness(_Margin);
                            check.Click += _event_column_checked;
                            check.Content = column._Metadata._Label;
                            check.IsChecked = true;
                            check.SetResourceReference(CheckBox.ForegroundProperty, "Brush_Foreground");
                            _checkable_content_list.Children.Add(check);
                        }
                    }

                    var top_row = new RowDefinition();
                    top_row.Height = new GridLength(0.0, GridUnitType.Auto);
                    _ui_element.RowDefinitions.Add(top_row);
                    Grid.SetRow(info, 0);
                    _ui_element.Children.Add(info);

                    var button_row = new RowDefinition();
                    button_row.Height = new GridLength(0.0, GridUnitType.Auto);
                    _ui_element.RowDefinitions.Add(button_row);
                    Grid.SetRow(deselect_button, 1);
                    _ui_element.Children.Add(deselect_button);

                    var list_row = new RowDefinition();
                    list_row.Height = new GridLength(0.0, GridUnitType.Auto);
                    _ui_element.RowDefinitions.Add(list_row);
                    Grid.SetRow(_checkable_content_list, 2);
                    _ui_element.Children.Add(_checkable_content_list);
                }
                return _ui_element;
            }

            protected override void apply_filter(GenericDataStructure out_data)
            {
                // Change out_data accordingly...
                foreach (var child in _checkable_content_list.Children)
                {
                    var checkable_column = child as CheckBox;
                    if (checkable_column == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                        return;
                    }

                    var is_selected = (bool)checkable_column.IsChecked;
                    var column_name = (string)checkable_column.Content;

                    if (out_data._Branches.Count < 1)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing rows...");
                    }
                    else
                    {
                        for (int i = 0; i < out_data._Branches[0]._Entries.Count; i++)
                        {
                            if (!is_selected && (column_name == out_data._Branches[0]._Entries[i]._Metadata._Label))
                            {
                                for (int j = 0; j < out_data._Branches.Count; j++)
                                {
                                    out_data._Branches[j]._Entries.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void _event_column_checked(object sender, RoutedEventArgs e)
            {
                var checkbox = sender as CheckBox;
                if (checkbox == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }
                //var is_checked = (bool)checkbox.IsChecked;
                base.SetDirty();
            }

            private void _event_deselect_button(object sender, RoutedEventArgs e)
            {
                var deselect_button = sender as Button;
                if (deselect_button == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                }

                foreach (var child in _checkable_content_list.Children)
                {
                    var checkable_column = child as CheckBox;
                    if (checkable_column == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                        return;
                    }
                    checkable_column.IsChecked = !_deselect;
                }

                if (_deselect)
                {
                    deselect_button.Content = "Select All";
                    _deselect = false;
                    base.SetDirty();
                }
                else
                {
                    deselect_button.Content = "Clear Selection";
                    _deselect = true;
                    base.SetDirty();
                }
                deselect_button.Width = Miscellaneous.MeasureButtonString(deselect_button).Width + (2 * _Margin);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _ui_element = null;
            private StackPanel _checkable_content_list = null;
            private bool _deselect = true;

            #endregion
        }
    }
}
