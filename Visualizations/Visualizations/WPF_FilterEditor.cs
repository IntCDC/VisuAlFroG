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



/*
 * Filter Editor Window Content
 * 
 */
namespace Visualizations
{
    public class FilterListMetadata
    {
        public string _ID { get; set; }
        public string _Name { get; set; }
        public string _Type { get; set; }
    }


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
            _add_filter_list.SelectionChanged += event_add_filter_list_changed;

            var list_label = new TextBlock();
            list_label.SetResourceReference(TextBlock.BackgroundProperty, "Brush_Background");
            list_label.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Foreground");
            list_label.FontWeight = FontWeights.Bold;
            list_label.Text = "  Add Filter  ";

            _filter_list_content = new ScrollViewer();
            _filter_list_content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _filter_list_content.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _filter_list_content.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
            _filter_list_content.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
            _filter_list_content.SetResourceReference(StackPanel.BackgroundProperty, "Brush_Background");
            _filter_list_content.PreviewMouseWheel += event_scrollviewer_mousewheel;

            var top_grid = new Grid();
            var column_label = new ColumnDefinition();
            column_label.Width = new GridLength(0.0, GridUnitType.Auto);
            top_grid.ColumnDefinitions.Add(column_label);
            var column_list = new ColumnDefinition();
            column_list.Width = new GridLength(1.0, GridUnitType.Star);
            top_grid.ColumnDefinitions.Add(column_list);
            Grid.SetColumn(list_label, 0);
            top_grid.Children.Add(list_label);
            Grid.SetColumn(_add_filter_list, 1);
            top_grid.Children.Add(_add_filter_list);

            DockPanel.SetDock(top_grid, System.Windows.Controls.Dock.Top);
            _Content.Children.Add(top_grid);
            _Content.Children.Add(_filter_list_content);


            _timer.Stop();
            _created = true;
            return _created;
        }

        private void event_add_filter_list_changed(object sender, SelectionChangedEventArgs e)
        {
            var combo_box = sender as ComboBox;
            if (combo_box == null)
            {
                Log.Default.Msg(Log.Level.Error, "Unexpected sender.");
                return;
            }
            var selected_item = combo_box.SelectedItem as FilterListMetadata;

            // selected_item._Type;
            //Person selectedPerson = (Person)myComboBox.SelectedItem;
            //int personID = selectedPerson.PersonID;

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
            return base.Terminate();
        }

        public override void AttachMenu(MenubarWindow menubar)
        {
            base.AttachMenu(menubar);
        }

        public void UpdateFilterListCallback(List<FilterListMetadata> filters)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }

            _add_filter_list.ItemsSource = filters;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

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
        private ScrollViewer _filter_list_content = null;


        #endregion
    }
}
