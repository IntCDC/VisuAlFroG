using System;
using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections.Generic;
using Core.Data;
using Core.Abstracts;
using Visualizations.WPFInterface;
using Core.GUI;
using Core.Filter;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Data;
using System.Globalization;



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

        public override string _TypeName { get { return "Filter Editor (WPF)"; } }
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


            var add_button = new Button();
            string label = "  Add Filter  ";
            add_button.Content = label;
            var formattedText = new FormattedText(label, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), add_button.FontSize, Brushes.Black);
            var text_width = formattedText.Width;
            add_button.Width = text_width;
            add_button.Click += event_apply_button;

            /*
            add_button.FontWeight = FontWeights.Bold;
            add_button.SetResourceReference(Button.ForegroundProperty, "Brush_Foreground");

            // Invert background color on mouse over
            var button_style = new Style();
            button_style.TargetType = typeof(Button);

            Setter setter_background = new Setter();
            setter_background.Property = Button.BackgroundProperty;
            setter_background.Value = new DynamicResourceExtension("Brush_Background");
            button_style.Setters.Add(setter_background);

            var border = new FrameworkElementFactory(typeof(Border));

            var button_binding = new Binding("Background");
            button_binding.Mode = BindingMode.TwoWay;
            button_binding.Source = add_button;
            border.SetBinding(Border.BackgroundProperty, button_binding);

            var control_template = new ControlTemplate();
            control_template.TargetType = typeof(Button);
            control_template.VisualTree = border;

            Trigger trigger = new Trigger();
            trigger.Property = Button.IsMouseOverProperty;
            trigger.Value = true;

            Setter setter_trigger_b = new Setter();
            setter_trigger_b.Property = Button.BackgroundProperty;
            setter_trigger_b.Value = new DynamicResourceExtension("Brush_Foreground");
            trigger.Setters.Add(setter_trigger_b);

            control_template.Triggers.Add(trigger);

            Setter setter_template = new Setter();
            setter_template.Property = Button.TemplateProperty;
            setter_template.Value = control_template;

            button_style.Setters.Add(setter_template);
            add_button.Style = button_style;
            */

            var filter_list_scrolling = new ScrollViewer();
            filter_list_scrolling.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            filter_list_scrolling.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            filter_list_scrolling.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
            filter_list_scrolling.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
            filter_list_scrolling.PreviewMouseWheel += event_scrollviewer_mousewheel;

            _filter_list = new StackPanel();
            filter_list_scrolling.Content = _filter_list;

            var top_grid = new Grid();

            var column_label = new ColumnDefinition();
            column_label.Width = new GridLength(text_width);
            top_grid.ColumnDefinitions.Add(column_label);
            Grid.SetColumn(add_button, 0);
            top_grid.Children.Add(add_button);

            var column_list = new ColumnDefinition();
            column_list.Width = new GridLength(1.0, GridUnitType.Star);
            top_grid.ColumnDefinitions.Add(column_list);
            Grid.SetColumn(_add_filter_list, 1);
            top_grid.Children.Add(_add_filter_list);



            DockPanel.SetDock(top_grid, System.Windows.Controls.Dock.Top);
            _Content.Children.Add(top_grid);
            _Content.Children.Add(filter_list_scrolling);


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

        private ComboBox _add_filter_list = null;
        private StackPanel _filter_list = null;
        private FilterManager.CreateFilterCallback_Delegate _create_filter_callback = null;

        #endregion
    }
}
