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



/*
 * Window Manager
 * 
 */

using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


namespace Core
{
    namespace GUI
    {
        public class WindowManager : AbstractService, IAbstractConfigurationData
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public bool Initialize(ContentCallbacks_Type content_callbacks)
            {
                if (_initialized)
                {
                    Terminate();
                }
                if (content_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing content callbacks");
                    return false;
                }

                _timer.Start();

                _window_root = new WindowBranch();
                _content = _window_root.CreateRoot(content_callbacks);
                bool initialized = (_content != null);

                _timer.Stop();
                _initialized = initialized;
                return _initialized;
            }

            public Panel Attach()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }
                return _content;
            }

            public override bool Terminate()
            {
                bool terminated = true;
                if (_initialized)
                {
                    _content = null;
                    _window_root = null;

                    _initialized = false;
                }
                return terminated;
            }

            public string CollectConfigurations()
            {
                var configurations = new WindowBranch.Configuration();
                collect_configurations(_window_root, configurations);
                return ConfigurationService.Serialize<WindowBranch.Configuration>(configurations);
            }

            public bool ApplyConfigurations(string configurations)
            {
                var windowbranch_configurations = ConfigurationService.Deserialize<WindowBranch.Configuration>(configurations);
                if (windowbranch_configurations != null)
                {
                    _window_root.ResetRoot();
                    apply_configurations(_window_root, windowbranch_configurations);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Load default window configuration.
            /// </summary>
            public void CreateDefault()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }

                /*
                _______________________________
                |Data Viewer |                |
                |            |                |
                |            |                |
                |____________|________________|
                |Log Console                  |
                |_____________________________|
                */
                _window_root.Split(WindowBranch.SplitOrientation.Horizontal, WindowBranch.ChildLocation.None, 0.7);
                _window_root._Children.Item1.Split(WindowBranch.SplitOrientation.Vertical, WindowBranch.ChildLocation.None, 0.31);
                _window_root._Children.Item1._Children.Item1._Leaf.CreateContent(UniqueID.InvalidInt, AbstractVisualization.TypeString_DataViewer);
                _window_root._Children.Item1._Children.Item2._Leaf.CreateContent(UniqueID.InvalidInt, AbstractVisualization.TypeString_SciChartLines);
                _window_root._Children.Item2._Leaf.CreateContent(UniqueID.InvalidInt, AbstractVisualization.TypeString_LogConsole);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            /// <summary>
            /// Traverse window tree in breadth first order to gather all configurations recursively.
            /// </summary>
            /// <param name="branch">The current branch object</param>
            /// <param name="configurations">The configurations are appended according to the branch configurations.</param>
            void collect_configurations(WindowBranch branch, WindowBranch.Configuration configurations)
            {
                if (branch == null)
                {
                    return;
                }
                if (configurations == null)
                {
                    return;
                }
                configurations.Location = branch._Location;
                configurations.Orientation = branch._Orientation;
                configurations.Position = branch._Position;

                if (branch._Leaf != null)
                {
                    var attached_content = branch._Leaf._AttachedContent;
                    if (attached_content != null)
                    {
                        configurations.Leaf = new WindowLeaf.Configuration()
                        {
                            ContentID = attached_content.Item1,
                            ContentType = attached_content.Item2,
                            Name = branch._Leaf._Name,
                        };
                    }
                }
                else if (branch._Children != null)
                {
                    var child1 = branch._Children.Item1;
                    var child2 = branch._Children.Item2;

                    configurations.Children = new Tuple<WindowBranch.Configuration, WindowBranch.Configuration>(new WindowBranch.Configuration(), new WindowBranch.Configuration());

                    collect_configurations(child1, configurations.Children.Item1);
                    collect_configurations(child2, configurations.Children.Item2);
                }
            }


            /// <summary>
            /// Traverse window tree in breadth first order and set branch configurations recursively.
            /// </summary>
            /// <param name="branch">The initial branch object.</param>
            /// <param name="configurations">The configurations object.</param>
            void apply_configurations(WindowBranch branch, WindowBranch.Configuration configurations)
            {
                if (configurations == null)
                {
                    return;
                }

                if (configurations.Leaf != null)
                {
                    branch._Leaf._Name = configurations.Leaf.Name;
                    branch._Leaf.CreateContent(configurations.Leaf.ContentID, configurations.Leaf.ContentType);
                }
                else if (configurations.Children != null)
                {
                    branch.Split(configurations.Orientation, configurations.Location, configurations.Position);

                    apply_configurations(branch._Children.Item1, configurations.Children.Item1);
                    apply_configurations(branch._Children.Item2, configurations.Children.Item2);
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _content = null;
            private WindowBranch _window_root = null;

            #endregion
        }
    }
}
