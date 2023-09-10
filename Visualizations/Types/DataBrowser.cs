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
using Core.GUI;
using Visualizations.Interaction;
using System.Windows.Markup;
using System.Globalization;
using Visualizations.Data;



/*
 *  Data Browser
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class DataBrowser : AbstractGenericVisualization<System.Windows.Controls.TreeView, GenericDataInterface<GenericDataStructure>>
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
                if (Data.RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();


                GenericDataStructure data = null;
                if (!Data.Set(data))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data");
                    return false;
                }

                _tree_root = new TreeViewItem();
                _tree_root.Header = "Data Root";
                _tree_root.IsExpanded = true;

                Content.Items.Add(_tree_root);
                Content.Background = ColorTheme.GenericBackground;
                Content.Foreground = ColorTheme.GenericForeground;

                ///TODO create_data_tree(Data.???, _tree_root);


                _timer.Stop();
                _created = true;
                return _created;
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

                    var tree_values = new TreeViewItem();
                    tree_values.Header = "Values";
                    //Align child items horizontal
                    var panel_template = new ItemsPanelTemplate();
                    var stack_panel = new FrameworkElementFactory(typeof(StackPanel));
                    stack_panel.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
                    panel_template.VisualTree = stack_panel;
                    tree_values.ItemsPanel = panel_template;

                    foreach (var value in entry.Values)
                    {
                        var tree_value = new TreeViewItem();
                        tree_value.Header = value.ToString();
                        tree_value.MouseDoubleClick += treevalue_clicked;
                        tree_value.Tag = entry.MetaData;
                        tree_values.Items.Add(tree_value);
                    }
                    var tree_meta = new TreeViewItem();
                    tree_meta.Header = "Meta Data";

                    var tree_index = new TreeViewItem();
                    tree_index.Header = "Index";
                    var tree_index_value = new TreeViewItem();
                    tree_index_value.Header = entry.MetaData.Index.ToString();
                    tree_index.Items.Add(tree_index_value);

                    var tree_selected = new TreeViewItem();
                    tree_selected.Header = "IsSelected";
                    var tree_selected_value = new TreeViewItem();
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
            private void treevalue_clicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                var treevalue = sender as TreeViewItem;
                if (treevalue != null)
                {
                    var meta_data = treevalue.Tag as MetaData;
                    if (meta_data != null)
                    {
                        meta_data.IsSelected = !meta_data.IsSelected;
                        change_metadata(_tree_root, meta_data.Index, meta_data.IsSelected);
                        return;
                    }
                }
                Log.Default.Msg(Log.Level.Error, "Failed to read meta data of selected tree item");
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="tree"></param>
            /// <param name="value_index"></param>
            /// <param name="is_selected"></param>
            private void change_metadata(TreeViewItem tree, int metadata_index, bool metadata_is_selected)
            {
                foreach (var treeobject in tree.Items)
                {
                    var treeitem = treeobject as TreeViewItem;
                    if (treeitem != null)
                    {
                        // IsSeleceted TreeViewItem of value with index
                        if (treeitem.Name == ("index_" + metadata_index.ToString()))
                        {
                            treeitem.Header = metadata_is_selected.ToString();
                            return;
                        }
                        change_metadata(treeitem, metadata_index, metadata_is_selected);
                    }
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private TreeViewItem _tree_root = null;

        }
    }
}
