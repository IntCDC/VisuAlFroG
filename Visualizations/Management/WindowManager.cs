using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;
using Core.Abstracts;
using Core.GUI;
using System.Windows.Controls;
using System.Runtime.Remoting.Contexts;
using Visualizations.Types;



using ContentCallbacks = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.RequestContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


/*
 * Window Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {

        public class WindowManager : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public bool Initialize(ContentCallbacks content_callbacks, Grid parent_content)
            {
                if (_initilized)
                {
                    Terminate();
                }
                if (content_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing content callbacks");
                    return false;
                }
                if (parent_content == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing parent content element");
                    return false;
                }
                _content_callbacks = content_callbacks;
                _parent_content = parent_content;

                _window_root = new WindowBranch();
                _content_root = _window_root.CreateRoot(_content_callbacks);
                bool initilized = (_content_root != null);

                _initilized = initilized;
                return _initilized;
            }


            public override bool Execute()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                _parent_content.Children.Add(_content_root);

                load_inital_tree();
                traverse_windows(_window_root, 0);

                return true;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    _window_root = null;

                    /// TODO Should be set in initialize
                    //_content_callbacks = null;
                    //_parent_content = null;

                    _initilized = false;
                }
                return terminated;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// DEBUG Load initial content tree
            /// </summary>
            void load_inital_tree()
            {
                if (_window_root.Split(AbstractWindow.SplitOrientation.Horizontal, AbstractWindow.ChildLocation.Top_Left, 0.75))
                {
                    if (_window_root.Children != null)
                    {
                        var child1 = _window_root.Children.Item1;
                        if (child1 != null)
                        {
                            if (child1.Split(AbstractWindow.SplitOrientation.Vertical, AbstractWindow.ChildLocation.Top_Left, 0.5))
                            {
                                var child11 = child1.Children.Item1;
                                if (child11 != null)
                                {
                                    child11.Leaf.AttachContent(UniqueID.Invalid, typeof(DEBUGLines));
                                }
                                var child12 = child1.Children.Item2;
                                if (child12 != null)
                                {
                                    child12.Leaf.AttachContent(UniqueID.Invalid, typeof(DEBUGColumns));
                                }
                            }
                        }

                        var child2 = _window_root.Children.Item2;
                        if (child2 != null)
                        {
                            child2.Leaf.AttachContent(UniqueID.Invalid, typeof(LogContent));
                        }
                    }
                }
            }


            /// <summary>
            /// DEBUG Traverse window tree in breadth first order
            /// </summary>
            void traverse_windows(WindowBranch branch, int depth)
            {
                if (branch == null)
                {
                    return;
                }

                var orientation = branch.Orientation;
                var location = branch.Location;
                var position = branch.Position;
                // Restore:
                ///branch.Split(orientation, location, position);
                Log.Default.Msg(Log.Level.Debug, "[" + depth.ToString() + "] Orientation: " + orientation.ToString() + " | Location: " + location.ToString() + " | Position: " + position.ToString());

                var leaf = branch.Leaf;
                if (leaf != null)
                {
                    var attached_content = leaf.AttachedContent;
                    if (attached_content != null)
                    {
                        Log.Default.Msg(Log.Level.Debug, "[" + depth.ToString() + "] Attached Content - ID: " + attached_content.Item1 + " | Type: " + attached_content.Item2.ToString());
                    }
                    // Restore: 
                    ///leaf.AttachContent(content_id, content_type);
                }

                if (branch.Children != null)
                {
                    var child1 = branch.Children.Item1;
                    var child2 = branch.Children.Item2;

                    traverse_windows(child1, (depth + 1));
                    traverse_windows(child2, (depth + 1));
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private WindowBranch _window_root = null;
            private Grid _content_root = null;

            private ContentCallbacks _content_callbacks = null;
            private Grid _parent_content = null;
        }
    }

}
