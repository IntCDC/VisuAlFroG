using System;
using System.Windows.Controls;
using System.Windows;
using Core.Utilities;
using Core.Data;
using Visualizations.WPFInterface;



/*
 *  Data Browser
 * 
 */
namespace Visualizations
{
    public class DataConfigurator : AbstractGenericVisualization<System.Windows.Controls.StackPanel>
    {
        /* ------------------------------------------------------------------*/
        // properties

        public override string Name { get { return "Data Configurator"; } }


        /* ------------------------------------------------------------------*/
        // public functions

        public override bool ReCreate()
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }
            if (_created)
            {
                Log.Default.Msg(Log.Level.Warn, "Re-creating visualization");
                _created = false;
            }
            if (this.RequestDataCallback == null)
            {
                Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                return false;
            }
            _timer.Start();


            GenericDataStructure data = null;
            if (!this.GetData(ref data))
            {
                Log.Default.Msg(Log.Level.Error, "Missing data");
                return false;
            }


            Content.SetResourceReference(StackPanel.BackgroundProperty, "Brush_Background");

            var text_dim = new TextBlock();
            text_dim.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Foreground");
            text_dim.Text = "Data Dimensionality: " + data.DataDimension().ToString();
            Content.Children.Add(text_dim);

            var text_value_types = new TextBlock();
            text_value_types.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Foreground");
            text_value_types.Text = "Data Value Type(s): ";
            foreach (var value_type in data.ValueTypes())
            {
                text_value_types.Text += value_type.ToString() + " ";
            }
            Content.Children.Add(text_value_types);

            _tree_root = new TreeViewItem();
            _tree_root.Header = "Data Root";
            _tree_root.IsExpanded = true;
            _tree_root.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");
            create_data_tree(data, _tree_root);
            Content.Children.Add(_tree_root);


            _timer.Stop();
            _created = true;
            return _created;
        }

        public override bool Update()
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return false;
            }

            GenericDataStructure data = null;
            if (!this.GetData(ref data))
            {
                Log.Default.Msg(Log.Level.Error, "Missing data");
                return false;
            }

            update_metadata(data);

            return true;
        }

        /// <summary>
        /// DEBUG
        /// </summary>
        ~DataConfigurator()
        {
            Console.WriteLine("DEBUG - DTOR: DataConfigurator");
        }

        /* ------------------------------------------------------------------*/
        // private functions

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tree_item"></param>
        private void create_data_tree(GenericDataStructure data, TreeViewItem tree_item)
        {
            int entry_index = 0;
            foreach (var entry in data.Entries)
            {
                var tree_entry = new TreeViewItem();
                tree_entry.Header = "Entry [" + entry_index.ToString() + "]";
                tree_entry.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");

                var tree_values = new TreeViewItem();
                tree_values.Header = "Values";
                tree_values.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");

                //Align child items horizontal
                var panel_template = new ItemsPanelTemplate();
                var stack_panel = new FrameworkElementFactory(typeof(StackPanel));
                stack_panel.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
                panel_template.VisualTree = stack_panel;
                tree_values.ItemsPanel = panel_template;

                foreach (var value in entry.Values)
                {
                    var tree_value = new TreeViewItem();
                    tree_value.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");

                    tree_value.Header = value.ToString();
                    tree_value.MouseDoubleClick += event_treevalue_clicked;
                    tree_value.Tag = entry.MetaData;
                    tree_values.Items.Add(tree_value);
                }
                var tree_meta = new TreeViewItem();
                tree_meta.Header = "Meta Data";
                tree_meta.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");

                var tree_index = new TreeViewItem();
                tree_index.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");
                tree_index.Header = "Index";
                var tree_index_value = new TreeViewItem();
                tree_index_value.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");
                tree_index_value.Header = entry.MetaData.Index.ToString();
                tree_index.Items.Add(tree_index_value);

                var tree_selected = new TreeViewItem();
                tree_selected.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");
                tree_selected.Header = "IsSelected";
                var tree_selected_value = new TreeViewItem();
                tree_selected_value.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");
                tree_selected_value.Header = entry.MetaData.IsSelected.ToString();
                // Set index of IsSelected to index of value to find it later
                tree_selected_value.Name = "index_" + entry.MetaData.Index.ToString();
                tree_selected.Items.Add(tree_selected_value);

                tree_meta.Items.Add(tree_index);
                tree_meta.Items.Add(tree_selected);

                tree_entry.Items.Add(tree_values);
                tree_entry.Items.Add(tree_meta);

                tree_item.Items.Add(tree_entry);
                entry_index++;
            }

            int branch_index = 0;
            foreach (var branch in data.Branches)
            {
                var tree_branch = new TreeViewItem();
                tree_branch.Header = "Branch [" + branch_index.ToString() + "]";
                tree_branch.SetResourceReference(TreeViewItem.ForegroundProperty, "Brush_Foreground");

                create_data_tree(branch, tree_branch);

                tree_item.Items.Add(tree_branch);
                branch_index++;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_treevalue_clicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treevalue = sender as TreeViewItem;
            if (treevalue != null)
            {
                var meta_data = treevalue.Tag as MetaData;
                if (meta_data != null)
                {
                    meta_data.IsSelected = !meta_data.IsSelected;
                    update_metadata_at_index(_tree_root, meta_data);
                    return;
                }
            }
            Log.Default.Msg(Log.Level.Error, "Failed to read meta data of selected tree item");
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="meta_data"></param>
        private void update_metadata_at_index(TreeViewItem tree, MetaData meta_data)
        {
            foreach (var treeobject in tree.Items)
            {
                var treeitem = treeobject as TreeViewItem;
                if (treeitem != null)
                {
                    // IsSeleceted TreeViewItem of value with index
                    if (treeitem.Name == ("index_" + meta_data.Index.ToString()))
                    {
                        treeitem.Header = meta_data.IsSelected.ToString();
                        return;
                    }
                    update_metadata_at_index(treeitem, meta_data);
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="branch"></param>
        private void update_metadata(GenericDataStructure branch)
        {
            foreach (var b in branch.Branches)
            {
                update_metadata(b);
            }
            foreach (var entry in branch.Entries)
            {
                update_metadata_at_index(_tree_root, entry.MetaData);
            }
        }

        /* ------------------------------------------------------------------*/
        // private variables

        private TreeViewItem _tree_root = null;

    }
}
