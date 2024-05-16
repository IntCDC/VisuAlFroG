using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Core.Abstracts;
using Core.Utilities;
using Newtonsoft.Json;



/*
 * Configuration Service
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class ConfigurationService : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public delegates

            public delegate string RegisterCollectCallback_Delegate();

            public delegate bool RegisterApplyCallback_Delegate(string configurations);


            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            /// [STATIC] Provides static serialization of configurations to JSON string.
            /// </summary>
            /// <typeparam name="T">The configurations type.</typeparam>
            /// <param name="window_data">The actual configurations object.</param>
            /// <returns>The serialized configurations.</returns>
            public static string Serialize<T>(T window_data)
            {
                try
                {
                    string configurations = JsonConvert.SerializeObject(window_data); //, Formatting.Indented);
                    return configurations;
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return null;
            }

            /// <summary>
            /// [STATIC] Provides static deserialization contained in JSON string.
            /// </summary>
            /// <typeparam name="T">The configurations type.</typeparam>
            /// <param name="content">The JSON string.</param>
            /// <returns>The configurations object.</returns>
            public static T Deserialize<T>(string content)
            {
                try
                {
                    T configurations = JsonConvert.DeserializeObject<T>(content);
                    return configurations;
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return default(T);
            }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _collect_callbacks = new Dictionary<string, RegisterCollectCallback_Delegate>();
                _apply_callbacks = new Dictionary<string, RegisterApplyCallback_Delegate>();

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _collect_callbacks.Clear();
                    _collect_callbacks = new Dictionary<string, RegisterCollectCallback_Delegate>();

                    _apply_callbacks.Clear();
                    _apply_callbacks = new Dictionary<string, RegisterApplyCallback_Delegate>();

                    _initialized = false;
                }
                return true;
            }

            /// <summary>
            /// Register object that provides configurations. Unique name of requesting caller is required in order to create separate JSON entries.
            /// </summary>
            /// <param name="name">Unique name of the caller.</param>
            /// <param name="collect_callback">Callback for collecting all configurations.</param>
            /// <param name="apply_callback">callback for applying all configurations.</param>
            public void RegisterConfiguration(string name, RegisterCollectCallback_Delegate collect_callback, RegisterApplyCallback_Delegate apply_callback)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to registration of callbacks");
                    return;
                }
                if ((collect_callback == null) || (apply_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                    return;
                }

                _collect_callbacks.Add(name, collect_callback);
                _apply_callbacks.Add(name, apply_callback);
            }

            /// <summary>
            /// Request saving all configurations to a JSON file.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public bool Save()
            {
                var _serialize_strucutre = new Dictionary<string, string>();
                foreach (var collect_callback in _collect_callbacks)
                {
                    // Call callback for collecting all configurations
                    _serialize_strucutre.Add(collect_callback.Key, collect_callback.Value());
                }
                string configurations = Serialize<Dictionary<string, string>>(_serialize_strucutre);
                FileDialogHelper.Save(configurations, "Save Configuration", "JSON files (*.json)|*.json", ResourcePaths.CreateFileName("configuration", "json"));
                return true;
            }

            /// <summary>
            /// Request loading of configurations from a JSON file.
            /// </summary>
            /// <param name="configuration_file">The configuration file. If empty file dialog is opened.</param>
            /// <returns>True on success, false otherwise.</returns>
            public bool LoadFile(string configuration_file)
            {
                try
                {
                    var fileStream = new FileStream(configuration_file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        Log.Default.Msg(Log.Level.Info, "Loading configuration from file: '" + configuration_file + "'");

                        string config_content = reader.ReadToEnd();
                        var _deserialize_structure = Deserialize<Dictionary<string, string>>(config_content);
                        foreach (var apply_callback in _deserialize_structure)
                        {
                            if (_apply_callbacks.ContainsKey(apply_callback.Key))
                            {
                                // Call callback for applying configurations
                                _apply_callbacks[apply_callback.Key](apply_callback.Value);
                            }
                        }
                        return true;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return false;
            }

            public bool LoadFileDialog()
            {
                string config_file = FileDialogHelper.Load("Load Configuration", "JSON files (*.json)|*.json", ResourcePaths.CreateFileName("configuration", "json"));
                return LoadFile(config_file);
            }

            public override void AttachMenu(MainMenuBar menu_bar)
            {
                var config_menu = MainMenuBar.GetDefaultMenuItem("Configuration");
                config_menu.Items.Add(MainMenuBar.GetDefaultMenuItem("Save", Save));
                config_menu.Items.Add(MainMenuBar.GetDefaultMenuItem("Load", LoadFileDialog));
                menu_bar.AddMenu(MainMenuBar.PredefinedMenuOption.FILE, config_menu);
            }


            /* ------------------------------------------------------------------*/
            // public variables

            private Dictionary<string, RegisterCollectCallback_Delegate> _collect_callbacks = null;
            private Dictionary<string, RegisterApplyCallback_Delegate> _apply_callbacks = null;
        }
    }
}
