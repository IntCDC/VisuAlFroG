using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Core.GUI;
using Core.Abstracts;
using Core.Utilities;
using System.Windows.Input;
using System.Windows.Shapes;



/*
 * Child Leaf
 * 
 * 
 */

using ContentDataType = System.Tuple<string, string, Core.Abstracts.AbstractContent.DetachContentCall>;

namespace Core
{
    namespace GUI
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

                _content = new Grid();
                _content.Background = ColorTheme.GridBackground;
                _content.Name = "grid_" + UniqueStringID.Generate();

                // Drag and drop
                _content.KeyDown += content_keydown;
                _content.KeyUp += content_keyup;
                _content.MouseMove += content_mousemove;
                _content.GiveFeedback += content_givefeedback;
                _content.AllowDrop = true;
                _content.DragEnter += content_dragenter;
                _content.DragLeave += content_dragleave;
                _content.DragOver += content_dragover;
                _content.Drop += content_drop;

                /// DEBUG background color
                 /*
                var generator = new Random();
                var color = generator.Next(1111, 9999).ToString();
                _content.Background = (Brush)new BrushConverter().ConvertFrom("#" + color);
                */

                contextmenu_setup();
            }

            ~ChildLeaf()
            {
                _content = null;
                _parent_branch = null;
            }


            public Grid GetContentElement()
            {
                return _content;
            }


            public void SetParent(ChildBranch parent_branch, bool parent_is_root)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                // Recreate context menu due to changed root
                contextmenu_setup();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void contextmenu_setup()
            {
                ContextMenu contextmenu = new ContextMenu();
                contextmenu.Style = ColorTheme.ContextMenuStyle();
                // Dynamically create context menu when loaded
                contextmenu.Loaded += contextmenu_loaded;
                _content.ContextMenu = contextmenu;
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
                    content_item.Header = content_data.Item2; // = Name
                    content_item.Name = content_data.Item1; // = ID;
                    content_item.Click += menuitem_clicked;
                    item_content_menu.Items.Add(content_item);
                }
                if (item_content_menu.Items.IsEmpty)
                {
                    item_content_menu.IsEnabled = false;
                }
                contextmenu.Items.Add(item_content_menu);

                var item_sep = new Separator();
                contextmenu.Items.Add(item_sep);

                var item_dad = new TextBlock();
                item_dad.Text = "Press 'Ctrl' key to drag & drop " + Environment.NewLine +
                                "content to another window." + Environment.NewLine +
                                "NOTE: Target content will be replaced.";
                item_dad.IsEnabled = false;
                contextmenu.Items.Add(item_dad);
            }


            private void menuitem_clicked(object sender, RoutedEventArgs e)
            {
                var sender_menuitem = sender as MenuItem;
                if (sender_menuitem == null)
                {
                    return;
                }

                string menuitem_id = sender_menuitem.Name;
                if (menuitem_id == _horizontal_top)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Horizontal, ChildBranch.ChildLocation.Top_Left);
                }
                else if (menuitem_id == _horizontal_bottom)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Horizontal, ChildBranch.ChildLocation.Bottom_Right);
                }
                else if (menuitem_id == _vertical_Left)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Vertical, ChildBranch.ChildLocation.Top_Left);
                }
                else if (menuitem_id == _vertical_right)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Vertical, ChildBranch.ChildLocation.Bottom_Right);
                }
                else if (menuitem_id == _delete)
                {
                    content_detach();
                    _parent_branch.DeleteLeaf();
                }

                var available_contents = _available_content();
                foreach (var available_content in available_contents)
                {
                    if (menuitem_id == available_content.Item1) // = ID
                    {
                        content_detach();
                        content_attach(available_content);
                    }
                }
            }


            private void content_attach(ContentDataType content_data)
            {
                if (content_data.Item3 == null) // DetachContentCall
                {
                    Log.Default.Msg(Log.Level.Error, "DetachContentCall delegate is null");
                    return;
                }
                // Request content to be added
                if (_request_content(content_data.Item1, _content))
                {
                    // Save local copy of attached content data
                    _attached_content = content_data;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested content could not be provided");
                }
            }


            private void content_detach()
            {
                _content.Children.Clear();
                _content.Background = ColorTheme.GridBackground;

                if (_attached_content != null)
                {
                    if (_attached_content.Item3 != null)
                    {
                        _attached_content.Item3(); // DetachContentCall
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "DetachContentCall delegate is null");
                    }
                    _attached_content = null;
                }
            }


            private void content_keydown(object sender, KeyEventArgs e)
            {
                if ((e.Key == Key.LeftCtrl) || (e.Key == Key.RightCtrl))
                {
                    _key_ctrl_down = true;
                }
            }


            private void content_keyup(object sender, KeyEventArgs e)
            {
                _key_ctrl_down = false;
            }


            private void content_mousemove(object sender, MouseEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                // Enable drag source if content is attached and Ctrl is pressed while mouse move
                if ((_attached_content != null) && (e.LeftButton == MouseButtonState.Pressed) && _key_ctrl_down)
                {
                    var tmp_attached_content = _attached_content;
                    content_detach();
                    _key_ctrl_down = false;
                    DragDrop.DoDragDrop(sender_grid, tmp_attached_content, DragDropEffects.All);
                }
            }


            private void content_givefeedback(object sender, GiveFeedbackEventArgs e)
            {
                // unused
            }


            private void content_dragenter(object sender, DragEventArgs e)
            {
                // unused
            }


            private void content_dragleave(object sender, DragEventArgs e)
            {
                // unused
            }


            private void content_dragover(object sender, DragEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                // Check for compatible data
                var data_type = typeof(ContentDataType);
                if (e.Data.GetDataPresent(data_type))
                {
                    e.Effects = DragDropEffects.All;
                }
            }


            private void content_drop(object sender, DragEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                // Drop data
                var data_type = typeof(ContentDataType);
                if (e.Data.GetDataPresent(data_type))
                {
                    var content_data = (ContentDataType)e.Data.GetData(data_type);
                    // Currently attached content is replaced
                    content_detach();
                    content_attach(content_data);
                    _key_ctrl_down = false;
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _horizontal_top = "child_horizontal_top_" + UniqueStringID.Generate();
            private readonly string _horizontal_bottom = "Child_horizontal_bottom_" + UniqueStringID.Generate();
            private readonly string _vertical_Left = "child_vertical_left_" + UniqueStringID.Generate();
            private readonly string _vertical_right = "child_vertical_right" + UniqueStringID.Generate();
            private readonly string _delete = "child_delete" + UniqueStringID.Generate();

            private ContentDataType _attached_content = null;

            // Used to trigger drag & drop
            private bool _key_ctrl_down = false;
            private bool _mouse_over = false;
        }
    }
}
