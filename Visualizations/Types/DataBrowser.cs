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


/*
 *  Content for Data Filtering
 * 
 */
namespace Visualizations
{
    namespace Types
    {
        public class DataBrowser : AbstractGenericVisualization<System.Windows.Controls.TreeView, DefaultData_Type>
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

                int branch_index = 0;
                foreach (var leaf_list in data)
                {
                    var tree_branch = new TreeViewItem();
                    tree_branch.Header = "Branch" + branch_index.ToString();

                    int leaf_index = 0;
                    foreach (var value in leaf_list)
                    {
                        var v = value;
                        var tree_leaf = new TreeViewItem();
                        tree_leaf.Header = v.ToString(); //"Leaf" + branch_index.ToString();

                        tree_branch.Items.Add(tree_leaf);
                        leaf_index++;
                    }
                    tree_root.Items.Add(tree_branch);
                    branch_index++;
                }

                Content.Items.Add(tree_root);
                Content.Background = ColorTheme.GenericBackground;
                Content.Foreground = ColorTheme.GenericForeground;

                _timer.Stop();
                _created = true;
                return _created;
            }
        }
    }
}
