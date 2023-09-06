﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Abstracts;
using Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.Collections;


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

            public delegate bool Save_Delegate();

            public delegate bool Load_Delegate();

            public delegate string RegisterCollect_Delegate();

            public delegate bool RegisterApply_Delegate(string configurations);


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
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                _collect_callbacks = new Dictionary<string, RegisterCollect_Delegate>();
                _apply_callbacks = new Dictionary<string, RegisterApply_Delegate>();

                _timer.Stop();
                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }

            public override bool Terminate()
            {
                if (_initilized)
                {
                    _collect_callbacks.Clear();
                    _collect_callbacks = new Dictionary<string, RegisterCollect_Delegate>();

                    _apply_callbacks.Clear();
                    _apply_callbacks = new Dictionary<string, RegisterApply_Delegate>();

                    _initilized = false;
                }
                return true;
            }

            /// <summary>
            /// Register object that provides configurations. Unique name of requesting caller is required in order to create separate JSON entries.
            /// </summary>
            /// <param name="name">Unique name of the caller.</param>
            /// <param name="collect_callback">Callback for collecting all configurations.</param>
            /// <param name="apply_callback">callback for applying all configurations.</param>
            public void RegisterConfiguration(string name, RegisterCollect_Delegate collect_callback, RegisterApply_Delegate apply_callback)
            {
                if (!_initilized)
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
                savefile_dialog(configurations);
                return true;
            }

            /// <summary>
            /// Request loading of configurations from a JSON file.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public bool Load()
            {
                string configurations = openfile_dialog();
                var _deserialize_structure = Deserialize<Dictionary<string, string>>(configurations);

                foreach (var apply_callback in _deserialize_structure)
                {
                    if (_apply_callbacks.ContainsKey(apply_callback.Key))
                    {
                        // Call callback for applying configurations
                        _apply_callbacks[apply_callback.Key](apply_callback.Value);
                    }
                }
                return false;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// File dialog for saving provided string to a JSON file.
            /// </summary>
            /// <param name="output_content">The content of the file as string.</param>
            private void savefile_dialog(string output_content)
            {
                Stream ouput_stream;
                System.Windows.Forms.SaveFileDialog save_file_dialog = new System.Windows.Forms.SaveFileDialog();

                save_file_dialog.Title = "Save Configuration";
                save_file_dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                save_file_dialog.Filter = "JSON files (*.json)|*.json";
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;
                save_file_dialog.AddExtension = true;
                save_file_dialog.FileName = WorkingDirectory.FileName("configuration", "json");

                if (save_file_dialog.ShowDialog() == DialogResult.OK)
                {
                    if ((ouput_stream = save_file_dialog.OpenFile()) != null)
                    {
                        byte[] byte_content = Encoding.ASCII.GetBytes(output_content);
                        MemoryStream stream = new MemoryStream(byte_content);
                        ouput_stream.Write(byte_content, 0, byte_content.Length);

                        ouput_stream.Close();
                    }
                }
            }

            /// <summary>
            /// File dialog for loading content from a JSON file.
            /// </summary>
            /// <returns>The content of the file as string.</returns>
            private string openfile_dialog()
            {
                var input_content = string.Empty;
                var input_filename = string.Empty;

                using (System.Windows.Forms.OpenFileDialog open_file_dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    open_file_dialog.Title = "Load Configuration";
                    open_file_dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                    open_file_dialog.Filter = "JSON files (*.json)|*.json";
                    open_file_dialog.FilterIndex = 1;
                    open_file_dialog.RestoreDirectory = true;
                    open_file_dialog.FileName = WorkingDirectory.FileName("configuration", "json");

                    if (open_file_dialog.ShowDialog() == DialogResult.OK)
                    {
                        input_filename = open_file_dialog.FileName;

                        var fileStream = open_file_dialog.OpenFile();
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            input_content = reader.ReadToEnd();
                        }
                    }
                }
                return input_content;
            }

            /* ------------------------------------------------------------------*/
            // public variables


            private Dictionary<string, RegisterCollect_Delegate> _collect_callbacks = null;
            private Dictionary<string, RegisterApply_Delegate> _apply_callbacks = null;
        }
    }
}