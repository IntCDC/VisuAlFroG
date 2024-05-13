﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



/*
 * Helper functions for loading and saving files via file dialog
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class FileDialogHelper
        {


            /// <summary>
            /// File dialog for saving provided string to a file.
            /// </summary>
            /// <param name="output_content">The content of the file as string.</param>
            public static void Save(string output_content, string dialog_title, string dialog_filter, string default_file_name)
            {
                Stream ouput_stream;

                System.Windows.Forms.SaveFileDialog save_file_dialog = new System.Windows.Forms.SaveFileDialog();
                save_file_dialog.Title = dialog_title;
                save_file_dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                save_file_dialog.Filter = dialog_filter;
                save_file_dialog.FilterIndex = 1;
                save_file_dialog.RestoreDirectory = true;
                save_file_dialog.AddExtension = true;
                save_file_dialog.FileName = default_file_name;

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
            public static string Load(string dialog_title, string dialog_filter, string default_file_name)
            {
                var input_content = "";
                using (System.Windows.Forms.OpenFileDialog open_file_dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    open_file_dialog.Title = dialog_title;
                    open_file_dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                    open_file_dialog.Filter = dialog_filter;
                    open_file_dialog.FilterIndex = 1;
                    open_file_dialog.RestoreDirectory = true;
                    open_file_dialog.FileName = default_file_name;

                    if (open_file_dialog.ShowDialog() == DialogResult.OK)
                    {
                        return open_file_dialog.FileName;
                    }
                }
                return input_content;
            }



        }
    }
}
