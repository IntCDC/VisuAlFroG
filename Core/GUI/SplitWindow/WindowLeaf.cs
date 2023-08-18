using System;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Abstracts;
using Core.Utilities;



// Parameters: <name, available, type>
using AvailableContentList_Type = System.Collections.Generic.List<System.Tuple<string, bool, System.Type>>;
// Parameters: <id, type>
using AttachedContent_Type = System.Tuple<string, System.Type>;

using ContentCallbacks = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.RequestContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


/*
 * Child Leaf
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class WindowLeaf : AbstractWindow
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public WindowLeaf(WindowBranch parent_branch, bool parent_is_root, ContentCallbacks content_callbacks)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                _content_callbacks = content_callbacks;
                if (_parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Paramter parent_branch should not be null");
                    return;
                }
                if (_content_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Paramter content_callbacks should not be null");
                    return;
                }

                _content = new Grid();
                _content.Background = ColorTheme.GridBackground;
                _content.Name = "grid_" + UniqueID.Generate();

                var info_text = new TextBlock();
                info_text.Text = "  [Right-click for Context Menu]";
                info_text.Foreground = ColorTheme.TextDisabled;
                _content.Children.Add(info_text);

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


            public void SetParent(WindowBranch parent_branch, bool parent_is_root)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                if (_parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Paramter parent_branch should not be null");
                    return;
                }

                // Recreate context menu due to changed root
                contextmenu_setup();
            }


            public Grid GetContentElement()
            {
                return _content;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void contextmenu_setup()
            {
                ContextMenu contextmenu = new ContextMenu();
                contextmenu.Style = ColorTheme.ContextMenuStyle();
                // Create context menu when loaded for updated availability of content
                contextmenu.Loaded += contextmenu_loaded;
                _content.ContextMenu = contextmenu;
            }


            private void contextmenu_loaded(object sender, RoutedEventArgs e)
            {
                var contextmenu = sender as ContextMenu;
                if (contextmenu == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Expected context menu");
                    return;
                }
                contextmenu.Items.Clear();

                // Horizontal 
                var item_horizontal_top = new MenuItem();
                item_horizontal_top.Style = ColorTheme.MenuItemStyle("align-top.png");
                item_horizontal_top.Header = "Align Top";
                item_horizontal_top.Name = _item_id_hori_top;
                item_horizontal_top.Click += menuitem_click;

                var item_horizontal_bottom = new MenuItem();
                item_horizontal_bottom.Style = ColorTheme.MenuItemStyle("align-bottom.png");
                item_horizontal_bottom.Header = "Align Bottom";
                item_horizontal_bottom.Name = _item_id_hori_bottom;
                item_horizontal_bottom.Click += menuitem_click;

                var item_horizontal = new MenuItem();
                item_horizontal.Style = ColorTheme.MenuItemStyle("split-horizontal.png");
                item_horizontal.Header = "Horizontal Split";
                item_horizontal.Items.Add(item_horizontal_top);
                item_horizontal.Items.Add(item_horizontal_bottom);
                contextmenu.Items.Add(item_horizontal);

                // Vertical
                var item_vertical_left = new MenuItem();
                item_vertical_left.Style = ColorTheme.MenuItemStyle("align-left.png");
                item_vertical_left.Header = "Align Left";
                item_vertical_left.Name = _item_id_vert_Left;
                item_vertical_left.Click += menuitem_click;

                var item_vertical_right = new MenuItem();
                item_vertical_right.Style = ColorTheme.MenuItemStyle("align-right.png");
                item_vertical_right.Header = "Align Right";
                item_vertical_right.Name = _item_id_vert_right;
                item_vertical_right.Click += menuitem_click;

                var item_vertical = new MenuItem();
                item_vertical.Style = ColorTheme.MenuItemStyle("split-vertical.png");
                item_vertical.Header = "Vertical Split";
                item_vertical.Items.Add(item_vertical_left);
                item_vertical.Items.Add(item_vertical_right);
                contextmenu.Items.Add(item_vertical);

                // Enable deletion of child only if it is not root
                if (!_parent_is_root)
                {
                    var item_delete = new MenuItem();
                    item_delete.Style = ColorTheme.MenuItemStyle("delete-window.png");
                    item_delete.Header = "Delete Window";
                    item_delete.Name = _item_id_window_delete;
                    item_delete.Click += menuitem_click;
                    contextmenu.Items.Add(item_delete);
                }

                var item_sep1 = new Separator();
                contextmenu.Items.Add(item_sep1);

                var item_content_add = new MenuItem();
                item_content_add.Style = ColorTheme.MenuItemStyle("add-content.png");
                item_content_add.Header = "Add Content";
                // Call AvailableContents_Delegate
                AvailableContentList_Type available_child_content = _content_callbacks.Item1();
                foreach (var content_data in available_child_content)
                {
                    // Item index: 1=name, 2=available, 3=type
                    var content_item = new MenuItem();
                    content_item.Style = ColorTheme.MenuItemStyle();

                    // Repalcement of spaces is necessary for Name property
                    string name = content_data.Item1.Replace(' ', '_'); // Hame
                    content_item.Header = name;
                    content_item.Name = name;
                    content_item.IsEnabled = content_data.Item2; // Available
                    content_item.Click += menuitem_click;
                    item_content_add.Items.Add(content_item);
                }
                if (item_content_add.Items.IsEmpty)
                {
                    item_content_add.IsEnabled = false;
                }
                contextmenu.Items.Add(item_content_add);

                var item_content_remove = new MenuItem();
                item_content_remove.Style = ColorTheme.MenuItemStyle("remove-content.png");
                item_content_remove.Header = "Remove Content";
                item_content_remove.Name = _item_id_remove_content;
                item_content_remove.Click += menuitem_click;
                if (_attached_content == null)
                {
                    item_content_remove.IsEnabled = false;
                }
                contextmenu.Items.Add(item_content_remove);

                var item_content_dad = new MenuItem();
                item_content_dad.Style = ColorTheme.MenuItemStyle("drag-and-drop.png");
                item_content_dad.Header = "[Middle Mouse Button] Drag&Drop Content";
                item_content_dad.ToolTip = "Target content will be replaced";
                item_content_dad.IsEnabled = true;
                contextmenu.Items.Add(item_content_dad);
            }


            private void menuitem_click(object sender, RoutedEventArgs e)
            {
                var sender_content = sender as MenuItem;
                if (sender_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Expected menu item");
                    return;
                }

                string content_id = sender_content.Name;
                if (content_id == _item_id_hori_top)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Horizontal, WindowBranch.ChildLocation.Top_Left);
                }
                else if (content_id == _item_id_hori_bottom)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Horizontal, WindowBranch.ChildLocation.Bottom_Right);
                }
                else if (content_id == _item_id_vert_Left)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Vertical, WindowBranch.ChildLocation.Top_Left);
                }
                else if (content_id == _item_id_vert_right)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Vertical, WindowBranch.ChildLocation.Bottom_Right);
                }
                else if (content_id == _item_id_window_delete)
                {
                    content_detach();
                    _parent_branch.DeleteLeaf();
                }
                else if (content_id == _item_id_remove_content)
                {
                    content_detach();
                }

                // Call AvailableContents_Delegate
                AvailableContentList_Type available_contents = _content_callbacks.Item1();
                foreach (var content_data in available_contents)
                {
                    // Repalcement of spaces is necessary for Name property
                    string name = content_data.Item1.Replace(' ', '_'); // name
                    if (content_id == name)
                    {
                        content_attach(UniqueID.Invalid, content_data.Item3);
                    }
                }
            }


            private void content_attach(string content_id, Type content_type)
            {
                content_detach();

                // Call RequestContent_Delegate
                string updated_content_id = _content_callbacks.Item2(content_id, content_type, _content);
                if (updated_content_id != UniqueID.Invalid)
                {
                    _attached_content = new AttachedContent_Type(updated_content_id, content_type);
                }
            }


            private void content_detach()
            {
                _content.Children.Clear();
                _content.Background = ColorTheme.GridBackground;
               if (_attached_content != null) {
                    if (_attached_content.Item1 != UniqueID.Invalid)
                    {
                        // Call DeleteContent_Delegate
                        _content_callbacks.Item3(_attached_content.Item1);
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
                var data_type = typeof(AttachedContent_Type);
                if (e.Data.GetDataPresent(data_type))
                {
                    // Change mouse cursor
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
                // Check for compatible drop data
                var data_type = typeof(AttachedContent_Type);
                if (e.Data.GetDataPresent(data_type))
                {
                    var content_data = (AttachedContent_Type)e.Data.GetData(data_type);
                    // Currently attached content is replaced
                    content_attach(content_data.Item1, content_data.Item2);
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private AttachedContent_Type _attached_content = null;

            private readonly string _item_id_hori_top = "item_horizontal_top_" + UniqueID.Generate();
            private readonly string _item_id_hori_bottom = "item_horizontal_bottom_" + UniqueID.Generate();
            private readonly string _item_id_vert_Left = "item_vertical_left_" + UniqueID.Generate();
            private readonly string _item_id_vert_right = "item_vertical_right" + UniqueID.Generate();
            private readonly string _item_id_window_delete = "item_window_delete" + UniqueID.Generate();
            private readonly string _item_id_remove_content = "item_remove_content" + UniqueID.Generate();
        }
    }
}
