using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Abstracts;
using Core.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;



/*
 * Settings Service
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class SettingsService : AbstractService
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();




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



                    _initilized = false;
                }
                return true;
            }


            public bool Save<T>(T settings)
            {
                string content = serialize<T>(settings);
                if (content != string.Empty)
                {
                    savefile_dialog(content);
                    return true;
                }
                return false;
            }


            public T Load<T>()
            {
                T settings = default(T);
                string content = openfile_dialog();
                if (content != string.Empty)
                {
                    settings = deserialize<T>(content);
                }
                return settings;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private string serialize<T>(T window_data)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                return JsonSerializer.Serialize<T>(window_data, options);
            }


            private T deserialize<T>(string content)
            {
                return JsonSerializer.Deserialize<T>(content);
            }


            private void savefile_dialog(string output_content)
            {
                Stream ouput_stream;
                System.Windows.Forms.SaveFileDialog save_file_dialog = new System.Windows.Forms.SaveFileDialog();

                save_file_dialog.Title = "Save Window Settings";
                save_file_dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                save_file_dialog.Filter = "JSON files (*.json)|*.json";
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;
                save_file_dialog.AddExtension = true;
                save_file_dialog.FileName = "settings";

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


            private string openfile_dialog()
            {
                var input_content = string.Empty;
                var input_filename = string.Empty;

                using (System.Windows.Forms.OpenFileDialog open_file_dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    open_file_dialog.Title = "Load Window Settings";
                    open_file_dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                    open_file_dialog.Filter = "JSON files (*.json)|*.json";
                    open_file_dialog.FilterIndex = 1;
                    open_file_dialog.RestoreDirectory = true;
                    open_file_dialog.FileName = "settings";

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

        }
    }
}
