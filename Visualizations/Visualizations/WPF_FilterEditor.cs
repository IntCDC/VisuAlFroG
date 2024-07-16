using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Core.Abstracts;
using Core.Filter;
using Core.GUI;
using Core.Utilities;

using Visualizations.WPFInterface;



/*
 * Filter Editor Window Content
 * 
 */
namespace Visualizations
{
    public class WPF_FilterEditor : AbstractWPFVisualization<DockPanel>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Filter Editor"; } }
        public override bool _MultipleInstances { get { return false; } }

        // Indicates to not create an unused copy of the data
        public override Type _RequiredDataType { get; } = null;

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public WPF_FilterEditor(int uid) : base(uid) { }

        public override bool CreateUI()
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }
            if (_created)
            {
                // Log Console does not depend on data
                Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                return false;
            }
            _timer.Start();


            _add_filter_list = new ComboBox();
            _add_filter_list.IsEditable = false;
            _add_filter_list.DisplayMemberPath = "Name";
            _add_filter_list.SelectedIndex = 0;
            _add_filter_list.Margin = new Thickness(0.0, _margin, _margin, _margin);

            var add_button = new Button();
            add_button.Content = " Add Filter ";
            add_button.Click += event_apply_button;
            add_button.Margin = new Thickness(_margin, _margin/2.0, _margin, _margin/2.0);

            _list_scrolling = new ScrollViewer();
            _list_scrolling.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _list_scrolling.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _list_scrolling.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
            _list_scrolling.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
            _list_scrolling.PreviewMouseWheel += event_scrollviewer_mousewheel;

            _filter_list = new StackPanel();
            _list_scrolling.Content = _filter_list;

            var note = new TextBlock();
            note.Text = "Filters are applied sequentially from top to bottom.";
            note.SetResourceReference(TextBlock.BackgroundProperty, "Brush_Background");
            note.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Foreground");
            note.Margin = new Thickness(_margin);

            var note_border = new Border();
            note_border.Child = note;
            note_border.SetResourceReference(Border.BackgroundProperty, "Brush_Background");
            note_border.BorderThickness = new Thickness(0.0, 0.0, 0.0, _border_thickness);
            note_border.SetResourceReference(Border.BorderBrushProperty, "Brush_Foreground");
            note_border.CornerRadius = new CornerRadius(0);

            note_border.Margin = new Thickness(0.0, 0.0, 0.0, _margin);

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

            var top_row = new RowDefinition();
            top_row.Height = new GridLength(1.0, GridUnitType.Star);
            caption_grid.RowDefinitions.Add(top_row);
            Grid.SetRow(add_grid, 0);

            caption_grid.Children.Add(add_grid);

            var row_note = new RowDefinition();
            row_note.Height = new GridLength(1.0, GridUnitType.Star);
            caption_grid.RowDefinitions.Add(row_note);
            Grid.SetRow(note_border, 1);
            caption_grid.Children.Add(note_border);

            DockPanel.SetDock(caption_grid, System.Windows.Controls.Dock.Top);
            _Content.Children.Add(caption_grid);
            _Content.Children.Add(_list_scrolling);


            _timer.Stop();
            _created = true;
            return _created;
        }

        public override void Update(bool new_data)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }

            // Unused
        }

        public override bool Terminate()
        {
            if (_initialized)
            {
                _add_filter_list = null;
                _list_scrolling = null;

                _filter_list.Children.Clear();
                _filter_list = null;

                _create_filter_callback = null;

                _initialized = false;
            }
            return base.Terminate();
        }

        public override void AttachMenu(MenubarWindow menubar)
        {
            base.AttachMenu(menubar);
        }

        public void SetCreateFilterCallback(FilterManager.CreateFilterCallback_Delegate create_filter_callback)
        {
            _create_filter_callback = create_filter_callback;
        }

        public void UpdateFilterTypeList(List<FilterManager.FilterListMetadata> filters)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }
            _add_filter_list.ItemsSource = filters;
        }

        public void ModifyUIFilterList(UIElement element, AbstractFilter.ListModification mod)
        {
            switch (mod)
            {
                case (AbstractFilter.ListModification.ADD):
                    {
                        _filter_list.Children.Add(element);
                        _list_scrolling.ScrollToBottom();
                    }
                    break;
                case (AbstractFilter.ListModification.DELETE):
                    {
                        _filter_list.Children.Remove(element);
                    }
                    break;
                default: break;
            }
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        private void event_apply_button(object sender, RoutedEventArgs e)
        {
            var apply_button = sender as Button;
            if (apply_button == null)
            {
                Log.Default.Msg(Log.Level.Error, "Unexpected sender.");
                return;
            }
            var selected_item = _add_filter_list.SelectedItem as FilterManager.FilterListMetadata;
            if (selected_item == null)
            {
                Log.Default.Msg(Log.Level.Warn, "Select filter type before trying to add a new filter.");
                return;
            }
            if (_create_filter_callback == null)
            {
                Log.Default.Msg(Log.Level.Error, "Missing callback to create new filter type.");
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

        private const double _margin = 5.0;
        private const double _border_thickness = 2.0;

        private ComboBox _add_filter_list = null;
        private ScrollViewer _list_scrolling = null;
        private StackPanel _filter_list = null;
        private FilterManager.CreateFilterCallback_Delegate _create_filter_callback = null;

        #endregion
    }
}
