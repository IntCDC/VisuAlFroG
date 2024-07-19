using System.Windows.Controls;
using System.Data;
using Core.Utilities;
using Core.Data;
using Visualizations.WPFInterface;
using Core.GUI;



/*
 *  Tabular raw data viewer
 * 
 * 
 */
namespace Visualizations
{
    public class WPF_DataViewer : AbstractWPFVisualization<ScrollViewer>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Raw Data Viewer"; } }
        public override bool _MultipleInstances { get { return false; } }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public WPF_DataViewer(int uid) : base(uid) { }

        public override bool CreateUI()
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }
            _timer.Start();


            _Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _Content.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _Content.SetResourceReference(ScrollViewer.BackgroundProperty, "Brush_Background");
            _Content.SetResourceReference(ScrollViewer.ForegroundProperty, "Brush_Foreground");
            _Content.SetResourceReference(StackPanel.BackgroundProperty, "Brush_Background");
            _Content.PreviewMouseWheel += event_scrollviewer_mousewheel;

            if (apply_data(out GenericDataStructure data))
            {
                create_table(data);

                var data_grid = new DataGrid();
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
                CreateUI();
            }
            else
            {
                if (!apply_data(out GenericDataStructure data))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }
                update_table_data(data);
            }
        }

        public override void AttachMenu(MenubarWindow menubar)
        {
            base.AttachMenu(menubar);

        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

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
            column.ColumnName = "BranchIdx";
            column.DefaultValue = uint.MaxValue;
            _table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "BranchLabel";
            column.DefaultValue = "";
            _table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(uint);
            column.ColumnName = "EntryIdx";
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

            for (uint i = 0; i < branch.GetDimension(); i++)
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
                row["EntryIdx"] = ei;
                row["EntryLabel"] = entry._Metadata._Label;
                row["Selected"] = entry._Metadata._Selected;
                row["BranchIdx"] = branch_index;
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
                var row = _table.Rows.Find((int)entry._Metadata._Index);
                if (row != null)
                {
                    row["Selected"] = entry._Metadata._Selected;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Missing entry at index: " + entry._Metadata._Index.ToString());
                }
            }
        }

        private string label_branchindex(uint i)
        {
            return "BranchLevel(" + i.ToString() + ")";
        }

        private string label_value(uint i)
        {
            return "Value" + i.ToString() + "";
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

        private DataTable _table = null;

        #endregion
    }
}
