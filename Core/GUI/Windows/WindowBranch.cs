using System;
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

using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


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

            public Tuple<WindowBranch, WindowBranch> _Children { get; private set; } = null;
            public WindowLeaf _Leaf { get; private set; } = null;
            public SplitOrientation _Orientation { get; private set; } = SplitOrientation.None;
            public ChildLocation _Location { get; private set; } = ChildLocation.None;
            public double _Position { get { return calculate_position(); } }


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
                _Children = null;
                _content_callbacks = content_callbacks;

                _Content = new Grid();
                _Content.Name = "grid_parent_is_root";

                _Leaf = new WindowLeaf(this, _parent_is_root, _content_callbacks);
                _Content.Children.Add(_Leaf._Content);

                return _Content;
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

                delete_content(_Content);

                _Orientation = orientation;
                _Location = location;

                var gridsplitter = new GridSplitter();
                gridsplitter.Name = "gridplitter_" + UniqueID.GenerateString();
                gridsplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                gridsplitter.Style = ColorTheme.GridSplitterStyle();

                Grid grid_topleft = (use_existing_childs) ? child_branch_1._Content : null;
                if (grid_topleft == null)
                {
                    grid_topleft = new Grid();
                    grid_topleft.Name = "grid_" + UniqueID.GenerateString();
                }

                Grid grid_bottomright = (use_existing_childs) ? child_branch_2._Content : null;
                if (grid_bottomright == null)
                {
                    grid_bottomright = new Grid();
                    grid_bottomright.Name = "grid_" + UniqueID.GenerateString();
                }

                switch (_Orientation)
                {
                    case (SplitOrientation.Vertical):
                        {
                            gridsplitter.Width = ColorTheme._GridSplitterSize;

                            var column_left = new ColumnDefinition();
                            column_left.Width = new GridLength(relative_position, GridUnitType.Star);
                            _Content.ColumnDefinitions.Add(column_left);

                            var column_splitter = new ColumnDefinition();
                            column_splitter.Width = new GridLength(ColorTheme._GridSplitterSize);
                            _Content.ColumnDefinitions.Add(column_splitter);

                            var column_right = new ColumnDefinition();
                            column_right.Width = new GridLength(1.0 - relative_position, GridUnitType.Star);
                            _Content.ColumnDefinitions.Add(column_right);

                            Grid.SetColumn(grid_topleft, 0);
                            Grid.SetColumn(gridsplitter, 1);
                            Grid.SetColumn(grid_bottomright, 2);
                        }
                        break;

                    case (SplitOrientation.Horizontal):
                        {
                            gridsplitter.Height = ColorTheme._GridSplitterSize;

                            var row_top = new RowDefinition();
                            row_top.Height = new GridLength(relative_position, GridUnitType.Star);
                            _Content.RowDefinitions.Add(row_top);

                            var row_splitter = new RowDefinition();
                            row_splitter.Height = new GridLength(ColorTheme._GridSplitterSize);
                            _Content.RowDefinitions.Add(row_splitter);

                            var row_bottom = new RowDefinition();
                            row_bottom.Height = new GridLength(1.0 - relative_position, GridUnitType.Star);
                            _Content.RowDefinitions.Add(row_bottom);

                            Grid.SetRow(grid_topleft, 0);
                            Grid.SetRow(gridsplitter, 1);
                            Grid.SetRow(grid_bottomright, 2);
                        }
                        break;
                }
                _Content.Children.Add(grid_topleft);
                _Content.Children.Add(gridsplitter);
                _Content.Children.Add(grid_bottomright);

                if (create_new_childs)
                {
                    bool move_top_left = (_Location == ChildLocation.Top_Left);
                    bool move_bottom_right = (_Location == ChildLocation.Bottom_Right);

                    _Children = new Tuple<WindowBranch, WindowBranch>(new WindowBranch(), new WindowBranch());
                    _Children.Item1.add_leafchild(this, _content_callbacks, grid_topleft, (move_top_left ? _Leaf : null));
                    _Children.Item2.add_leafchild(this, _content_callbacks, grid_bottomright, (move_bottom_right ? _Leaf : null));

                    _Leaf = null;
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

                if (_Leaf != null)
                {
                    /// Don't do that because this will delete newly added visualizations having the same ID as the previous one:
                    //_Leaf.ResetLeaf();
                    _Leaf = null;
                }
                _Orientation = SplitOrientation.None;
                _Location = ChildLocation.None;
                delete_content(_Content);

                if (_Children != null)
                {
                    clear_windows(_Children.Item1);
                    clear_windows(_Children.Item2);
                }
                _Children = null;

                _Leaf = new WindowLeaf(this, _parent_is_root, _content_callbacks);
                _Content.Children.Add(_Leaf._Content);
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
                _Children = null;
                _content_callbacks = content_callbacks;

                _Leaf = child_leaf;
                if (_Leaf == null)
                {
                    _Leaf = new WindowLeaf(this, _parent_is_root, _content_callbacks);
                }
                else
                {
                    _Leaf.SetParent(this, _parent_is_root);
                }

                _Content = grid;
                _Content.Children.Add(_Leaf._Content);
            }

            /// <summary>
            /// Delete the given child branch. Restructure children accordingly. 
            /// </summary>
            /// <param name="delete_child">The branch to delete.</param>
            private void delete_childbranch(WindowBranch delete_child)
            {
                WindowBranch kept_child = ((delete_child == _Children.Item1) ? _Children.Item2 : ((delete_child == _Children.Item2) ? _Children.Item1 : null));
                if (kept_child == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid parameter value constellation");
                    return;
                }

                _Orientation = kept_child._Orientation;
                _Location = kept_child._Location;

                // Clear current branch child
                _Children = null;
                _Leaf = null;

                // Reset grids
                delete_content(_Content);
                delete_content(kept_child._Content);

                if ((kept_child._Children != null) && (kept_child._Children.Item1 != null) && (kept_child._Children.Item2 != null))
                {
                    _Children = new Tuple<WindowBranch, WindowBranch>(kept_child._Children.Item1, kept_child._Children.Item2);

                    _Children.Item1._parent_branch = this;
                    _Children.Item2._parent_branch = this;

                    Split(_Orientation, _Location, 0.5, _Children.Item1, _Children.Item2);
                }
                else if (kept_child._Leaf != null)
                {
                    _Leaf = kept_child._Leaf;
                    _Leaf.SetParent(this, _parent_is_root);
                    _Content.Children.Add(_Leaf._Content);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid parameter value constellation");
                    return;
                }

                // Clear unused branch child
                kept_child.reset();
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
                switch (_Orientation)
                {
                    case (SplitOrientation.Horizontal):
                        value = _Content.RowDefinitions[0].Height.Value;
                        if (value > 1.0) // Absolute value (in Pixel)
                        {
                            double total_height = 0;
                            foreach (var rowdef in _Content.RowDefinitions)
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
                        value = _Content.ColumnDefinitions[0].Width.Value;
                        if (value > 1.0)
                        {
                            double total_width = 0;
                            foreach (var coldef in _Content.ColumnDefinitions)
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
                if (branch._Children != null)
                {
                    if (branch._Children.Item1 != null)
                    {
                        clear_windows(branch._Children.Item1);
                    }
                    if (branch._Children.Item2 != null)
                    {
                        clear_windows(branch._Children.Item2);
                    }
                }
                branch.reset_window();
            }

            /// <summary>
            /// Reset attached resources.
            /// </summary>
            private void reset_window()
            {
                _Children = null;
                if (_Leaf != null)
                {
                    /// Don't do that because this will delete newly added visualizations having the same ID as the previous one:
                    //_Leaf.ResetLeaf();
                    _Leaf = null;
                }
                _Orientation = SplitOrientation.None;
                _Location = ChildLocation.None;
                delete_content(_Content);
                base.reset();
            }
        }
    }
}
