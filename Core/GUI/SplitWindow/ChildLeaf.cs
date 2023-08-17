using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using Core.Abstracts;
using Core.Utilities;
/*
 * Child Leaf
 * 
 * 
 */

using ContentDataType = System.Tuple<string, string, Core.Abstracts.AbstractContent.DetachContentCallback>;

namespace Core
{
    namespace GUI
    {
        public class ChildLeaf : AbstractChild
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public ChildLeaf(ChildBranch parent_branch, bool parent_is_root, AvailableContentCallback available_content, RequestContentCallback request_content)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                _available_content = available_content;
                _request_content = request_content;

                _content = new Grid();
                _content.Background = ColorTheme.GridBackground;
                _content.Name = "grid_" + UniqueStringID.Generate();

                // Drag and drop
                _content.MouseMove += content_mousemove;
                _content.AllowDrop = true;
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
                item_horizontal_top.Style = ColorTheme.MenuItemStyle("/resources/menu/move-top.png");
                item_horizontal_top.Header = "Move Top";
                item_horizontal_top.Name = _item_id_hori_top;
                item_horizontal_top.Click += menuitem_click;

                var item_horizontal_bottom = new MenuItem();
                item_horizontal_bottom.Style = ColorTheme.MenuItemStyle("/resources/menu/move-bottom.png");
                item_horizontal_bottom.Header = "Move Bottom";
                item_horizontal_bottom.Name = _item_id_hori_bottom;
                item_horizontal_bottom.Click += menuitem_click;

                var item_horizontal = new MenuItem();
                item_horizontal.Style = ColorTheme.MenuItemStyle("/resources/menu/split-horizontal.png");
                item_horizontal.Header = "Horizontal Split";
                item_horizontal.Items.Add(item_horizontal_top);
                item_horizontal.Items.Add(item_horizontal_bottom);
                contextmenu.Items.Add(item_horizontal);

                // Vertical
                var item_vertical_left = new MenuItem();
                item_vertical_left.Style = ColorTheme.MenuItemStyle("/resources/menu/move-left.png");
                item_vertical_left.Header = "Move Left";
                item_vertical_left.Name = _item_id_vert_Left;
                item_vertical_left.Click += menuitem_click;

                var item_vertical_right = new MenuItem();
                item_vertical_right.Style = ColorTheme.MenuItemStyle("/resources/menu/move-right.png");
                item_vertical_right.Header = "Move Right";
                item_vertical_right.Name = _item_id_vert_right;
                item_vertical_right.Click += menuitem_click;

                var item_vertical = new MenuItem();
                item_vertical.Style = ColorTheme.MenuItemStyle("/resources/menu/split-vertical.png");
                item_vertical.Header = "Vertical Split";
                item_vertical.Items.Add(item_vertical_left);
                item_vertical.Items.Add(item_vertical_right);
                contextmenu.Items.Add(item_vertical);

                // Enable deletion of child only if it is not root
                if (!_parent_is_root)
                {
                    var item_delete = new MenuItem();
                    item_delete.Style = ColorTheme.MenuItemStyle("/resources/menu/delete-window.png");
                    item_delete.Header = "Delete Window";
                    item_delete.Name = _item_id_delete;
                    item_delete.Click += menuitem_click;
                    contextmenu.Items.Add(item_delete);
                }

                var item_content_menu = new MenuItem();
                item_content_menu.Style = ColorTheme.MenuItemStyle("/resources/menu/add-content.png");
                item_content_menu.Header = "Add Content";
                var available_child_content = _available_content();
                foreach (var content_data in available_child_content)
                {
                    var content_item = new MenuItem();
                    content_item.Style = ColorTheme.MenuItemStyle();
                    content_item.Header = content_data.Item2; // = Name
                    content_item.Name = content_data.Item1; // = ID;
                    content_item.Click += menuitem_click;
                    item_content_menu.Items.Add(content_item);
                }
                if (item_content_menu.Items.IsEmpty)
                {
                    item_content_menu.IsEnabled = false;
                }
                contextmenu.Items.Add(item_content_menu);

                var item_sep = new Separator();
                contextmenu.Items.Add(item_sep);

                var item_sep_temp = new Separator();
                var sep_template = new ControlTemplate();
                sep_template.TargetType = typeof(Separator);
                var text = new FrameworkElementFactory(typeof(TextBlock));
                text.SetValue(TextBlock.TextProperty, "Drag & Drop Content: Middle mouse button" + Environment.NewLine + "(Target content will be replaced)");
                text.SetValue(TextBlock.ForegroundProperty, ColorTheme.TextDisabled);
                text.SetValue(TextBlock.MarginProperty, new Thickness(35.0, 2.0, 2.0, 2.0)); // left,top,right,bottom
                sep_template.VisualTree = text;
                item_sep_temp.Template = sep_template;
                contextmenu.Items.Add(item_sep_temp);
            }


            private void menuitem_click(object sender, RoutedEventArgs e)
            {
                var sender_content = sender as MenuItem;
                if (sender_content == null)
                {
                    return;
                }

                string content_id = sender_content.Name;
                if (content_id == _item_id_hori_top)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Horizontal, ChildBranch.ChildLocation.Top_Left);
                }
                else if (content_id == _item_id_hori_bottom)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Horizontal, ChildBranch.ChildLocation.Bottom_Right);
                }
                else if (content_id == _item_id_vert_Left)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Vertical, ChildBranch.ChildLocation.Top_Left);
                }
                else if (content_id == _item_id_vert_right)
                {
                    _parent_branch.Split(ChildBranch.SplitOrientation.Vertical, ChildBranch.ChildLocation.Bottom_Right);
                }
                else if (content_id == _item_id_delete)
                {
                    content_detach();
                    _parent_branch.DeleteLeaf();
                }

                var available_contents = _available_content();
                foreach (var available_content in available_contents)
                {
                    if (content_id == available_content.Item1) // = ID
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


            private void content_mousemove(object sender, MouseEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                if ((_attached_content != null) && (e.MiddleButton == MouseButtonState.Pressed))
                {
                    var tmp_attached_content = _attached_content;
                    content_detach();
                    DragDrop.DoDragDrop(sender_grid, tmp_attached_content, DragDropEffects.All);
                }
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
                    e.Effects = DragDropEffects.Move | DragDropEffects.Copy;
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
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ContentDataType _attached_content = null;

            private readonly string _item_id_hori_top = "item_horizontal_top_" + UniqueStringID.Generate();
            private readonly string _item_id_hori_bottom = "item_horizontal_bottom_" + UniqueStringID.Generate();
            private readonly string _item_id_vert_Left = "item_vertical_left_" + UniqueStringID.Generate();
            private readonly string _item_id_vert_right = "item_vertical_right" + UniqueStringID.Generate();
            private readonly string _item_id_delete = "item_delete" + UniqueStringID.Generate();
        }
    }
}
