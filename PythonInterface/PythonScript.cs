using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // public functions

            public bool Initialize()
            {
                /// DEBUG Load script from file
                try
                {
                    _source = File.ReadAllText(@"bokeh-template.py");
                    if (!string.IsNullOrEmpty(_source))
                    {
                        _initilized = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Default.Msg(Log.Level.Error, e.Message);
                }
                return _initilized;
            }


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
                        //PyObject pyPerson = person.ToPython();
                        // create a Python variable "person"
                        //scope.Set("person", pyPerson);
                        // the person object may now be used in Python

                        scope.Exec(_source);

                        /// DEBUG Get 'Start' fuction as PyObject 
                        dynamic start = scope.Get("Start");
                        start();
                    }
                }
                Log.Default.Msg(Log.Level.Debug, "PythonScript>>> StringID = " + _StringID);

            }


            /* ------------------------------------------------------------------*/
            // public variables

            public string _StringID { get; set; }


            /* ------------------------------------------------------------------*/
            // private variables

            private bool _initilized = false;
            private string _source = "";

        }

    }
}
