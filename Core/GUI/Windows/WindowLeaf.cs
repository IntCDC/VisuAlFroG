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
using DragDrop_Type = System.Tuple<Core.GUI.WindowLeaf, string, string>;
using AttachedContent_Type = System.Tuple<string, string>;


/*
 * Window Leaf
 * 
 */

// Arguments: <content name, flag: is content available, flag: are multiple instances allowed, content type>
using ReadContentMetaData_Type = System.Tuple<string, bool, bool, string>;

using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


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
                public string ContentID { get; set; }
                public string ContentType { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties 

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
                _parent_branch = parent_branch;
                _parent_is_root = parent_is_root;
                _content_callbacks = content_callbacks;
                if (_parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parameter parent_branch should not be null");
                    return;
                }
                if (_content_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parameter content_callbacks should not be null");
                    return;
                }

                _Content = new Grid();
                _Content.SetResourceReference(Grid.BackgroundProperty, "Brush_Background");
                _Content.Name = "grid_" + UniqueID.GenerateString();

                delete_content();

                // Drag and drop
                _Content.MouseMove += event_content_mousemove;
                _Content.AllowDrop = true;
                _Content.DragOver += event_content_dragover;
                _Content.Drop += event_content_drop;

                /// DEBUG background color
                 /*
                var generator = new Random();
                var color = generator.Next(1111, 9999).ToString();
                _Content.Background = (Brush)new BrushConverter().ConvertFrom("#" + color);
                */
                contextmenu_setup();
            }

            /// <summary>
            /// Request creation of new content.
            /// </summary>
            /// <param name="content_id">Provide ID of existing content or invalid id otherwise.</param>
            /// <param name="content_type">The type of the content as string.</param>
            public void CreateContent(string content_id, string content_type)
            {
                // Call Create Content
                var content_metadata = _content_callbacks.Item2(content_id, content_type);
                if (content_metadata != null)
                {
                    if ((content_metadata.Item1 != UniqueID.InvalidString) && (content_metadata.Item2 != null))
                    {
                        _Content.Children.Add(content_metadata.Item2);
                        _AttachedContent = new AttachedContent_Type(content_metadata.Item1, content_type);
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
                base.reset();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            /// <summary>
            /// Used to setup the context menu once.
            /// </summary>
            private void contextmenu_setup()
            {
                ContextMenu contextmenu = new ContextMenu();
                contextmenu.Style = ColorTheme.ContextMenuStyle();
                // Create context menu when loaded for updated availability of content
                contextmenu.Loaded += event_contextmenu_loaded;
                _Content.ContextMenu = contextmenu;
            }

            /// <summary>
            /// Callback called whenever context menu is opened.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The routed event arguments.</param>
            private void event_contextmenu_loaded(object sender, RoutedEventArgs e)
            {
                var contextmenu = sender as ContextMenu;
                if (contextmenu == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Expected context menu");
                    return;
                }
                contextmenu.Items.Clear();

                // Caption 
                var item_caption = new MenuItem();
                item_caption.Header = "───── Visualization Configuration ─────";
                item_caption.IsHitTestVisible = false;
                item_caption.Focusable = false;
                contextmenu.Items.Add(item_caption);
                contextmenu.Items.Add(new Separator());

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
                contextmenu.Items.Add(item_horizontal);

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
                contextmenu.Items.Add(item_vertical);

                // Enable deletion of child only if it is not root
                var item_delete = new MenuItem();
                item_delete.Style = ColorTheme.MenuItemIconStyle("delete-window.png");
                item_delete.Header = "Delete Window";
                item_delete.Name = _item_id_window_delete;
                item_delete.Click += event_menuitem_click;
                item_delete.IsEnabled = (!_parent_is_root);
                contextmenu.Items.Add(item_delete);

                contextmenu.Items.Add(new Separator());

                var item_content_add = new MenuItem();
                item_content_add.Style = ColorTheme.MenuItemIconStyle("add-content.png");
                item_content_add.Header = "Add Content";
                // Call Available Contents
                List<ReadContentMetaData_Type> available_child_content = _content_callbacks.Item1();
                foreach (var content_data in available_child_content)
                {
                    // Item index: 1=name, 2=available, 3=is-multi, 4=type
                    var content_item = new MenuItem();
                    content_item.Style = ColorTheme.MenuItemIconStyle();

                    // Replacement of spaces is necessary for Name property
                    content_item.Header = content_data.Item1;
                    content_item.Name = conform_name(content_data.Item1); // Name
                    content_item.IsEnabled = content_data.Item2; // Available
                    if (content_data.Item3) // Multiple instances allowed
                    {
                        content_item.Style = ColorTheme.MenuItemIconStyle("multi-instance.png");
                        content_item.ToolTip = "Multiple instances";
                    }
                    else
                    {
                        content_item.Style = ColorTheme.MenuItemIconStyle("single-instance.png");
                        content_item.ToolTip = "Only single instance";
                    }
                    content_item.Click += event_menuitem_click;
                    item_content_add.Items.Add(content_item);
                }
                if (item_content_add.Items.IsEmpty)
                {
                    item_content_add.IsEnabled = false;
                }
                contextmenu.Items.Add(item_content_add);

                var item_content_delete = new MenuItem();
                item_content_delete.Style = ColorTheme.MenuItemIconStyle("delete-content.png");
                item_content_delete.Header = "Delete Content";
                item_content_delete.Name = _item_id_delete_content;
                item_content_delete.Click += event_menuitem_click;
                if (_AttachedContent == null)
                {
                    item_content_delete.IsEnabled = false;
                }
                contextmenu.Items.Add(item_content_delete);

                var item_content_dad = new MenuItem();
                item_content_dad.Style = ColorTheme.MenuItemIconStyle("drag-and-drop.png");
                item_content_dad.Header = "Content Swap";
                item_content_dad.IsHitTestVisible = false;
                item_content_dad.Focusable = false;
                contextmenu.Items.Add(item_content_dad);

                var item_content_dad_text = new MenuItem();
                item_content_dad_text.Header = "> Drag&Drop [Middle Mouse Button]";
                item_content_dad_text.IsEnabled = false;
                contextmenu.Items.Add(item_content_dad_text);

            }

            /// <summary>
            /// Called when a menu item in the context menu is clicked.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The routed event arguments.</param>
            private void event_menuitem_click(object sender, RoutedEventArgs e)
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
                    delete_content();
                    _parent_branch.DeleteLeaf();
                }
                else if (content_id == _item_id_delete_content)
                {
                    delete_content();
                }

                // Call Available Contents
                List<ReadContentMetaData_Type> available_contents = _content_callbacks.Item1();
                foreach (var content_data in available_contents)
                {
                    string name = conform_name(content_data.Item1);
                    if (content_id == name)
                    {
                        delete_content();
                        CreateContent(UniqueID.InvalidString, content_data.Item4);
                    }
                }
            }

            /// <summary>
            /// Delete attached content permanently.
            /// </summary>
            private void delete_content(bool only_detach = false)
            {
                _Content.Children.Clear();

                var info_text = new TextBlock();
                info_text.Text = "    [Right-click for Context Menu]";
                info_text.SetResourceReference(TextBlock.ForegroundProperty, "Brush_TextDisabled");
                _Content.Children.Add(info_text);

                if (_AttachedContent != null)
                {
                    if (!only_detach)
                    {
                        if (_AttachedContent.Item1 != UniqueID.InvalidString)
                        {
                            // Call Delete Content
                            _content_callbacks.Item3(_AttachedContent.Item1);
                        }
                    }
                    _AttachedContent = null;
                }
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
                    var drag_and_drop_load = new DragDrop_Type(this, _AttachedContent.Item1, _AttachedContent.Item2);
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
                    }
                    // Drop content from source in target
                    CreateContent(source_content.Item2, source_content.Item3);
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
                var conform_name = name.Replace(" ", ""); ;
                conform_name = conform_name.Replace("(", "");
                conform_name = conform_name.Replace(")", "");
                conform_name = Regex.Replace(conform_name, "[0-9]", "");
                return conform_name;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

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
