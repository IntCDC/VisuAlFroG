using System;
using System.Windows;
using System.Collections.Generic;
using Core.Abstracts;
using Core.GUI;
using Core.Utilities;
using Core.Data;
using System.Windows.Controls;



/*
 * Data Manager
 * 
 */
namespace Core
{
    namespace Filter
    {
        public class FilterManager : AbstractRegisterService<AbstractFilter>
        {
            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring filters.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public List<AbstractFilter.Configuration> FilterList { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public delegates

            public delegate bool DeleteFilterCallback_Delegate(int filter_uid);
            public delegate void FilterChanged_Delegate(int filter_uid, List<int> data_uids);
            public delegate UIElement GetUICallback_Delegate();
            public delegate void ResetFilterManagerCallback_Delegate();

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public bool Initialize(DataManager.GetGenericDataCallback_Delegate get_selected_data_callback, DataManager.UpdateFilteredDataCallback_Delegate update_selected_data_callback)
            {
                if (!base.Initialize())
                {
                    return false;
                }
                if ((update_selected_data_callback == null) || (get_selected_data_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                }
                _timer.Start();


                _contents_metadata = new List<AbstractFilter.ContentMetadata>();
                _ordered_filter_list = new List<int>();

                _get_selected_data_callback = get_selected_data_callback;
                _update_selected_data_callback = update_selected_data_callback;

                register_entry(typeof(Filter_Transpose));
                register_entry(typeof(Filter_RowSelection));
                register_entry(typeof(Filter_ColumnSelection));
                register_entry(typeof(Filter_ValuesAxisMapping));
                /// >>> Register your new filter type here:
                /// register_content(typeof(CustomFilter));


                _add_filter_list = new ComboBox();
                _list_scrolling = new ScrollViewer();
                _filter_list = new StackPanel();


                var list = new List<FilterTypeMetadata>();
                foreach (var filter_data in _entries)
                {
                    var filter_metadata = new FilterTypeMetadata();
                    var filter = (AbstractFilter)Activator.CreateInstance(filter_data.Key);
                    filter_metadata.Name = filter._Name;
                    filter_metadata.Type = filter_data.Key;
                    list.Add(filter_metadata);
                }
                _add_filter_list.ItemsSource = list;

                _timer.Stop();
                _initialized = true;
                return _initialized;
            }

            public override bool Terminate()
            {
                bool terminated = base.Terminate();
                if (_initialized)
                {
                    _contents_metadata.Clear();
                    _contents_metadata = null;

                    _ordered_filter_list.Clear();
                    _ordered_filter_list = null;

                    _update_selected_data_callback = null;
                    _get_selected_data_callback = null;

                    _add_filter_list = null;
                    _list_scrolling = null;
                    _filter_list = null;

                    _initialized = false;
                    terminated &= true;
                }
                return terminated;
            }

            public UIElement GetUI()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }
                return _create_ui();
            }

            public bool DeleteFilterCallback(int filter_uid)
            {
                // Loop over registered types
                foreach (var filter_types in _entries)
                {
                    if (filter_types.Value.ContainsKey(filter_uid))
                    {
                        // Remove filter from UI list
                        _filter_list.Children.Remove(filter_types.Value[filter_uid].GetUI());

                        filter_types.Value[filter_uid].Terminate();
                        // Call after terminate
                        _ordered_filter_list.Remove(filter_types.Value[filter_uid]._UID);
                        return filter_types.Value.Remove(filter_uid);
                    }
                }
                Log.Default.Msg(Log.Level.Debug, "Filter not available for deletion: " + filter_uid);
                return false;
            }

            public string CollectConfigurations()
            {
                var filtermanager_configuration = new FilterManager.Configuration();
                filtermanager_configuration.FilterList = new List<AbstractFilter.Configuration>();

                foreach (var filter_types in _entries)
                {
                    foreach (var filter in filter_types.Value)
                    {
                        var filter_config = new AbstractFilter.Configuration() { Type = filter_types.Key.FullName };
                        filtermanager_configuration.FilterList.Add(filter_config);
                    }
                }

                return ConfigurationService.Serialize<FilterManager.Configuration>(filtermanager_configuration);
            }

            public bool ApplyConfigurations(string configurations)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                var filtermanager_configuration = ConfigurationService.Deserialize<FilterManager.Configuration>(configurations);
                if (filtermanager_configuration != null)
                {
                    foreach (var filter_config in filtermanager_configuration.FilterList)
                    {
                        _create_filter_callback(get_type(filter_config.Type));
                    }
                    return true;
                }
                return false;
            }

            public void AddContentMetadataCallback(AbstractFilter.ContentMetadata content_metadata)
            {
                _contents_metadata.Add(content_metadata);

                foreach (var filter_type in _entries)
                {
                    foreach (var filter in filter_type.Value)
                    {
                        filter.Value.ContentMetadataListCallback(_contents_metadata);
                    }
                }
            }

            public void DeleteContentMetadataCallback(int data_uid)
            {
                foreach (var cm in _contents_metadata)
                {
                    if (cm.DataUID == data_uid)
                    {
                        _contents_metadata.Remove(cm);
                        break;
                    }
                }
                foreach (var filter_type in _entries)
                {
                    foreach (var filter in filter_type.Value)
                    {
                        filter.Value.ContentMetadataListCallback(_contents_metadata);
                    }
                }
            }

            public void Reset()
            {
                var get_selected_data_callback = _get_selected_data_callback;
                var update_selected_data_callback = _update_selected_data_callback;
                Terminate();
                Initialize(get_selected_data_callback, update_selected_data_callback);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// <summary>
            /// Reset specific filter (only used in AbstractRegisterService)
            /// </summary>
            /// <param name="filter_value"></param>
            /// <returns></returns>
            protected override bool reset_entry(AbstractFilter filter_value)
            {
                return filter_value.Terminate();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private bool _create_filter_callback(Type filter_type)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (filter_type == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to find type: " + filter_type);
                    return false;
                }

                if (_entries.ContainsKey(filter_type))
                {
                    var filter = (AbstractFilter)Activator.CreateInstance(filter_type);
                    if (filter.Initialize(_get_selected_data_callback, _update_selected_data_callback, this.filter_changed, DeleteFilterCallback))
                    {
                        if (filter.CreateUI())
                        {
                            filter.ContentMetadataListCallback(_contents_metadata);
                            _entries[filter_type].Add(filter._UID, filter);
                            _ordered_filter_list.Add(filter._UID);

                            // Add to UI list
                            _filter_list.Children.Add(filter.GetUI());
                            _list_scrolling.ScrollToBottom();
                            return true;
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Unable to create the UI of the new filter: " + filter_type.ToString());
                        }
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to initialize the new filter: " + filter_type.ToString());
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unregistered filter type: " + filter_type.ToString());
                }
                return false;
            }

            void filter_changed(int filter_uid, List<int> data_uids)
            {
                // Notify all subsequent filters that previous filter has changed and that their original data has changed
                if (!_ordered_filter_list.Contains(filter_uid))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing filter UID");
                    return;
                }
                var index = _ordered_filter_list.FindIndex(f => (f == filter_uid));
                for (int i = index + 1; i < _ordered_filter_list.Count; i++)
                {
                    foreach (var filter_type in _entries)
                    {
                        foreach (var filter in filter_type.Value)
                        {
                            if (filter.Value._UID == _ordered_filter_list[i])
                            {
                                filter.Value.SubsequentApply(data_uids);
                            }
                        }
                    }
                }
            }

            private UIElement _create_ui()
            {
                _add_filter_list.IsEditable = false;
                _add_filter_list.DisplayMemberPath = "Name";
                _add_filter_list.SelectedIndex = 0;
                _add_filter_list.Margin = new Thickness(0.0, AbstractFilter._Margin, AbstractFilter._Margin, AbstractFilter._Margin);

                var add_button = new Button();
                add_button.Content = " Add Filter ";
                add_button.Click += event_apply_button;
                add_button.Margin = new Thickness(AbstractFilter._Margin, AbstractFilter._Margin / 2.0, AbstractFilter._Margin, AbstractFilter._Margin / 2.0);

                _list_scrolling.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _list_scrolling.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                _list_scrolling.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
                _list_scrolling.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
                _list_scrolling.PreviewMouseWheel += event_scrollviewer_mousewheel;
                _list_scrolling.Content = _filter_list;

                var help = new HelpText();
                help._HelpText = "NOTE: Filters are applied sequentially from top to bottom. \n" +
                    "On filter change, below filters with same checked content are automatically re-applied";

                var hrule = new Border();
                hrule.SetResourceReference(Border.BackgroundProperty, "Brush_Background");
                hrule.SetResourceReference(Border.BorderBrushProperty, "Brush_Foreground");
                hrule.BorderThickness = new Thickness(AbstractFilter._BorderThickness);
                hrule.Margin = new Thickness(AbstractFilter._Margin);
                hrule.CornerRadius = new CornerRadius(0);
                hrule.Height = AbstractFilter._BorderThickness;
                hrule.VerticalAlignment = VerticalAlignment.Bottom;
                hrule.HorizontalAlignment = HorizontalAlignment.Stretch;

                var caption_grid = new Grid();

                var add_grid = new Grid();
                var column_label = new ColumnDefinition();
                column_label.Width = new GridLength(0.0, GridUnitType.Auto);
                add_grid.ColumnDefinitions.Add(column_label);
                Grid.SetColumn(add_button, 0);
                add_grid.Children.Add(add_button);

                var column_list = new ColumnDefinition();
                column_list.Width = new GridLength(1.0, GridUnitType.Star);
                add_grid.ColumnDefinitions.Add(column_list);
                Grid.SetColumn(_add_filter_list, 1);
                add_grid.Children.Add(_add_filter_list);

                var column_help = new ColumnDefinition();
                column_help.Width = new GridLength(0.0, GridUnitType.Auto);
                add_grid.ColumnDefinitions.Add(column_help);
                Grid.SetColumn(help, 2);
                add_grid.Children.Add(help);

                var row_index = -1;

                var apply_row = new RowDefinition();
                apply_row.Height = new GridLength(1.0, GridUnitType.Star);
                caption_grid.RowDefinitions.Add(apply_row);
                Grid.SetRow(add_grid, ++row_index);
                caption_grid.Children.Add(add_grid);

                var row_hrule = new RowDefinition();
                row_hrule.Height = new GridLength(1.0, GridUnitType.Star);
                caption_grid.RowDefinitions.Add(row_hrule);
                Grid.SetRow(hrule, ++row_index);
                caption_grid.Children.Add(hrule);

                var parent = new DockPanel();
                DockPanel.SetDock(caption_grid, System.Windows.Controls.Dock.Top);
                parent.Children.Add(caption_grid);
                parent.Children.Add(_list_scrolling);

                return parent;
            }

            private void event_apply_button(object sender, RoutedEventArgs e)
            {
                var apply_button = sender as Button;
                if (apply_button == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender.");
                    return;
                }
                var selected_item = _add_filter_list.SelectedItem as FilterManager.FilterTypeMetadata;
                if (selected_item == null)
                {
                    Log.Default.Msg(Log.Level.Warn, "Select filter type before trying to add a new filter.");
                    return;
                }
                if (selected_item.Type != null)
                {
                    _create_filter_callback(selected_item.Type);
                }
            }

            private void event_scrollviewer_mousewheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
            {
                ScrollViewer scv = (ScrollViewer)sender;
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                scv.UpdateLayout();
                e.Handled = true;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private class FilterTypeMetadata
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public Type Type { get; set; }
            }

            private ComboBox _add_filter_list = null;
            private ScrollViewer _list_scrolling = null;
            private StackPanel _filter_list = null;

            // Required to provide new filters with content meta data
            private List<AbstractFilter.ContentMetadata> _contents_metadata = null;
            private List<int> _ordered_filter_list = null;

            private DataManager.UpdateFilteredDataCallback_Delegate _update_selected_data_callback = null;
            private DataManager.GetGenericDataCallback_Delegate _get_selected_data_callback = null;

            #endregion
        }
    }
}
