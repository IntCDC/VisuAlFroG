using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using Core.Abstracts;
using Core.Utilities;
using System.Windows.Forms;
using System.Runtime.Remoting.Contexts;
using System.Collections.ObjectModel;
using Visualizations.Abstracts;
using Visualizations.Management;
using Core.GUI;
using Visualizations.Interaction;
using System.Windows.Markup;


/*
 *  Content for Data Filtering
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class DataBrowser : AbstractGenericVisualization<System.Windows.Controls.TreeView, GenericDataBranch>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Data Browser"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Create()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Info, "Re-creating content");
                    _created = false;
                }
                if (_request_data_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();

                var data = Data();
                if (data == null)
                {
                    return false;
                }
                var tree_root = new TreeViewItem();
                tree_root.Header = "Data Root";
                tree_root.IsExpanded = true;

                traverse_data(data, tree_root);

                Content.Items.Add(tree_root);
                Content.Background = ColorTheme.GenericBackground;
                Content.Foreground = ColorTheme.GenericForeground;

                _timer.Stop();
                _created = true;
                return _created;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void traverse_data(GenericDataBranch data, TreeViewItem tree_item)
            {

                int leaf_index = 0;
                foreach (var leaf in data.Leafs)
                {
                    var tree_leaf = new TreeViewItem();
                    tree_leaf.Header = "Leaf [" + leaf_index.ToString() + "]";

                    var tree_values = new TreeViewItem();
                    tree_values.Header = "Values";
                    //Align child items horizontal
                    var panel_template = new ItemsPanelTemplate();
                    var stack_panel = new FrameworkElementFactory(typeof(StackPanel));
                    stack_panel.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
                    panel_template.VisualTree = stack_panel;
                    tree_values.ItemsPanel = panel_template;

                    foreach (var value in leaf.Values)
                    {
                        var tree_value = new TreeViewItem();
                        tree_value.Header = value.ToString();
                        tree_values.Items.Add(tree_value);
                    }
                    var tree_meta = new TreeViewItem();
                    tree_meta.Header = "Meta Data";

                    var tree_index = new TreeViewItem();
                    tree_index.Header = "Index";
                    var tree_index_value = new TreeViewItem();
                    tree_index_value.Header = leaf.MetaData.Index.ToString();
                    tree_index.Items.Add(tree_index_value);

                    var tree_selected = new TreeViewItem();
                    tree_selected.Header = "IsSelected";
                    var tree_selected_value = new TreeViewItem();
                    tree_selected_value.Header = leaf.MetaData.IsSelected.ToString();
                    tree_selected.Items.Add(tree_selected_value);

                    tree_meta.Items.Add(tree_index);
                    tree_meta.Items.Add(tree_selected);

                    tree_leaf.Items.Add(tree_values);
                    tree_leaf.Items.Add(tree_meta);

                    tree_item.Items.Add(tree_leaf);
                    leaf_index++;
                }

                int branch_index = 0;
                foreach (var branch in data.Branches)
                {
                    var tree_branch = new TreeViewItem();
                    tree_branch.Header = "Branch [" + branch_index.ToString() + "]";

                    traverse_data(branch, tree_branch);

                    tree_item.Items.Add(tree_branch);
                    branch_index++;
                }
            }
        }
    }
}
