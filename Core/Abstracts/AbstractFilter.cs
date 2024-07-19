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
using System.Windows.Data;



/*
 * Abstract data filter
 * 
 */
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
            public bool _UniqueContent { get; protected set; } = false;

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractFilter() { }

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

                _applied = new Dictionary<int, Tuple<ListModification, GenericDataStructure>>();
                _reserved = new Dictionary<int, Tuple<ListModification, GenericDataStructure>>();


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
                    // Revert all data changes
                    foreach (var item in _applied)
                    {
                        _update_selected_data_callback(item.Key, item.Value.Item2);
                    }

                    // Notify subsequent filters that this filter has changed (= will be deleted)
                    var changed_content_list = new List<int>();
                    foreach (var item in _applied)
                    {
                        changed_content_list.Add(item.Key);
                    }
                    foreach (var item in _reserved)
                    {
                        if (item.Value.Item1 == ListModification.DELETE)
                        {
                            changed_content_list.Add(item.Key);
                        }
                    }
                    if (changed_content_list.Count > 0)
                    {
                        _filter_changed_callback(_UID, changed_content_list);
                    }

                    _content = null;
                    _filter_caption = null;
                    _checkable_content_list = null;
                    _apply_button = null;

                    _update_selected_data_callback = null;
                    _get_selected_data_callback = null;
                    _filter_changed_callback = null;
                    _delete_self_callback = null;

                    _applied.Clear();
                    _applied = null;
                    _reserved.Clear();
                    _reserved = null;

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
                _checkable_content_list = new StackPanel();
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

                var list_caption_grid = new Grid();

                var list_label = new TextBlock();
                list_label.Text = "Select Content";
                list_label.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                var apply_text_column = new ColumnDefinition();
                apply_text_column.Width = new GridLength(1.0, GridUnitType.Auto);
                list_caption_grid.ColumnDefinitions.Add(apply_text_column);
                Grid.SetColumn(_apply_button, 0);
                list_caption_grid.Children.Add(list_label);

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
                list_caption_grid.ColumnDefinitions.Add(apply_rule_column);
                Grid.SetColumn(hrule, 1);
                list_caption_grid.Children.Add(hrule);

                var content_grid = new Grid();

                int row_index = -1;
                var caption_row = new RowDefinition();
                caption_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(caption_row);
                Grid.SetRow(_filter_caption, ++row_index);
                content_grid.Children.Add(_filter_caption);

                var button_row = new RowDefinition();
                button_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(button_row);
                Grid.SetRow(button_grid, ++row_index);
                content_grid.Children.Add(button_grid);

                var list_caption_row = new RowDefinition();
                list_caption_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(list_caption_row);
                Grid.SetRow(list_caption_grid, ++row_index);
                content_grid.Children.Add(list_caption_grid);

                if (_UniqueContent)
                {
                    var unique_label = new TextBlock();
                    unique_label.Text = "(filter only allows individual selection)";
                    unique_label.TextWrapping = TextWrapping.Wrap;
                    unique_label.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                    var unique_label_row = new RowDefinition();
                    unique_label_row.Height = new GridLength(1.0, GridUnitType.Auto);
                    content_grid.RowDefinitions.Add(unique_label_row);
                    Grid.SetRow(unique_label, 3);
                    content_grid.Children.Add(unique_label);

                    row_index++;
                }

                var list_row = new RowDefinition();
                list_row.Height = new GridLength(1.0, GridUnitType.Auto);
                content_grid.RowDefinitions.Add(list_row);
                Grid.SetRow(_checkable_content_list, ++row_index);
                content_grid.Children.Add(_checkable_content_list);


                var child = create_update_ui(null);

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
                Grid.SetRow(child_border, ++row_index);
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

                _checkable_content_list.Children.Clear();
                foreach (var metadata in content_metadata)
                {
                    var check = new CheckBox();
                    check.SetBinding(CheckBox.ContentProperty, metadata.NameBinding);
                    check.Margin = new Thickness(_margin, _margin, 0.0, 0.0);
                    check.Tag = metadata.DataUID;
                    check.Click += _event_content_checked;
                    check.SetResourceReference(CheckBox.ForegroundProperty, "Brush_Foreground");
                    _checkable_content_list.Children.Add(check);
                }
            }

            /// <summary>
            /// Force apply changes when previous filter has been changed
            /// </summary>
            /// <param name="data_uids"></param>
            public void SubsequentApply(List<int> data_uids)
            {
                foreach (var data_uid in data_uids)
                {
                    if (_reserved.ContainsKey(data_uid) && (_reserved[data_uid].Item1 == ListModification.DELETE))
                    {
                        _reserved.Remove(data_uid);
                    }
                    else if (!_reserved.ContainsKey(data_uid) && _applied.ContainsKey(data_uid))
                    {
                        _reserved.Add(data_uid, new Tuple<ListModification, GenericDataStructure>(ListModification.ADD, null));
                    }
                }
                // notify_changes = false prevents double execution
                _apply_changes(false);
            }

            public void SetDirty()
            {
                foreach (var item in _applied)
                {
                    if (!_reserved.ContainsKey(item.Key))
                    {
                        _reserved.Add(item.Key, new Tuple<ListModification, GenericDataStructure>(ListModification.ADD, null));
                    }
                }
                if (_reserved.Count > 0)
                {
                    _set_dirty(true);
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected virtual UIElement create_update_ui(in GenericDataStructure in_data)
            {
                throw new InvalidOperationException("Should be implemented by inheriting class.");
            }

            protected virtual void apply_filter(GenericDataStructure out_data)
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

                if (is_checked)
                {
                    if (_reserved.ContainsKey(data_uid))
                    {
                        if (_reserved[data_uid].Item1 == ListModification.DELETE)
                        {
                            // If content should be removed and now added ... remove from reserved list
                            _reserved.Remove(data_uid);
                        }
                        // If content has already been reserved for being added, don't do anything. This case can not happen!
                    }
                    else // !_reserved.ContainsKey(data_uid)
                    {
                        if (_applied.ContainsKey(data_uid))
                        {
                            if (_applied[data_uid].Item1 == ListModification.ADD)
                            {
                                // If content has previously already been applied, don't reserve to be reapplied
                            }
                            // If content has already been applied for being delete, don't do anything. This case can not happen (there won't be an entry for this case in _applied)!
                        }
                        else
                        {
                            _reserved.Add(data_uid, new Tuple<ListModification, GenericDataStructure>(ListModification.ADD, null));
                        }
                    }
                    _disable_content_checkboxes(data_uid);
                }
                else
                {
                    if (_reserved.ContainsKey(data_uid))
                    {
                        if (_reserved[data_uid].Item1 == ListModification.ADD)
                        {
                            // If content should be added and now removed ... remove from reserved list
                            _reserved.Remove(data_uid);
                        }
                        // If content has already been reserved for being delete, don't do anything. This case can not happen!
                    }
                    else // !_reserved.ContainsKey(data_uid)
                    {
                        if (_applied.ContainsKey(data_uid))
                        {
                            if (_applied[data_uid].Item1 == ListModification.ADD)
                            {
                                // If content has previously already been applied, reserve content for deletion
                                _reserved.Add(data_uid, new Tuple<ListModification, GenericDataStructure>(ListModification.DELETE, _applied[data_uid].Item2));
                            }
                            // If content has already been applied for being delete, don't do anything. This case can not happen (there won't be an entry for this case in _applied)!
                        }
                        else
                        {
                            // If content has not been applied, don't do anything. 
                        }
                    }
                    _enable_content_checkboxes();
                }

                bool dirty = !(_reserved.Count == 0);
                if (_reserved.Count > 0)
                {
                    dirty = (_applied.Count != _reserved.Count);
                    foreach (var key_applied in _applied.Keys.ToList())
                    {
                        if (!_reserved.ContainsKey(key_applied))
                        {
                            dirty = true;
                        }
                        else if (_reserved[key_applied].Item1 != _applied[key_applied].Item1)
                        {
                            dirty = true;
                        }
                    }
                    foreach (var key_reserved in _reserved.Keys.ToList())
                    {
                        if (!_applied.ContainsKey(key_reserved))
                        {
                            dirty = true;
                        }
                        else if (_reserved[key_reserved].Item1 != _applied[key_reserved].Item1)
                        {
                            dirty = true;
                        }
                    }
                }
                _set_dirty(dirty);
            }

            private void _apply_changes(bool notify_changes = true)
            {
                _applied.Clear();
                var changed_content_list = new List<int>();

                var save_border_thickness = _content.BorderThickness;
                var save_border_color = _content.BorderBrush;
                _content.BorderThickness = new Thickness(_border_thickness * 3.0);
                _content.SetResourceReference(Border.BorderBrushProperty, "Brush_ApplyDirtyBackground");
                Miscellaneous.AllowGlobalUIUpdate();

                foreach (var item in _reserved)
                {
                    var data_uid = item.Key;

                    if (item.Value.Item1 == ListModification.ADD)
                    {
                        var original_data = _get_selected_data_callback(data_uid);
                        original_data = original_data.DeepCopy();

                        var filter_data = original_data.DeepCopy();
                        apply_filter(filter_data);
                        _update_selected_data_callback(data_uid, filter_data);

                        var applied_data = new Tuple<ListModification, GenericDataStructure>(ListModification.ADD, original_data);

                        _applied.Add(data_uid, applied_data);
                        changed_content_list.Add(data_uid);
                    }
                    else // if (item.Value.Item1 == ListModification.DELETE)
                    {
                        // Undo changes applied by the filter
                        if (item.Value.Item2 != null)
                        {
                            _update_selected_data_callback(data_uid, item.Value.Item2);
                            _enable_content_checkboxes();
                            changed_content_list.Add(data_uid);
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Missing original data to revert filter");
                        }
                    }
                }
                _set_dirty(false);

                _content.BorderThickness = save_border_thickness;
                _content.BorderBrush = save_border_color;
                Miscellaneous.AllowGlobalUIUpdate();

                if (notify_changes)
                {
                    // Notify subsequent filters on change
                    if (changed_content_list.Count > 0)
                    {
                        _filter_changed_callback(_UID, changed_content_list);
                    }
                }
                _reserved.Clear();
            }

            private void _event_apply_button(object sender, RoutedEventArgs e)
            {
                var sender_button = sender as Button;
                if (sender_button == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }
                _apply_changes();
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

            private void _enable_content_checkboxes()
            {
                // Only for unique content:
                // Enable all other content for being selectable again
                if (_UniqueContent)
                {
                    foreach (var child in _checkable_content_list.Children)
                    {
                        var content_item = child as CheckBox;
                        if (content_item == null)
                        {
                            Log.Default.Msg(Log.Level.Error, "Unexpected item type");
                            continue;
                        }
                        content_item.IsEnabled = true;
                    }
                }
            }

            private void _disable_content_checkboxes(int data_uid)
            {
                // Only for unique content:
                // Disable all other content and request update of filter UI
                if (_UniqueContent)
                {
                    var original_data = _get_selected_data_callback(data_uid);
                    original_data = original_data.DeepCopy();
                    create_update_ui(original_data);

                    foreach (var child in _checkable_content_list.Children)
                    {
                        var content_item = child as CheckBox;
                        if (content_item == null)
                        {
                            Log.Default.Msg(Log.Level.Error, "Unexpected item type");
                            continue;
                        }
                        if ((int)content_item.Tag != data_uid)
                        {
                            content_item.IsEnabled = false;
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private const double _margin = 5.0;
            private const double _border_thickness = 2.0;

            private bool _created = false;

            // Required to be available in cotr of derived filter
            private RenameLabel _filter_caption = new RenameLabel();

            private Border _content = null;
            private StackPanel _checkable_content_list = null;
            private Button _apply_button = null;
            private Brush _apply_button_default_background = null;

            private DataManager.UpdateSelectedDataCallback_Delegate _update_selected_data_callback = null;
            private DataManager.GetGenericDataCallback_Delegate _get_selected_data_callback = null;
            private FilterManager.FilterChanged_Delegate _filter_changed_callback = null;
            private FilterManager.DeleteFilterCallback_Delegate _delete_self_callback = null;

            // Track content selection changes
            private Dictionary<int, Tuple<ListModification, GenericDataStructure>> _applied = null;
            private Dictionary<int, Tuple<ListModification, GenericDataStructure>> _reserved = null;

            #endregion
        }
    }
}
