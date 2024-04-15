﻿using System;
using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;



/*
 * Window Branch
 * 
 *                         main_grid
 *                             |
 *                     Branch: grid (= Root)
 *                       |              |
 *              Branch: grid          Branch: grid
 *                  |                 |          | 
 *            Leaf: grid     Branch: grid      Branch: grid
 *                                    |            |
 *                              Leaf: grid    Leaf: grid                           
 */
namespace Core
{
    namespace GUI
    {
        public class WindowBranch : AbstractWindow
        {

            /* ------------------------------------------------------------------*/
            // public classes 

            /// <summary>
            /// Configuration data.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public WindowBranch.SplitOrientation Orientation { get; set; }
                public WindowBranch.ChildLocation Location { get; set; }
                public double Position { get; set; }
                public WindowLeaf.Configuration Leaf { get; set; }
                public Tuple<WindowBranch.Configuration, WindowBranch.Configuration> Children { get; set; }
            }


            /* ------------------------------------------------------------------*/
            // public properties 

            public Tuple<WindowBranch, WindowBranch> Children { get { return _children; } }
            public WindowLeaf Leaf { get { return _child_leaf; } }
            public SplitOrientation Orientation { get { return _orientation; } }
            public ChildLocation Location { get { return _location; } }
            public double Position { get { return calculate_position(); } }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="content_callbacks">Content callbacks are piped to each window leaf.</param>
            /// <returns>Content element of root window.</returns>
            public Grid CreateRoot(ContentCallbacks_Type content_callbacks)
            {
                if (content_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing content callbacks");
                    return null;
                }

                _parent_is_root = true;
                _parent_branch = null;
                _children = null;
                _content_callbacks = content_callbacks;

                _content = new Grid();
                _content.Name = "grid_parent_is_root";

                _child_leaf = new WindowLeaf(this, _parent_is_root, _content_callbacks);
                _content.Children.Add(_child_leaf.ContentElement);

                return _content;
            }

            /// <summary>
            /// Split branch and attach two new branches.
            /// </summary>
            /// <param name="orientation">The orientation the window should be split.</param>
            /// <param name="location">The location the existing window should be moved to.</param>
            /// <param name="position">The relative position of the splitter.</param>
            /// <param name="child_branch_1">The first child branch if present.</param>
            /// <param name="child_branch_2">The second child branch if present.</param>
            /// <returns></returns>
            public bool Split(SplitOrientation orientation, ChildLocation location, double position = 0.5, WindowBranch child_branch_1 = null, WindowBranch child_branch_2 = null)
            {
                bool create_new_childs = ((child_branch_1 == null) && (child_branch_2 == null));
                bool use_existing_childs = ((child_branch_1 != null) && (child_branch_2 != null));
                if (!create_new_childs && !use_existing_childs)
                {
                    Log.Default.Msg(Log.Level.Error, "No valid child parameter set");
                    return false;
                }

                double relative_position = ((position < 0.0) ? (0.0) : (position > 1.0) ? (1.0) : position);
                if ((position < 0.0) && (position > 1.0))
                {
                    Log.Default.Msg(Log.Level.Error, "Relative splitter position must be in range [0.0, 1.0]");
                }

                delete_content(_content);

                _orientation = orientation;
                _location = location;

                var gridsplitter = new GridSplitter();
                gridsplitter.Name = "gridplitter_" + UniqueID.Generate();
                gridsplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                gridsplitter.Style = ColorTheme.GridSplitterStyle();

                Grid grid_topleft = (use_existing_childs) ? child_branch_1._content : null;
                if (grid_topleft == null)
                {
                    grid_topleft = new Grid();
                    grid_topleft.Name = "grid_" + UniqueID.Generate();
                }

                Grid grid_bottomright = (use_existing_childs) ? child_branch_2._content : null;
                if (grid_bottomright == null)
                {
                    grid_bottomright = new Grid();
                    grid_bottomright.Name = "grid_" + UniqueID.Generate();
                }

                switch (_orientation)
                {
                    case (SplitOrientation.Vertical):
                        {
                            gridsplitter.Width = ColorTheme.GridSplitterSize;

                            var column_left = new ColumnDefinition();
                            column_left.Width = new GridLength(relative_position, GridUnitType.Star);
                            _content.ColumnDefinitions.Add(column_left);

                            var column_splitter = new ColumnDefinition();
                            column_splitter.Width = new GridLength(ColorTheme.GridSplitterSize);
                            _content.ColumnDefinitions.Add(column_splitter);

                            var column_right = new ColumnDefinition();
                            column_right.Width = new GridLength(1.0 - relative_position, GridUnitType.Star);
                            _content.ColumnDefinitions.Add(column_right);

                            Grid.SetColumn(grid_topleft, 0);
                            Grid.SetColumn(gridsplitter, 1);
                            Grid.SetColumn(grid_bottomright, 2);
                        }
                        break;

                    case (SplitOrientation.Horizontal):
                        {
                            gridsplitter.Height = ColorTheme.GridSplitterSize;

                            var row_top = new RowDefinition();
                            row_top.Height = new GridLength(relative_position, GridUnitType.Star);
                            _content.RowDefinitions.Add(row_top);

                            var row_splitter = new RowDefinition();
                            row_splitter.Height = new GridLength(ColorTheme.GridSplitterSize);
                            _content.RowDefinitions.Add(row_splitter);

                            var row_bottom = new RowDefinition();
                            row_bottom.Height = new GridLength(1.0 - relative_position, GridUnitType.Star);
                            _content.RowDefinitions.Add(row_bottom);

                            Grid.SetRow(grid_topleft, 0);
                            Grid.SetRow(gridsplitter, 1);
                            Grid.SetRow(grid_bottomright, 2);
                        }
                        break;
                }
                _content.Children.Add(grid_topleft);
                _content.Children.Add(gridsplitter);
                _content.Children.Add(grid_bottomright);

                if (create_new_childs)
                {
                    bool move_top_left = (_location == ChildLocation.Top_Left);
                    bool move_bottom_right = (_location == ChildLocation.Bottom_Right);

                    _children = new Tuple<WindowBranch, WindowBranch>(new WindowBranch(), new WindowBranch());
                    _children.Item1.add_leafchild(this, _content_callbacks, grid_topleft, (move_top_left ? _child_leaf : null));
                    _children.Item2.add_leafchild(this, _content_callbacks, grid_bottomright, (move_bottom_right ? _child_leaf : null));

                    _child_leaf = null;
                }

                return true;
            }

            /// <summary>
            /// Reset root branch.
            /// </summary>
            public void ResetRoot()
            {
                if (!_parent_is_root)
                {
                    Log.Default.Msg(Log.Level.Warn, "Tried to reset none root branch");
                    return;
                }

                if (_child_leaf != null)
                {
                    /// Don't do that because this will delete newly added visualizations having the same ID as the previous one:
                    //_child_leaf.ResetLeaf();
                    _child_leaf = null;
                }
                _orientation = SplitOrientation.None;
                _location = ChildLocation.None;
                delete_content(_content);

                if (_children != null)
                {
                    clear_windows(_children.Item1);
                    clear_windows(_children.Item2);
                }
                _children = null;

                _child_leaf = new WindowLeaf(this, _parent_is_root, _content_callbacks);
                _content.Children.Add(_child_leaf.ContentElement);
            }

            /// <summary>
            /// Request the parent branch to delete this branch holding a leaf.
            /// Called by the leaf of this branch.
            /// </summary>
            public void DeleteLeaf()
            {
                if (_parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parent should not be null");
                    return;
                }
                _parent_branch.delete_childbranch(this);
            }

            /// <summary>
            /// DEBUG
            /// </summary>
            ~WindowBranch()
            {
                Console.WriteLine("DEBUG - DTOR: WindowBranch");
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Add window leaf to this branch.
            /// </summary>
            /// <param name="parent_branch">The parent branch.</param>
            /// <param name="content_callbacks">The content callbacks.</param>
            /// <param name="grid">The content element the child is located in.</param>
            /// <param name="child_leaf">The actual child leaf object if present.</param>
            private void add_leafchild(WindowBranch parent_branch, ContentCallbacks_Type content_callbacks, Grid grid, WindowLeaf child_leaf)
            {
                _parent_is_root = false;
                _parent_branch = parent_branch;
                _children = null;
                _content_callbacks = content_callbacks;

                _child_leaf = child_leaf;
                if (_child_leaf == null)
                {
                    _child_leaf = new WindowLeaf(this, _parent_is_root, _content_callbacks);
                }
                else
                {
                    _child_leaf.SetParent(this, _parent_is_root);
                }

                _content = grid;
                _content.Children.Add(_child_leaf.ContentElement);
            }

            /// <summary>
            /// Delete the given child branch. Restructure children accordingly. 
            /// </summary>
            /// <param name="delete_child">The branch to delete.</param>
            private void delete_childbranch(WindowBranch delete_child)
            {
                WindowBranch kept_child = ((delete_child == _children.Item1) ? _children.Item2 : ((delete_child == _children.Item2) ? _children.Item1 : null));
                if (kept_child == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid parameter value constellation");
                    return;
                }

                _orientation = kept_child._orientation;
                _location = kept_child._location;

                // Clear current branch child
                _children = null;
                _child_leaf = null;

                // Reset grids
                delete_content(_content);
                delete_content(kept_child._content);

                if ((kept_child._children != null) && (kept_child._children.Item1 != null) && (kept_child._children.Item2 != null))
                {
                    _children = new Tuple<WindowBranch, WindowBranch>(kept_child._children.Item1, kept_child._children.Item2);

                    _children.Item1._parent_branch = this;
                    _children.Item2._parent_branch = this;

                    Split(_orientation, _location, 0.5, _children.Item1, _children.Item2);
                }
                else if (kept_child._child_leaf != null)
                {
                    _child_leaf = kept_child._child_leaf;
                    _child_leaf.SetParent(this, _parent_is_root);
                    _content.Children.Add(_child_leaf.ContentElement);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid parameter value constellation");
                    return;
                }

                // Clear unused branch child
                kept_child.Reset();
            }

            /// <summary>
            /// Clear content of provided Grid.
            /// </summary>
            /// <param name="content_element">The grid to clear.</param>
            private void delete_content(Grid content_element)
            {
                if (content_element == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Provided Grid is null");
                }
                content_element.Children.Clear();
                content_element.RowDefinitions.Clear();
                content_element.ColumnDefinitions.Clear();
                content_element.UpdateLayout();
            }

            /// <summary>
            /// Calculate the relative position of the splitter.
            /// </summary>
            /// <returns>The relative position of the splitter.</returns>
            private double calculate_position()
            {
                /// NOTE: GridUnitType is fixed and always Star independent of the actual value, 
                /// so we have to check for the actual value to determine whether the value is absolute or relative.
                double value = 0.0;
                switch (_orientation)
                {
                    case (SplitOrientation.Horizontal):
                        value = _content.RowDefinitions[0].Height.Value;
                        if (value > 1.0) // Absolute value (in Pixel)
                        {
                            double total_height = 0;
                            foreach (var rowdef in _content.RowDefinitions)
                            {
                                total_height += rowdef.Height.Value;
                            }
                            return ((total_height > 0.0) ? (value / total_height) : (0.0));
                        }
                        else // Relative value (Star)
                        {
                            return value;
                        }
                    case (SplitOrientation.Vertical):
                        value = _content.ColumnDefinitions[0].Width.Value;
                        if (value > 1.0)
                        {
                            double total_width = 0;
                            foreach (var coldef in _content.ColumnDefinitions)
                            {
                                total_width += coldef.Width.Value;
                            }
                            return ((total_width > 0.0) ? (value / total_width) : (0.0));
                        }
                        else
                        {
                            return value;
                        }
                }
                return value;
            }

            /// <summary>
            /// Clear window tree recursively.
            /// </summary>
            /// <param name="branch">The branch to start with.</param>
            private void clear_windows(WindowBranch branch)
            {
                if (branch.Children != null)
                {
                    if (branch.Children.Item1 != null)
                    {
                        clear_windows(branch.Children.Item1);
                    }
                    if (branch.Children.Item2 != null)
                    {
                        clear_windows(branch.Children.Item2);
                    }
                }
                branch.reset_window();
            }

            /// <summary>
            /// Reset attached resources.
            /// </summary>
            private void reset_window()
            {
                _children = null;
                if (_child_leaf != null)
                {
                    /// Don't do that because this will delete newly added visualizations having the same ID as the previous one:
                    //_child_leaf.ResetLeaf();
                    _child_leaf = null;
                }
                _orientation = SplitOrientation.None;
                _location = ChildLocation.None;
                delete_content(_content);
                base.Reset();
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Tuple<WindowBranch, WindowBranch> _children = null;
            private WindowLeaf _child_leaf = null;
            private SplitOrientation _orientation = SplitOrientation.None;
            private ChildLocation _location = ChildLocation.None;
        }
    }
}
