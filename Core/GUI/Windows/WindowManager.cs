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
namespace Core
{
    namespace GUI
    {
        public class WindowManager : AbstractService, IAbstractSettingData
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
                return _content;
            }

            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    _content = null;
                    _window_root = null;
                    _content_callbacks = null;

                    _initilized = false;
                }
                return terminated;
            }

            public string CollectSettings()
            {
                var settings = new WindowBranch.Settings();
                collect_settings(_window_root, settings);
                return SettingsService.Serialize<WindowBranch.Settings>(settings);
            }

            public bool ApplySettings(string settings)
            {
                var windowbranch_settings = SettingsService.Deserialize<WindowBranch.Settings>(settings);
                if (windowbranch_settings != null)
                {
                    apply_settings(_window_root, windowbranch_settings);
                    return true;
                }
                return false;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Traverse window tree in breadth first order to gather all settings recursively.
            /// </summary>
            /// <param name="branch">The current branch object</param>
            /// <param name="settings">The settings are appended according to the branch settings.</param>
            void collect_settings(WindowBranch branch, WindowBranch.Settings settings)
            {
                if (branch == null)
                {
                    return;
                }
                if (settings == null)
                {
                    return;
                }
                settings.Location = branch.Location;
                settings.Orientation = branch.Orientation;
                settings.Position = branch.Position;

                if (branch.Leaf != null)
                {
                    var attached_content = branch.Leaf.AttachedContent;
                    if (attached_content != null)
                    {
                        settings.Leaf = new WindowLeaf.Settings()
                        {
                            ContentID = attached_content.Item1,
                            ContentType = attached_content.Item2,
                        };
                    }
                }
                else if (branch.Children != null)
                {
                    var child1 = branch.Children.Item1;
                    var child2 = branch.Children.Item2;

                    settings.Children = new Tuple<WindowBranch.Settings, WindowBranch.Settings>(new WindowBranch.Settings(), new WindowBranch.Settings());

                    collect_settings(child1, settings.Children.Item1);
                    collect_settings(child2, settings.Children.Item2);
                }
            }


            /// <summary>
            /// Traverse window tree in breadth first order and set branch settings recursively.
            /// </summary>
            /// <param name="branch">The initial branch object.</param>
            /// <param name="settings">The settings object.</param>
            void apply_settings(WindowBranch branch, WindowBranch.Settings settings)
            {
                if (settings == null)
                {
                    return;
                }

                if (settings.Leaf != null)
                {
                    branch.Leaf.CreateContent(settings.Leaf.ContentID, settings.Leaf.ContentType);
                }
                else if (settings.Children != null)
                {
                    branch.Split(settings.Orientation, settings.Location, settings.Position);

                    apply_settings(branch.Children.Item1, settings.Children.Item1);
                    apply_settings(branch.Children.Item2, settings.Children.Item2);
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
