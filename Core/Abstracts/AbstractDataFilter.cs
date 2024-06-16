using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Dynamic;
using static Core.GUI.ColorTheme;
using Core.GUI;
using Core.Utilities;



/*
 * Abstract data filter
 * 
 */

using AxisFilterData = System.Tuple<uint, uint>;


namespace Core
{
    namespace Data
    {
        public abstract class AbstractDataFilter
        {
            /* ------------------------------------------------------------------*/
            #region public enum

            [Flags]
            public enum Filters
            {
                NONE = 0,
                TRANSPOSE = 1 << 0,
                AXIS_SELECTION = 1 << 1,
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public List<MenuItem> _Menu { get; protected set; } = new List<MenuItem>();

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractDataFilter(Filters supported_filters)
            {
                _filters = supported_filters;
            }

            public bool Update(GenericDataStructure original_data)
            {
                _original_filter_data = original_data;

                /// TODO

                create_menu();
                return true;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            // For each supported filter the respective function should be implemented by inheriting class

            protected virtual bool Filter_TRANSPOSE()
            {
                throw new InvalidOperationException("Filter_TRANSPOSE is marked as supported filter but has not been implemented.");
            }

            protected virtual bool Filter_AXIS_SELECTION()
            {
                throw new InvalidOperationException("Filter_AXIS_SELECTION is marked as supported filter but has not been implemented.");
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions


            protected void create_menu()
            {
                _Menu.Clear();

                if ((_filters & Filters.TRANSPOSE) == Filters.TRANSPOSE)
                {
                    var menu_item = ContentMenuBar.GetDefaultMenuItem("Transpose", Filter_TRANSPOSE);
                    _Menu.Add(menu_item);
                }

                if ((_filters & Filters.AXIS_SELECTION) == Filters.AXIS_SELECTION)
                {

                    uint dim = (_original_filter_data != null) ? (_original_filter_data.GetDimension()) : (0);

                    /// XXX TODO How is this determined?
                    uint axis_count = 2;

                    for (uint axis_idx = 0; axis_idx < axis_count; axis_idx++)
                    {
                        var menu_item = ContentMenuBar.GetDefaultMenuItem(((axis_idx == 0) ? ("[X]") : ("[Y]")) + " Axis Filter");
                        _Menu.Add(menu_item);

                        var radio_btn = new RadioButton();
                        radio_btn.Name = "radio_" + UniqueID.GenerateString();
                        radio_btn.Content = "Index";
                        radio_btn.Tag = new AxisFilterData(axis_idx, __IndexAxisIdx__);
                        if (_axis_value_map.ContainsKey(axis_idx) && (_axis_value_map[axis_idx] == __IndexAxisIdx__))
                        {
                            radio_btn.IsChecked = true;
                        }
                        radio_btn.Checked += radio_btn_checked;
                        menu_item.Items.Add(radio_btn);

                        for (uint d = 0; d < dim; d++)
                        {
                            radio_btn = new RadioButton();
                            radio_btn.Name = "radio_" + UniqueID.GenerateString();
                            radio_btn.Content = "Value" + d.ToString();
                            radio_btn.Tag = new AxisFilterData(axis_idx, d); // Save index of values
                            if (_axis_value_map.ContainsKey(axis_idx) && (_axis_value_map[axis_idx] == d))
                            {
                                radio_btn.IsChecked = true;
                            }
                            radio_btn.Checked += radio_btn_checked;
                            menu_item.Items.Add(radio_btn);
                        }
                    }


                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void radio_btn_checked(object sender, RoutedEventArgs e)
            {
                var sender_selection = sender as RadioButton;
                if (sender_selection == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown sender");
                    return;
                }

                var data = sender_selection.Tag as AxisFilterData;
                if (data != null)
                {
                    /// TODO Provide axis....
                    Filter_AXIS_SELECTION();
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to get data from radio button");
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private const uint __IndexAxisIdx__ = uint.MaxValue;

            private GenericDataStructure _original_filter_data = null; // only keep pointer
            private Filters _filters = Filters.NONE;


            private Dictionary<uint, uint> _axis_value_map = new Dictionary<uint, uint>();


            #endregion

        }
    }
}
