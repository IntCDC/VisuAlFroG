using System;
using System.Windows.Controls;
using System.Data;
using Core.Utilities;
using Core.Data;
using Visualizations.WPFInterface;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using System.Data.Common;
using static Core.Utilities.Log;
using SciChart.Charting.Visuals.RenderableSeries;



/*
 *  Data Browser
 * 
 * Control Hierarchy: ScrollViewer(Content) -> StackPanel(_stack_panel) -> TextBlock,TextBlock,TreeViewItem(_tree_root)
 * 
 */
namespace Visualizations
{
    public class WPF_DataViewer : AbstractWPFVisualization<System.Windows.Controls.ScrollViewer>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string _Name { get { return "Data Viewer (WPF)"; } }
        public override bool _MultipleInstances { get { return false; } }


        /* ------------------------------------------------------------------*/
        // public functions

        public override bool Create()
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }
            _timer.Start();


            _Content.Name = _ID;
            _Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _Content.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _Content.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
            _Content.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
            _Content.PreviewMouseWheel += event_scrollviewer_mousewheel;
            _Content.SetResourceReference(StackPanel.BackgroundProperty, "Brush_Background");

            if (GetData(out GenericDataStructure data))
            {
                create_table(data);

                System.Windows.Controls.DataGrid data_grid = new System.Windows.Controls.DataGrid();
                data_grid.IsReadOnly = true;
                data_grid.AutoGenerateColumns = true;
                data_grid.ItemsSource = _table.AsDataView();
                _Content.Content = data_grid;
            }


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

            if (new_data)
            {
                // Re-creation of content is required for new data
                Create();
            }
            else
            {
                if (!GetData(out GenericDataStructure data))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }
                update_table_data(data);
            }
        }


        /* ------------------------------------------------------------------*/
        // private functions

        private void create_table(GenericDataStructure branch)
        {
            if (_table != null)
            {
                _table.Clear();
                _table.PrimaryKey = null;
            }
            _table = new DataTable();

            var column = new DataColumn();
            column.DataType = typeof(uint);
            column.ColumnName = "Index";
            column.Unique = true;
            column.DefaultValue = uint.MaxValue;
            _table.Columns.Add(column);
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = _table.Columns["Index"];
            _table.PrimaryKey = PrimaryKeyColumns;

            /// create_level_columns(branch, 1);

            column = new DataColumn();
            column.DataType = typeof(uint);
            column.ColumnName = "BranchIndex";
            column.DefaultValue = uint.MaxValue;
            _table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "BranchLabel";
            column.DefaultValue = "";
            _table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(uint);
            column.ColumnName = "EntryIndex";
            column.DefaultValue = uint.MaxValue;
            _table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "EntryLabel";
            column.DefaultValue = "";
            _table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(bool);
            column.ColumnName = "Selected";
            column.DefaultValue = false;
            _table.Columns.Add(column);

            for (uint i = 0; i < branch.Dimension(); i++)
            {
                column = new DataColumn();
                column.DataType = typeof(double);
                column.ColumnName = label_value(i);
                column.DefaultValue = double.MinValue;
                _table.Columns.Add(column);
            }

            create_data_rows(branch, 0, 0);
        }

        private void create_level_columns(GenericDataStructure branch, uint branch_level)
        {
            string level_str = label_branchindex(branch_level);

            if ((branch._Branches.Count > 0) && !_table.Columns.Contains(level_str))
            {
                var column = new DataColumn();
                column.DataType = typeof(string);
                column.ColumnName = level_str;
                column.DefaultValue = "";
                _table.Columns.Add(column);
            }

            foreach (var b in branch._Branches)
            {
                create_level_columns(b, branch_level);
            }
        }

        private void create_data_rows(GenericDataStructure branch, uint branch_level, uint branch_index)
        {
            uint bi = 0;
            foreach (var b in branch._Branches)
            {
                create_data_rows(b, branch_level + 1, bi);
                bi++;
            }

            string level_str = label_branchindex(branch_level);
            uint ei = 0;
            foreach (var entry in branch._Entries)
            {
                var row = _table.NewRow();
                row["Index"] = entry._Metadata._Index;
                row["EntryIndex"] = ei;
                row["EntryLabel"] = entry._Metadata._Label;
                row["Selected"] = entry._Metadata._Selected;
                row["BranchIndex"] = branch_index;
                row["BranchLabel"] = branch._Label;
                /// row[level_str] = "X";

                for (int i = 0; i < entry._Values.Count; i++)
                {
                    row[label_value((uint)i)] = entry._Values[i];
                }

                _table.Rows.Add(row);
                ei++;
            }
        }

        private void update_table_data(GenericDataStructure branch)
        {

            foreach (var b in branch._Branches)
            {
                update_table_data(b);
            }
            foreach (var entry in branch._Entries)
            {
                _table.Rows[(int)entry._Metadata._Index]["Selected"] = entry._Metadata._Selected;
            }
        }

        private string label_branchindex(uint i)
        {
            return "BranchLevel(" + i.ToString() + ")";
        }

        private string label_value(uint i)
        {
            return "Value(" + i.ToString() + ")";
        }

        private void event_scrollviewer_mousewheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            scv.UpdateLayout();
            e.Handled = true;
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private System.Data.DataTable _table = null;

    }
}
