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

            public bool Initialize(ContentCallbacks_Type content_callbacks)
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

                _timer.Start();

                _content_callbacks = content_callbacks;

                _window_root = new WindowBranch();
                _content = _window_root.CreateRoot(_content_callbacks);
                bool initilized = (_content != null);

                _timer.Stop();
                _initilized = initilized;
                return _initilized;
            }


            public Panel Attach()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                create_startup_default();
                // traverse_windows(_window_root, 0);

                return _content;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    _window_root = null;
                    _content_callbacks = null;

                    _initilized = false;
                }
                return terminated;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Load initial window tree
            /// </summary>
            void create_startup_default()
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
            void traverse_window_tree(WindowBranch branch, int depth)
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
                //Log.Default.Msg(Log.Level.Debug, "[" + depth.ToString() + "] Orientation: " + orientation.ToString() + " | Location: " + location.ToString() + " | Position: " + position.ToString());

                var leaf = branch.Leaf;
                if (leaf != null)
                {
                    var attached_content = leaf.AttachedContent;
                    if (attached_content != null)
                    {
                        //Log.Default.Msg(Log.Level.Debug, "[" + depth.ToString() + "] Attached Content - ID: " + attached_content.Item1 + " | Type: " + attached_content.Item2.ToString());
                    }
                    // Restore: 
                    ///leaf.AttachContent(content_id, content_type);
                }

                if (branch.Children != null)
                {
                    var child1 = branch.Children.Item1;
                    var child2 = branch.Children.Item2;

                    traverse_window_tree(child1, (depth + 1));
                    traverse_window_tree(child2, (depth + 1));
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Grid _content = null;
            private WindowBranch _window_root = null;
            private ContentCallbacks_Type _content_callbacks = null;
        }
    }

}
