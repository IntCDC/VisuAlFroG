using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Core.GUI;
using Core.Utilities;
using Core.Abstracts;
using Core.Filter;
using Core.Data;
using System.Runtime.Remoting.Contexts;
using System.Windows.Input;
using System.Windows.Data;



/*
 * Abstract data filter
 * 
 */

///                              considered, list modification,                              original data of content
using ChangedContentApplyData_Type = System.Tuple<bool, Core.Abstracts.AbstractFilter.ListModification, Core.Data.GenericDataStructure>;


namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractFilter : AbstractService, IAbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public int UID { get; set; }
                public string Type { get; set; }
                public string Name { get; set; }
                public List<int> ContentUIDList { get; set; }
            }

            public class ContentMetadata
            {
                public Binding NameBinding { get; set; }
                public int DataUID { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public enum

            // Define in Core for overall availability
            [Flags]
            public enum ListModification
            {
                DELETE,
                ADD,
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public int _UID { get; } = UniqueID.GenerateInt();
            public string _Name { get { return _filter_caption.Text; } protected set { _filter_caption.Text = value; } }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public bool Initialize(DataManager.GetGenericDataCallback_Delegate get_selected_data_callback,
                DataManager.UpdateSelectedDataCallback_Delegate update_selected_data_callback,
FilterManager.FilterChanged_Delegate filter_changed_callback,
                FilterManager.DeleteFilterCallback_Delegate delete_self_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                if ((get_selected_data_callback == null) || (update_selected_data_callback == null) || (filter_changed_callback == null) || (delete_self_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                }
                _timer.Start();


                _update_selected_data_callback = update_selected_data_callback;
                _get_selected_data_callback = get_selected_data_callback;
                _filter_changed_callback = filter_changed_callback;
                _delete_self_callback = delete_self_callback;

                _checked_content_changes = new Dictionary<int, ChangedContentApplyData_Type>();


                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    // Notify subsequent filters that this filter has changed (= will be deleted)
                    _filter_changed_callback(_UID, _get_checked_content_list());

                    // Revert all data changes
                    foreach (int key in _checked_content_changes.Keys.ToList())
                    {
                        _update_selected_data_callback(key, _checked_content_changes[key].Item3);
                    }

                    _content = null;
                    _filter_caption = null;
                    _visualizations_list = null;
                    _apply_button = null;

                    _update_selected_data_callback = null;
                    _get_selected_data_callback = null;
                    _filter_changed_callback = null;
                    _delete_self_callback = null;

                    _checked_content_changes.Clear();
                    _checked_content_changes = null;

                    _created = false;
                    _initialized = false;
                }
                return true;
            }

            /// <summary>
            /// Call in inherited class via base.CreateUI()
            /// </summary>
            public virtual bool CreateUI()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();

                _content = new Border();
                _filter_caption = new RenameLabel();
                _visualizations_list = new StackPanel();
                _apply_button = new Button();

                _filter_caption.Margin = new Thickness(_margin, _margin, _margin, 0.0);
                _filter_caption.HorizontalAlignment = HorizontalAlignment.Stretch;

                var button_grid = new Grid();

                _apply_button.Content = " Apply ";
                _apply_button.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                _apply_button.Click += _event_apply_button;
                _apply_button_default_background = _apply_button.Background;
                _apply_button.IsEnabled = false;

                var button_column = new ColumnDefinition();
                button_column.Width = new GridLength(0.0, GridUnitType.Auto);
                button_grid.ColumnDefinitions.Add(button_column);
                Grid.SetColumn(_apply_button, 0);
                button_grid.Children.Add(_apply_button);

                var rename_button = new Button();
                rename_button.Content = " Rename ";
                rename_button.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                rename_button.Click += (object sender, RoutedEventArgs e) =>
                {
                    _filter_caption.Focusable = true;
                    _filter_caption.Style = null;
                    _filter_caption.Cursor = null;
                    _filter_caption.Focus();
                };
                button_column = new ColumnDefinition();
                button_column.Width = new GridLength(0.0, GridUnitType.Auto);
                button_grid.ColumnDefinitions.Add(button_column);
                Grid.SetColumn(rename_button, 1);
                button_grid.Children.Add(rename_button);

                var delete_button = new Button();
                delete_button.Content = " Delete ";
                delete_button.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                delete_button.Click += (object sender, RoutedEventArgs e) =>
                {
                    if (_delete_self_callback != null)
                    {
                        _delete_self_callback(_UID);
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing callback.");
                    }
                };
                button_column = new ColumnDefinition();
                button_column.Width = new GridLength(0.0, GridUnitType.Auto);
                button_grid.ColumnDefinitions.Add(button_column);
                Grid.SetColumn(delete_button, 2);
                button_grid.Children.Add(delete_button);

                var apply_grid = new Grid();

                var list_label = new TextBlock();
                list_label.Text = "Select Content";
                list_label.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                var apply_text_column = new ColumnDefinition();
                apply_text_column.Width = new GridLength(1.0, GridUnitType.Auto);
                apply_grid.ColumnDefinitions.Add(apply_text_column);
                Grid.SetColumn(_apply_button, 0);
                apply_grid.Children.Add(list_label);

                var hrule = new Border();
                hrule.SetResourceReference(Border.BackgroundProperty, "Brush_Background");
                hrule.SetResourceReference(Border.BorderBrushProperty, "Brush_Foreground");
                hrule.BorderThickness = new Thickness(_border_thickness);
                hrule.Margin = new Thickness(_margin);
                hrule.CornerRadius = new CornerRadius(0);
                hrule.Height = _border_thickness;
                hrule.VerticalAlignment = VerticalAlignment.Bottom;
                hrule.HorizontalAlignment = HorizontalAlignment.Stretch;
                var apply_rule_column = new ColumnDefinition();
                apply_rule_column.Width = new GridLength(1.0, GridUnitType.Star);
                apply_grid.ColumnDefinitions.Add(apply_rule_column);
                Grid.SetColumn(hrule, 1);
                apply_grid.Children.Add(hrule);

                var content_grid = new Grid();

                var caption_row = new RowDefinition();
                caption_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(caption_row);
                Grid.SetRow(_filter_caption, 0);
                content_grid.Children.Add(_filter_caption);

                var button_row = new RowDefinition();
                button_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(button_row);
                Grid.SetRow(button_grid, 1);
                content_grid.Children.Add(button_grid);

                var list_caption_row = new RowDefinition();
                list_caption_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(list_caption_row);
                Grid.SetRow(apply_grid, 2);
                content_grid.Children.Add(apply_grid);

                var list_row = new RowDefinition();
                list_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(list_row);
                Grid.SetRow(_visualizations_list, 3);
                content_grid.Children.Add(_visualizations_list);


                var child = create_ui();

                var child_border = new Border();
                child_border.SetResourceReference(Border.BackgroundProperty, "Brush_Background");
                child_border.SetResourceReference(Border.BorderBrushProperty, "Brush_Foreground");
                child_border.BorderThickness = new Thickness(_border_thickness);
                child_border.CornerRadius = new CornerRadius(0);
                child_border.Margin = new Thickness(_margin);
                child_border.Child = child;

                var filter_row = new RowDefinition();
                filter_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(filter_row);
                Grid.SetRow(child_border, 4);
                content_grid.Children.Add(child_border);

                _content.SetResourceReference(Border.BackgroundProperty, "Brush_Background");
                _content.BorderThickness = new Thickness(_border_thickness);
                _content.SetResourceReference(Border.BorderBrushProperty, "Brush_Foreground");
                _content.CornerRadius = new CornerRadius(0);
                _content.Margin = new Thickness(_margin);
                _content.Child = content_grid;


                _timer.Stop();
                _created = true;
                return _created;
            }

            public UIElement GetUI()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }
                return _content;
            }

            /// <summary>
            /// Called on creation of filter
            /// </summary>
            /// <param name="content_metadata"></param>
            public void ContentMetadataListCallback(List<ContentMetadata> content_metadata)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return;
                }

                _visualizations_list.Children.Clear();

                foreach (var metadata in content_metadata)
                {
                    var check = new CheckBox();
                    check.SetBinding(CheckBox.ContentProperty, metadata.NameBinding);
                    check.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                    check.Tag = metadata.DataUID;
                    check.Click += _event_content_checked;
                    check.SetResourceReference(CheckBox.ForegroundProperty, "Brush_Foreground");
                    _visualizations_list.Children.Add(check);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void SetDirty(List<int> data_uids = null)
            {
                bool checked_content = false;
                if (data_uids == null)
                {
                    // If not data_uids are given, set all checked content "not considered" = false ...
                    foreach (int key in _checked_content_changes.Keys.ToList())
                    {
                        if (_checked_content_changes[key].Item2 == ListModification.ADD)
                        {
                            _checked_content_changes[key] = new ChangedContentApplyData_Type(false, ListModification.ADD, _checked_content_changes[key].Item3);
                            checked_content = true;
                        }
                    }
                }
                else
                {
                    // ... else set only checked content  "not considered" = false
                    foreach (var data_uid in data_uids)
                    {
                        if (_checked_content_changes.ContainsKey(data_uid) && (_checked_content_changes[data_uid].Item2 == ListModification.ADD))
                        {
                            _checked_content_changes[data_uid] = new ChangedContentApplyData_Type(false, ListModification.ADD, _checked_content_changes[data_uid].Item3);
                            checked_content = true;
                        }
                    }
                }

                if (checked_content)
                {
                    _set_dirty(true);
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected virtual UIElement create_ui()
            {
                throw new InvalidOperationException("Should be implemented by inheriting class.");
            }

            protected virtual void apply_filter(GenericDataStructure in_out_data)
            {
                throw new InvalidOperationException("Should be implemented by inheriting class.");
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void _event_content_checked(object sender, RoutedEventArgs e)
            {
                var checkbox = sender as CheckBox;
                if (checkbox == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }
                var data_uid = (int)checkbox.Tag;
                var is_checked = (bool)checkbox.IsChecked;

                if (_checked_content_changes.ContainsKey(data_uid))
                {
                    bool content_considered = _checked_content_changes[data_uid].Item1;

                    if ((is_checked && (_checked_content_changes[data_uid].Item2 == ListModification.DELETE)) ||
                        (!is_checked && (_checked_content_changes[data_uid].Item2 == ListModification.ADD)))
                    {
                        var mod = (_checked_content_changes[data_uid].Item2 == ListModification.ADD) ? (ListModification.DELETE) : (ListModification.ADD);
                        if (content_considered)
                        {
                            _checked_content_changes[data_uid] = new ChangedContentApplyData_Type(false, mod, _checked_content_changes[data_uid].Item3);
                        }
                        else
                        {
                            // Ignore changes reverted before having been applied
                            _checked_content_changes.Remove(data_uid);
                        }
                    }
                }
                else if (is_checked)
                {
                    _checked_content_changes[data_uid] = new ChangedContentApplyData_Type(false, ListModification.ADD, null);
                }

                bool considered = true;
                foreach (var content_tuple in _checked_content_changes)
                {
                    considered &= content_tuple.Value.Item1;
                }
                _set_dirty(!considered);
            }

            private void _event_apply_button(object sender, RoutedEventArgs e)
            {
                var sender_button = sender as Button;
                if (sender_button == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }

                bool filter_changed = false;

                foreach (int key in _checked_content_changes.Keys.ToList())
                {
                    var data_uid = key;

                    if (!_checked_content_changes[data_uid].Item1)
                    {
                        if (_checked_content_changes[data_uid].Item2 == ListModification.ADD)
                        {
                            var original_data = _get_selected_data_callback(data_uid);
                            original_data = original_data.DeepCopy(); /// XXX Required?

                            var filter_data = original_data.DeepCopy();
                            apply_filter(filter_data);
                            _update_selected_data_callback(data_uid, filter_data);

                            // Add original data only for the first time the filter is applied
                            var kept_original_data = _checked_content_changes[data_uid].Item3;
                            _checked_content_changes[data_uid] = new ChangedContentApplyData_Type(true, ListModification.ADD, ((kept_original_data == null) ? (original_data) : (kept_original_data)));
                            filter_changed = true;
                        }
                        else if (_checked_content_changes[data_uid].Item2 == ListModification.DELETE)
                        {
                            // Undo changes applied by the filter
                            if (_checked_content_changes[data_uid].Item3 != null)
                            {
                                _update_selected_data_callback(data_uid, _checked_content_changes[data_uid].Item3);
                                _checked_content_changes.Remove(data_uid);
                                filter_changed = true;
                            }
                            else
                            {
                                Log.Default.Msg(Log.Level.Error, "Missing original data");
                            }
                        }
                    }
                }

                // Notify filter manager on change
                if (filter_changed)
                {
                    _filter_changed_callback(_UID, _get_checked_content_list());
                }
                _set_dirty(false);
            }

            private void _set_dirty(bool dirty)
            {
                _apply_button.IsEnabled = dirty;
                if (dirty)
                {
                    _apply_button.SetResourceReference(Button.BackgroundProperty, "Brush_ApplyDirtyBackground");
                }
                else
                {
                    _apply_button.Background = _apply_button_default_background;
                }
            }

            private List<int> _get_checked_content_list()
            {
                var checked_content_lsit = new List<int>();

                foreach (int key in _checked_content_changes.Keys.ToList())
                {
                    if (_checked_content_changes[key].Item2 == ListModification.ADD)
                    {
                        checked_content_lsit.Add(key);
                    }
                }
                return checked_content_lsit;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private const double _margin = 5.0;
            private const double _border_thickness = 2.0;

            private bool _created = false;

            private Border _content = null;
            private RenameLabel _filter_caption = null;
            private StackPanel _visualizations_list = null;
            private Button _apply_button = null;
            private Brush _apply_button_default_background = null;

            private DataManager.UpdateSelectedDataCallback_Delegate _update_selected_data_callback = null;
            private DataManager.GetGenericDataCallback_Delegate _get_selected_data_callback = null;
            private FilterManager.FilterChanged_Delegate _filter_changed_callback = null;
            private FilterManager.DeleteFilterCallback_Delegate _delete_self_callback = null;

            // Track content selection changes
            private Dictionary<int, ChangedContentApplyData_Type> _checked_content_changes = null;

            #endregion
        }
    }
}
