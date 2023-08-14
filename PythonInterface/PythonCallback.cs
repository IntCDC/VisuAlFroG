using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                Log.Default.Msg(Log.Level.Debug, "PythonCallback>>> Message = " + message);
            }

            public static string GetBokehOutputFilePath()
            {
                return Artefacts.FilePath("bokeh", "html");
            }

        }
    }
}
