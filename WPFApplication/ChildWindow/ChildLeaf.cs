using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Frontend.GUI;
using Frontend.ChildWindows;
using Core.Utilities;



/*
 * Child Leaf
 * 
 * 
 */
namespace Frontend
{
    namespace ChildWindows
    {
        public class ChildLeaf : AbstractChild
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public ChildLeaf(ChildBranch parent_branch, bool parent_is_root, AvailableContentCall available_content, RequestContentCall request_content)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                _available_content = available_content;
                _request_content = request_content;

                _grid = new Grid();
                _grid.Background = ColorTheme.GridBackground;
                _grid.Name = "grid_" + ChildBranch.GenerateID();

                /// DEBUG background color
                var generator = new Random();
                var color = generator.Next(1111, 9999).ToString();
                _grid.Background = (Brush)new BrushConverter().ConvertFrom("#" + color);

                setup_contextmenu();
            }

            ~ChildLeaf()
            {
                _grid = null;
                _parent_branch = null;
            }


            public Grid GetContentElement()
            {
                return _grid;
            }


            public void SetParent(ChildBranch parent_branch, bool parent_is_root)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                // Recreate context menu due to changed root
                setup_contextmenu();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void setup_contextmenu()
            {
                ContextMenu contextmenu = new ContextMenu();
                contextmenu.Style = ColorTheme.ContextMenuStyle();
                // Dynamically create context menu when loaded
                contextmenu.Loaded += contextmenu_loaded;
                _grid.ContextMenu = contextmenu;
            }


            private void contextmenu_loaded(object sender, RoutedEventArgs e)
            {

                var contextmenu = (ContextMenu)sender;
                contextmenu.Items.Clear();

                // Horizontal 
                var item_horizontal_top = new MenuItem();
                item_horizontal_top.Style = ColorTheme.MenuItemStyle();
                item_horizontal_top.Header = "Move Child Top";
                item_horizontal_top.Name = _horizontal_top;
                item_horizontal_top.Click += menuitem_clicked;

                var item_horizontal_bottom = new MenuItem();
                item_horizontal_bottom.Style = ColorTheme.MenuItemStyle();
                item_horizontal_bottom.Header = "Move Child Bottom";
                item_horizontal_bottom.Name = _horizontal_bottom;
                item_horizontal_bottom.Click += menuitem_clicked;

                var item_horizontal = new MenuItem();
                item_horizontal.Style = ColorTheme.MenuItemStyle();
                item_horizontal.Header = "Horizontal Split";
                item_horizontal.Items.Add(item_horizontal_top);
                item_horizontal.Items.Add(item_horizontal_bottom);
                contextmenu.Items.Add(item_horizontal);

                // Vertical
                var item_vertical_left = new MenuItem();
                item_vertical_left.Style = ColorTheme.MenuItemStyle();
                item_vertical_left.Header = "Move Child Left";
                item_vertical_left.Name = _vertical_Left;
                item_vertical_left.Click += menuitem_clicked;

                var item_vertical_right = new MenuItem();
                item_vertical_right.Style = ColorTheme.MenuItemStyle();
                item_vertical_right.Header = "Move Child Right";
                item_vertical_right.Name = _vertical_right;
                item_vertical_right.Click += menuitem_clicked;

                var item_vertical = new MenuItem();
                item_vertical.Style = ColorTheme.MenuItemStyle();
                item_vertical.Header = "Vertical Split";
                item_vertical.Items.Add(item_vertical_left);
                item_vertical.Items.Add(item_vertical_right);
                contextmenu.Items.Add(item_vertical);

                // Enable deletion of child only if it is not root
                if (!_parent_is_root)
                {
                    var item_delete = new MenuItem();
                    item_delete.Style = ColorTheme.MenuItemStyle();
                    item_delete.Header = "Delete Child Window";
                    item_delete.Name = _delete;
                    item_delete.Click += menuitem_clicked;
                    contextmenu.Items.Add(item_delete);
                }

                var item_content_menu = new MenuItem();
                item_content_menu.Style = ColorTheme.MenuItemStyle();
                item_content_menu.Header = "Content";
                var available_child_content = _available_content();
                foreach (var content_data in available_child_content)
                {
                    var content_item = new MenuItem();
                    content_item.Style = ColorTheme.MenuItemStyle();
                    content_item.Header = content_data.Item1;
                    string name_id = content_data.Item2;
                    content_item.Name = name_id;
                    content_item.Click += menuitem_clicked;
                    item_content_menu.Items.Add(content_item);
                }
                contextmenu.Items.Add(item_content_menu);
            }

            private void menuitem_clicked(object sender, RoutedEventArgs e)
            {
                var sender_menuitem = sender as MenuItem;
                string sender_name = sender_menuitem.Name;

                if (sender_name == _horizontal_top)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Horizontal, ChildBranch.ChildLocation.Top_Left);
                }
                else if (sender_name == _horizontal_bottom)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Horizontal, ChildBranch.ChildLocation.Bottom_Right);
                }
                else if (sender_name == _vertical_Left)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Vertical, ChildBranch.ChildLocation.Top_Left);
                }
                else if (sender_name == _vertical_right)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Vertical, ChildBranch.ChildLocation.Bottom_Right);
                }
                else if (sender_name == _delete)
                {
                    _parent_branch.DeleteLeaf();
                    _grid.Children.Clear();
                    if (_reset_content_available != null)
                    {
                        _reset_content_available(true);
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "BUG: Missing value for _reset_content_available");
                    }
                }

                var available_child_content = _available_content();
                foreach (var content in available_child_content)
                {
                    if (sender_name == content.Item2)
                    {
                        _request_content(content.Item2, _grid);
                        _reset_content_available = content.Item3;
                    }
                }
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _horizontal_top = "child_horizontal_top_" + GenerateID();
            private readonly string _horizontal_bottom = "Child_horizontal_bottom_" + GenerateID();
            private readonly string _vertical_Left = "child_vertical_left_" + GenerateID();
            private readonly string _vertical_right = "child_vertical_right" + GenerateID();
            private readonly string _delete = "child_delete" + GenerateID();

            private AbstractContent.SetContentAvailableCall _reset_content_available = null;
        }
    }
}
