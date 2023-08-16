﻿using System;
using Core.Utilities;



/*
 * Python Callback
 * 
 */
namespace Visualizations
{
    namespace PythonInterface
    {
        public static class PythonCallback
        {

            /* ------------------------------------------------------------------*/
            // static functions

            public static void PrintMessage(string message)
            {
                /// Runs in parallel thread which is currently not supported by Log.Default.Msg
                Console.WriteLine("PythonCallback>>> Message = " + message);
            }

            public static string GetBokehOutputFilePath()
            {
                return Artefacts.FilePath("bokeh", "html");
            }

        }
    }
}