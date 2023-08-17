using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;
using Core.GUI;



/*
 * Child Branch
 * 
 * 
 * NOTE:
 * The canvas actually contains the content and is passed on to sub grids
 * 
 *                          main_grid
 *                              |
 *                     ChildBranch: grid (= Root)
 *                       |                 |
 *         ChildBranch: grid           ChildBranch: grid
 *                       |             |              | 
 *         ChildLeaf: canvas    ChildBranch: grid     ChildBranch: grid
 *                                    |                   |
 *                               ChildLeaf: canvas    ChildLeaf: canvas
 *                               
 */
namespace Core
{
    namespace GUI
    {
        public sealed class WindowBranch : AbstractWindow
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public void CreateRoot(Grid parent_element, AvailableContentCallback available_content, RequestContentCallback request_content)
            {
                if (parent_element == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Grid parameter must not be NULL");
                    return;
                }

                _parent_is_root = true;
                _parent_branch = null;
                _child_branch_1 = null;
                _child_branch_2 = null;
                _available_content = available_content;
                _request_content = request_content;

                _child_leaf = new WindowLeaf(this, _parent_is_root, _available_content, _request_content);

                _content = new Grid();
                _content.Name = "grid_parent_is_root";
                _content.Children.Add(_child_leaf.GetContentElement());
                parent_element.Children.Add(_content);
            }


            public void Split(SplitOrientation orientation, ChildLocation location, WindowBranch child_branch_1 = null, WindowBranch child_branch_2 = null)
            {
                bool create_new_childs = ((child_branch_1 == null) && (child_branch_2 == null));
                bool use_existing_childs = ((child_branch_1 != null) && (child_branch_2 != null));
                if (!create_new_childs && !use_existing_childs)
                {
                    Log.Default.Msg(Log.Level.Error, "No valid child paramter set");
                    return;
                }

                clear_content(_content);

                _orientation = orientation;
                _location = location;

                var gridsplitter = new GridSplitter();
                gridsplitter.Name = "gridplitter_" + UniqueStringID.Generate();
                gridsplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                gridsplitter.Style = ColorTheme.GridSplitterStyle();

                Grid grid_topleft = (use_existing_childs) ? child_branch_1._content : null;
                if (grid_topleft == null)
                {
                    grid_topleft = new Grid();
                    grid_topleft.Name = "grid_" + UniqueStringID.Generate();
                }

                Grid grid_bottomright = (use_existing_childs) ? child_branch_2._content : null;
                if (grid_bottomright == null)
                {
                    grid_bottomright = new Grid();
                    grid_bottomright.Name = "grid_" + UniqueStringID.Generate();
                }

                switch (_orientation)
                {
                    case (SplitOrientation.Vertical):
                        {
                            gridsplitter.Width = ColorTheme.GridSplitterSize;

                            var column_left = new ColumnDefinition();
                            column_left.Width = new GridLength(1, GridUnitType.Star);
                            _content.ColumnDefinitions.Add(column_left);

                            var column_splitter = new ColumnDefinition();
                            column_splitter.Width = new GridLength(ColorTheme.GridSplitterSize);
                            _content.ColumnDefinitions.Add(column_splitter);

                            var column_right = new ColumnDefinition();
                            column_right.Width = new GridLength(1, GridUnitType.Star); ;
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
                            row_top.Height = new GridLength(1, GridUnitType.Star);
                            _content.RowDefinitions.Add(row_top);

                            var row_splitter = new RowDefinition();
                            row_splitter.Height = new GridLength(ColorTheme.GridSplitterSize);
                            _content.RowDefinitions.Add(row_splitter);

                            var row_bottom = new RowDefinition();
                            row_bottom.Height = new GridLength(1, GridUnitType.Star);
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

                    _child_branch_1 = new WindowBranch();
                    _child_branch_1.add_leafchild(this, _available_content, _request_content, grid_topleft, (move_top_left ? _child_leaf : null));

                    _child_branch_2 = new WindowBranch();
                    _child_branch_2.add_leafchild(this, _available_content, _request_content, grid_bottomright, (move_bottom_right ? _child_leaf : null));

                    _child_leaf = null;
                }
            }


            public void DeleteLeaf()
            {
                if (_parent_branch == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Parent should not be NULL");
                    return;
                }
                _parent_branch.delete_childbranch(this);
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void add_leafchild(WindowBranch parent_branch, AvailableContentCallback available_content, RequestContentCallback request_content, Grid grid, WindowLeaf child_leaf)
            {
                _parent_is_root = false;
                _parent_branch = parent_branch;
                _child_branch_1 = null;
                _child_branch_2 = null;
                _available_content = available_content;
                _request_content = request_content;

                _child_leaf = child_leaf;
                if (_child_leaf == null)
                {
                    _child_leaf = new WindowLeaf(this, _parent_is_root, _available_content, _request_content);
                }
                else
                {
                    _child_leaf.SetParent(this, _parent_is_root);
                }

                _content = grid;
                _content.Children.Add(_child_leaf.GetContentElement());
            }


            private void delete_childbranch(WindowBranch delete_child)
            {
                WindowBranch kept_child = ((delete_child == _child_branch_1) ? _child_branch_2 : ((delete_child == _child_branch_2) ? _child_branch_1 : null));
                if (kept_child == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid parameter value constellation");
                    return;
                }

                _orientation = kept_child._orientation;
                _location = kept_child._location;

                // Clear current branch child
                _child_branch_1 = null;
                _child_branch_2 = null;
                _child_leaf = null;

                // Reset grids
                clear_content(_content);
                clear_content(kept_child._content);

                if ((kept_child._child_branch_1 != null) && (kept_child._child_branch_2 != null))
                {
                    _child_branch_1 = kept_child._child_branch_1;
                    _child_branch_1._parent_branch = this;

                    _child_branch_2 = kept_child._child_branch_2;
                    _child_branch_2._parent_branch = this;

                    Split(_orientation, _location, _child_branch_1, _child_branch_2);
                }
                else if (kept_child._child_leaf != null)
                {
                    _child_leaf = kept_child._child_leaf;
                    _child_leaf.SetParent(this, _parent_is_root);
                    _content.Children.Add(_child_leaf.GetContentElement());
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Invalid parameter value constellation");
                    return;
                }

                // Clear unused branch child
                kept_child._content = null;
                kept_child._child_branch_1 = null;
                kept_child._child_branch_2 = null;
                kept_child._child_leaf = null;
                kept_child._parent_branch = null;
            }

            private void clear_content(Grid cotent_element)
            {
                cotent_element.Children.Clear();
                cotent_element.RowDefinitions.Clear();
                cotent_element.ColumnDefinitions.Clear();
                cotent_element.UpdateLayout();
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private WindowBranch _child_branch_1 = null;
            private WindowBranch _child_branch_2 = null;
            private WindowLeaf _child_leaf = null;

            private SplitOrientation _orientation = SplitOrientation.None;
            private ChildLocation _location = ChildLocation.None;
        }
    }
}
