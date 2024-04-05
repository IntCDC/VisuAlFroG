using System;

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

            /// <summary>
            /// [STATIC] Callback called from within Python script to print message (DEBUG).
            /// </summary>
            /// <param name="message"></param>
            public static void PrintMessage(string message)
            {
                /// TODO Runs in parallel thread which is currently not supported by Log.Default.Msg
                Console.WriteLine("PythonCallback>>> Message = " + message);
            }

            /// <summary>
            /// [STATIC] Callback called from within Python script to get output file for Bokeh (DEBUG).
            /// </summary>
            /// <returns></returns>
            public static string GetOutputFile()
            {
                return ResourcePaths.CreateFilePath("bokeh", "html");
            }

        }
    }
}
