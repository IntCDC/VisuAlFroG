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
        public class WindowManager : AbstractService
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            public delegate bool SettingsSave_Delegate();

            public delegate bool SettingsLoad_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            public bool Initialize(ContentCallbacks_Type content_callbacks)
            {
                if (_initilized)
                {
                    Terminate();
                    _settingsservice.Terminate();
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

                _settingsservice = new SettingsService();

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

                    terminated &= _settingsservice.Terminate();

                    _initilized = false;
                }
                return terminated;
            }


            public bool SaveSettings()
            {
                WindowBranch.Settings settings = new WindowBranch.Settings();
                collect_settings(_window_root, settings);
                return _settingsservice.Save<WindowBranch.Settings>(settings); ;
            }


            public bool LoadSettings()
            {
                WindowBranch.Settings settings = _settingsservice.Load<WindowBranch.Settings>();
                if (settings != null)
                {
                    apply_settings(_window_root, settings);
                    return true;
                }
                return false;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Traverse window tree in breadth first order to gather all settings
            /// </summary>
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
                            ContentType = attached_content.Item2.FullName,
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
            /// Traverse window tree in breadth first order to set branch settings
            /// </summary>
            void apply_settings(WindowBranch branch, WindowBranch.Settings settings)
            {
                if (settings == null)
                {
                    return;
                }

                if (settings.Leaf != null)
                {
                    try
                    {
                        Type content_type = Type.GetType(settings.Leaf.ContentType, true);
                        branch.Leaf.CreateContent(settings.Leaf.ContentID, content_type);
                    }
                    catch (TypeLoadException e)
                    {
                        Log.Default.Msg(Log.Level.Error, e.Message);
                    }
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

            private SettingsService _settingsservice = null;
        }
    }

}
