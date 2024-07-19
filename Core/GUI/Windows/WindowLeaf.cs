using System;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Utilities;


// Additional types only used here:
using DragDrop_Type = System.Tuple<Core.GUI.WindowLeaf, int, string, string>;
using AttachedContent_Type = System.Tuple<int, string>;


/*
 * Window Leaf
 * 
 */

// Arguments: <content name, flag: is content available, flag: are multiple instances allowed, content type>
using ReadContentMetaData_Type = System.Tuple<string, bool, bool, string>;
using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;
using System.Windows.Data;
using System.Linq;


namespace Core
{
    namespace GUI
    {
        public class WindowLeaf : AbstractWindow
        {
            /* ------------------------------------------------------------------*/
            #region public classes 

            /// <summary>
            /// Configuration data.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public string Name { get; set; }
                public int ContentUID { get; set; }
                public string ContentType { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties 

            public string _Name { get { return _content_caption.Text; } set { _content_caption.Text = value; } }
            public AttachedContent_Type _AttachedContent { get; private set; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="parent_branch">The parent window branch.</param>
            /// <param name="parent_is_root">Flag indicating whether branch is root.</param>
            /// <param name="content_callbacks">The content callbacks required in the window leaf.</param>
            public WindowLeaf(WindowBranch parent_branch, bool parent_is_root, ContentCallbacks_Type content_callbacks)
            {
                if (parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parameter parent_branch should not be null");
                    return;
                }
                if (content_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parameter content_callbacks should not be null");
                    return;
                }
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                _content_callbacks = content_callbacks;

                _menu = new MenubarWindow();
                _content_caption = new RenameLabel();
                _content_child = new Grid();
                _attached_type_text = new TextBlock();

                _AttachedContent = null;
                _Name = _default_caption;

                _content_child.SetResourceReference(Grid.BackgroundProperty, "Brush_Background");
                _content_child.Name = "grid_" + UniqueID.GenerateString();
                // Drag and drop
                _content_child.MouseMove += event_content_mousemove;
                _content_child.AllowDrop = true;
                _content_child.DragOver += event_content_dragover;
                _content_child.Drop += event_content_drop;

                // Default usage hint
                /*
                var info_text = new TextBlock();
                info_text.Text = "HINT: Add new content via menu 'Content' -> 'Add Content'";
                info_text.SetResourceReference(TextBlock.ForegroundProperty, "Brush_TextDisabled");
                _content_child.Children.Add(info_text);
                */

                var content_panel = new DockPanel();
                StackPanel stack = new StackPanel();
                stack.Children.Add(create_menu());
                DockPanel.SetDock(stack, System.Windows.Controls.Dock.Top);
                content_panel.Children.Add(stack);
                content_panel.Children.Add(_content_child);

                _Content = new Grid();
                _Content.Children.Add(content_panel);
            }

            /// <summary>
            /// Request creation of new content.
            /// </summary>
            /// <param name="uid">Provide ID of existing content or invalid id otherwise.</param>
            /// <param name="content_type">The type of the content as string.</param>
            public void CreateContent(int uid, string content_type)
            {
                // Call Create Content
                var content_metadata = _content_callbacks.Item2(uid, content_type, create_caption_binding());
                if (content_metadata != null)
                {
                    if ((content_metadata.Item1 != UniqueID.InvalidInt) && (content_metadata.Item3 != null))
                    {
                        _content_child.Children.Add(content_metadata.Item3);
                        _AttachedContent = new AttachedContent_Type(content_metadata.Item1, content_type);

                        // Enable delete content option
                        var delete_content_menu_item = _menu.FindMenuItemByName(_item_id_delete_content);
                        if (delete_content_menu_item != null)
                        {
                            delete_content_menu_item.IsEnabled = true;
                        }

                        // Set content caption
                        _Name = content_metadata.Item2;

                        var split_typenames = content_type.Split(new[] { '.' }, 2);
                        _attached_type_text.Text = "  (" + split_typenames[(split_typenames.Count() - 1)] + ")";
                        _attached_type_text.SetResourceReference(TextBlock.ForegroundProperty, "Brush_TextDisabled");

                        _menu.Clear(MenubarWindow.PredefinedMenuOption.VIEW);

                        _menu.Clear(MenubarWindow.PredefinedMenuOption.DATA);
                        /// XXX Exclude some content from having the following menu --- Find better solution... Check if DataUID=Invalid but not available here
                        if (!((content_type == AbstractVisualization.TypeString_FilterEditor) || (content_type == AbstractVisualization.TypeString_LogConsole)))
                        {
                            var filter_menu_item = MenubarMain.GetDefaultMenuItem("Open Filter Editor", open_filter_editor);
                            _menu.AddMenu(MenubarWindow.PredefinedMenuOption.DATA, filter_menu_item);

                            List<ReadContentMetaData_Type> available_child_content = _content_callbacks.Item1();
                            foreach (var content_data in available_child_content)
                            {
                                if (content_data.Item4 == AbstractVisualization.TypeString_FilterEditor)
                                {
                                    filter_menu_item.Name = conform_name(content_data.Item1);
                                    break;
                                }
                            }
                            if (filter_menu_item.Name == "")
                            {
                                Log.Default.Msg(Log.Level.Error, "Could not find content.");
                            }
                            var parent = filter_menu_item.Parent as MenuItem;
                            if (parent != null)
                            {
                                parent.SubmenuOpened += event_togglecontent_click;
                            }
                            else
                            {
                                Log.Default.Msg(Log.Level.Error, "Missing parent menu item.");
                            }

                        }
                        content_metadata.Item4(_menu);
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing valid id or content element");
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Missing content element to attach");
                }
            }

            /// <summary>
            /// Set new parent branch.
            /// </summary>
            /// <param name="parent_branch">The new parent branch.</param>
            /// <param name="parent_is_root">Flag indicating if parent is root (required to prevent window deletion on top most level).</param>
            public void SetParent(WindowBranch parent_branch, bool parent_is_root)
            {
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                if (_parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parent branch parameter should not be null");
                    return;
                }
            }

            /// <summary>
            /// Reset attached resources.
            /// </summary>
            public void ResetLeaf()
            {
                delete_content();

                _content_child = null;
                _menu = null;
                _content_caption = null;
                _attached_type_text = null;

                base.reset();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            /// <summary>
            /// Delete attached content permanently.
            /// </summary>
            private void delete_content(bool only_detach = false)
            {
                _content_child.Children.Clear();
                _Name = _default_caption;
                _attached_type_text.Text = "";

                if (_AttachedContent != null)
                {
                    if (!only_detach)
                    {
                        if (_AttachedContent.Item1 != UniqueID.InvalidInt)
                        {
                            // Call Delete Content
                            _content_callbacks.Item3(_AttachedContent.Item1);
                        }
                    }
                    _AttachedContent = null;
                }
            }

            /// <summary>
            /// Create content menu.
            /// </summary>
            /// <returns>Return the content element holding the menu.</returns>
            private Grid create_menu()
            {
                _menu.Initialize();

                // ----- VIEW -----------------------------------------
                var menu_grid = new Grid();
                menu_grid.Height = 20.0;
                menu_grid.SetResourceReference(Grid.BackgroundProperty, "Brush_MenuBarBackground");

                var column_label = new ColumnDefinition();
                column_label.Width = new GridLength(0.0, GridUnitType.Auto);
                menu_grid.ColumnDefinitions.Add(column_label);
                var column_menu = new ColumnDefinition();
                column_menu.Width = new GridLength(1.0, GridUnitType.Auto);
                menu_grid.ColumnDefinitions.Add(column_menu);
                var info_menu = new ColumnDefinition();
                info_menu.Width = new GridLength(1.0, GridUnitType.Auto);
                menu_grid.ColumnDefinitions.Add(info_menu);

                Grid.SetColumn(_content_caption, 0);
                menu_grid.Children.Add(_content_caption);

                var menu = _menu.AttachUI();
                Grid.SetColumn(menu, 1);
                menu.Style = ColorTheme.ContentMenuBarStyle();
                menu_grid.Children.Add(menu);

                _attached_type_text.Text = "";
                Grid.SetColumn(_attached_type_text, 2);
                menu_grid.Children.Add(_attached_type_text);


                var _menu_rename = MenubarMain.GetDefaultMenuItem("Rename");
                _menu_rename.Click += (object sender, RoutedEventArgs e) =>
                {
                    _content_caption.Focusable = true;
                    _content_caption.Style = null;
                    _content_caption.Cursor = null;
                    _content_caption.Focus();
                };
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, _menu_rename);

                _menu.AddSeparator(MenubarWindow.PredefinedMenuOption.CONTENT);

                // Horizontal 
                var item_horizontal_top = new MenuItem();
                item_horizontal_top.Style = ColorTheme.MenuItemIconStyle("align-top.png");
                item_horizontal_top.Header = "Align Top";
                item_horizontal_top.Name = _item_id_hori_top;
                item_horizontal_top.Click += event_menuitem_click;

                var item_horizontal_bottom = new MenuItem();
                item_horizontal_bottom.Style = ColorTheme.MenuItemIconStyle("align-bottom.png");
                item_horizontal_bottom.Header = "Align Bottom";
                item_horizontal_bottom.Name = _item_id_hori_bottom;
                item_horizontal_bottom.Click += event_menuitem_click;

                var item_horizontal = new MenuItem();
                item_horizontal.Style = ColorTheme.MenuItemIconStyle("split-horizontal.png");
                item_horizontal.Header = "Horizontal Split";
                item_horizontal.Items.Add(item_horizontal_top);
                item_horizontal.Items.Add(item_horizontal_bottom);
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, item_horizontal);

                // Vertical
                var item_vertical_left = new MenuItem();
                item_vertical_left.Style = ColorTheme.MenuItemIconStyle("align-left.png");
                item_vertical_left.Header = "Align Left";
                item_vertical_left.Name = _item_id_vert_Left;
                item_vertical_left.Click += event_menuitem_click;

                var item_vertical_right = new MenuItem();
                item_vertical_right.Style = ColorTheme.MenuItemIconStyle("align-right.png");
                item_vertical_right.Header = "Align Right";
                item_vertical_right.Name = _item_id_vert_right;
                item_vertical_right.Click += event_menuitem_click;

                var item_vertical = new MenuItem();
                item_vertical.Style = ColorTheme.MenuItemIconStyle("split-vertical.png");
                item_vertical.Header = "Vertical Split";
                item_vertical.Items.Add(item_vertical_left);
                item_vertical.Items.Add(item_vertical_right);
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, item_vertical);

                // Enable deletion of child only if it is not root
                var item_delete = new MenuItem();
                item_delete.Style = ColorTheme.MenuItemIconStyle("delete-window.png");
                item_delete.Header = "Delete Window";
                item_delete.Name = _item_id_window_delete;
                item_delete.Click += event_menuitem_click;
                _menu.AddSubMenuOpenEvent(MenubarWindow.PredefinedMenuOption.CONTENT, event_menuitem_deletewindow_state);
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, item_delete);

                _menu.AddSeparator(MenubarWindow.PredefinedMenuOption.CONTENT);

                var item_content_add = new MenuItem();
                item_content_add.Style = ColorTheme.MenuItemIconStyle("add-content.png");
                item_content_add.Header = "Add Content";
                item_content_add.SubmenuOpened += event_togglecontent_click;
                // Call Available Contents
                List<ReadContentMetaData_Type> available_child_content = _content_callbacks.Item1();
                foreach (var content_data in available_child_content)
                {
                    // Item index: 1=name, 2=available, 3=is-multi, 4=type
                    var content_item = new MenuItem();
                    content_item.Style = ColorTheme.MenuItemIconStyle();

                    content_item.Header = content_data.Item1;
                    content_item.Name = conform_name(content_data.Item1); // Name
                    content_item.IsEnabled = content_data.Item2; // Available
                    if (content_data.Item3) // Multiple instances allowed
                    {
                        content_item.Style = ColorTheme.MenuItemIconStyle("multi-instance.png");
                        content_item.ToolTip = "Multiple Instances";
                    }
                    else
                    {
                        content_item.Style = ColorTheme.MenuItemIconStyle("single-instance.png");
                        content_item.ToolTip = "Single Instance";
                    }
                    content_item.Click += event_menuitem_click;
                    item_content_add.Items.Add(content_item);
                }
                if (item_content_add.Items.IsEmpty)
                {
                    item_content_add.IsEnabled = false;
                }
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, item_content_add);

                var item_content_delete = new MenuItem();
                item_content_delete.Style = ColorTheme.MenuItemIconStyle("delete-content.png");
                item_content_delete.Header = "Delete Content";
                item_content_delete.Name = _item_id_delete_content;
                item_content_delete.Click += event_menuitem_click;
                item_content_delete.IsEnabled = false;
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, item_content_delete);

                var item_content_dad = new MenuItem();
                item_content_dad.Style = ColorTheme.MenuItemIconStyle("drag-and-drop.png");
                item_content_dad.Header = "Content Swap";
                _menu.AddMenu(MenubarWindow.PredefinedMenuOption.CONTENT, item_content_dad);

                var item_content_dad_text = new MenuItem();
                item_content_dad_text.Header = "Drag&Drop [Middle Mouse Button]";
                item_content_dad_text.IsEnabled = false;
                item_content_dad_text.IsHitTestVisible = false;
                item_content_dad_text.Focusable = false;
                item_content_dad.Items.Add(item_content_dad_text);

                // ----- DATA -----------------------------------------
                // attached on creation of content, see CreateContent()

                return menu_grid;
            }

            /// <summary>
            /// Called when a menu item is clicked.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The routed event arguments.</param>
            private void event_menuitem_click(object sender, RoutedEventArgs e)
            {
                var sender_content = sender as MenuItem;
                if (sender_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }

                string uid = sender_content.Name;
                if (uid == _item_id_hori_top)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Horizontal, WindowBranch.ChildLocation.Top_Left);
                }
                else if (uid == _item_id_hori_bottom)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Horizontal, WindowBranch.ChildLocation.Bottom_Right);
                }
                else if (uid == _item_id_vert_Left)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Vertical, WindowBranch.ChildLocation.Top_Left);
                }
                else if (uid == _item_id_vert_right)
                {
                    _parent_branch.Split(WindowBranch.SplitOrientation.Vertical, WindowBranch.ChildLocation.Bottom_Right);
                }
                else if (uid == _item_id_window_delete)
                {
                    delete_content();
                    _parent_branch.DeleteLeaf();
                }
                else if (uid == _item_id_delete_content)
                {
                    delete_content();
                    // Disable delete content option
                    sender_content.IsEnabled = false;
                }

                // Call Available Contents
                List<ReadContentMetaData_Type> available_contents = _content_callbacks.Item1();
                foreach (var content_data in available_contents)
                {
                    string name = conform_name(content_data.Item1);
                    if (uid == name)
                    {
                        delete_content();
                        CreateContent(UniqueID.InvalidInt, content_data.Item4);
                    }
                }
            }

            /// <summary>
            /// Called to dynamically determine whether the delete window option should be active or not.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void event_menuitem_deletewindow_state(object sender, RoutedEventArgs e)
            {
                var target_menuitem = _menu.FindMenuItemByName(_item_id_window_delete);
                if (target_menuitem != null)
                {
                    target_menuitem.IsEnabled = (!_parent_is_root);
                }
            }

            private void event_togglecontent_click(object sender, RoutedEventArgs e)
            {
                var parent_menu = sender as MenuItem;
                if (parent_menu == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected sender");
                    return;
                }

                List<ReadContentMetaData_Type> available_child_content = _content_callbacks.Item1();
                foreach (var content_data in available_child_content)
                {
                    foreach (var item in parent_menu.Items)
                    {
                        var menu_item = item as MenuItem;
                        if (menu_item == null)
                        {
                            continue;
                        }
                        if (menu_item.Name == conform_name(content_data.Item1))
                        {
                            menu_item.IsEnabled = content_data.Item2;
                        }
                    }
                }
            }

            private bool open_filter_editor()
            {
                var previous_parent_branch = _parent_branch;
                _parent_branch.Split(WindowBranch.SplitOrientation.Vertical, WindowBranch.ChildLocation.Top_Left, 0.7);
                previous_parent_branch._Children.Item2._Leaf.CreateContent(UniqueID.InvalidInt, AbstractVisualization.TypeString_FilterEditor);
                return true;
            }

            /// <summary>
            /// Callback called when middle mouse button is pressed and mouse is moved.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The mouse event arguments.</param>
            private void event_content_mousemove(object sender, MouseEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                if ((_AttachedContent != null) && (e.MiddleButton == MouseButtonState.Pressed))
                {
                    var drag_and_drop_load = new DragDrop_Type(this, _AttachedContent.Item1, _AttachedContent.Item2, _Name);
                    // Only detach content
                    delete_content(true);
                    DragDrop.DoDragDrop(sender_grid, drag_and_drop_load, DragDropEffects.All);
                }
            }

            /// <summary>
            /// Callback called when content is started to be dragged.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The drag event arguments.</param>
            private void event_content_dragover(object sender, DragEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                // Check for compatible data
                var data_type = typeof(DragDrop_Type);
                if (e.Data.GetDataPresent(data_type))
                {
                    // Change mouse cursor
                    e.Effects = DragDropEffects.Move | DragDropEffects.Copy;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected drag over data type");
                }
            }

            /// <summary>
            /// Callback called when dragged content is dropped.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The drag event arguments.</param>
            private void event_content_drop(object sender, DragEventArgs e)
            {
                var sender_grid = sender as Grid;
                if (sender_grid == null)
                {
                    return;
                }
                // Check for compatible drop data
                var data_type = typeof(DragDrop_Type);
                if (e.Data.GetDataPresent(data_type))
                {
                    var source_content = (DragDrop_Type)e.Data.GetData(data_type);
                    var target_content = _AttachedContent;
                    // Only detach content
                    delete_content(true);

                    // Move content from target to source (content in source is already detached)
                    if ((source_content.Item1 != null) && (target_content != null))
                    {
                        source_content.Item1.CreateContent(target_content.Item1, target_content.Item2);
                        source_content.Item1._Name = _Name;
                    }
                    // Drop content from source in target
                    CreateContent(source_content.Item2, source_content.Item3);
                    _Name = source_content.Item4;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unexpected drop data type");
                }
            }

            /// <summary>
            /// Convert string to WPF conform 'Name'
            /// </summary>
            /// <param name="name">The input name.</param>
            /// <returns>The conform output name.</returns>
            private string conform_name(string name)
            {
                var conform_name = name.Replace(" ", "");
                conform_name = conform_name.Replace("_", "");
                conform_name = conform_name.Replace(".", "");
                conform_name = conform_name.Replace("(", "");
                conform_name = conform_name.Replace(")", "");
                conform_name = Regex.Replace(conform_name, "[0-9]", "");
                return conform_name;
            }

            private Binding create_caption_binding()
            {
                var caption_binding = new Binding("Text");
                caption_binding.Mode = BindingMode.TwoWay;
                caption_binding.Source = _content_caption;
                return caption_binding;
                /// Client: _filter_caption.SetBinding(TextBlock.TextProperty, caption_binding);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _content_child = null;
            private MenubarWindow _menu = null;
            private RenameLabel _content_caption = null;
            private TextBlock _attached_type_text = null;

            private const string _default_caption = "...";

            private readonly string _item_id_hori_top = "item_horizontal_top_" + UniqueID.GenerateString();
            private readonly string _item_id_hori_bottom = "item_horizontal_bottom_" + UniqueID.GenerateString();
            private readonly string _item_id_vert_Left = "item_vertical_left_" + UniqueID.GenerateString();
            private readonly string _item_id_vert_right = "item_vertical_right" + UniqueID.GenerateString();
            private readonly string _item_id_window_delete = "item_window_delete" + UniqueID.GenerateString();
            private readonly string _item_id_delete_content = "item_delete_content" + UniqueID.GenerateString();

            #endregion
        }
    }
}
