using System;
using Python.Runtime;
using System.IO;
using Core.Utilities;



/*
 * Python Script
 * 
 */
namespace Visualizations
{
    namespace PythonInterface
    {
        public class PythonScript
        {

            /* ------------------------------------------------------------------*/
            // public property

            public string _StringID { get; set; }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Load script from file (DEBUG).
            /// </summary>
            /// <returns></returns>
            public bool Initialize()
            {
                try
                {
                    Uri template_uri = WorkingDirectory.GetResourcePath(WorkingDirectory.Locations.Visualizations, "bokeh-template.py");
                    _source = File.ReadAllText(template_uri.ToString());
                    if (!string.IsNullOrEmpty(_source))
                    {
                        _initialized = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Default.Msg(Log.Level.Error, e.Message);
                }
                return _initialized;
            }

            /// <summary>
            /// Execute script.
            /// </summary>
            public void Execute()
            {
                // Global Interpreter Lock 
                using (Py.GIL())
                {
                    // Create a Python scope
                    using (PyModule scope = Py.CreateScope())
                    {
                        /// DEBUG
                        // convert object to a PyObject
                        // PyObject pyPerson = person.ToPython();
                        // create a Python variable "person"
                        // scope.Set("person", pyPerson);
                        // the person object may now be used in Python

                        scope.Exec(_source);

                        /// DEBUG Get 'Start' function as PyObject 
                        dynamic start = scope.Get("Start");
                        start();
                    }
                }

                /// TODO Runs in parallel thread which is currently not supported by Log.Default.Msg
                Console.WriteLine("PythonScript>>> StringID = " + _StringID);
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private bool _initialized = false;
            private string _source = null;
        }
    }
}
