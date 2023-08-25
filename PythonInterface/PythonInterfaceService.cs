using System;
using System.Threading;
using Python.Runtime;
using Core.Utilities;
using Core.Abstracts;
using Microsoft.Scripting.Runtime;



/*
 * Python.NET API
 * 
 */
/* TEST
Execute:

    _worker.Start();
*/
namespace Visualizations
{
    namespace PythonInterface
    {

        public class PythonInterfaceService : AbstractService
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

                bool initilized = false;

                //Setup Python Environment
                /// TODO Get path to Python DLL dynamically
                string environment_path = @"C:\ProgramData\Anaconda3";
                string python_dll = "python38.dll";
                string python_paths = $"{environment_path}\\Lib\\site-packages;{environment_path}\\Lib;{environment_path}\\DLLs";

                if (string.IsNullOrEmpty(Runtime.PythonDLL))
                {
                    Runtime.PythonDLL = $"{environment_path}\\{python_dll}";
                }
                PythonEngine.PythonHome = environment_path;
                PythonEngine.PythonPath = python_paths; // Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);
                PythonEngine.Initialize();
                _python_threads = PythonEngine.BeginAllowThreads();

                _script = new PythonScript();
                if (_script.Initialize())
                {
                    _script._StringID = DateTime.Now.ToLongTimeString();
                    _worker = new Thread(_script.Execute);
                    _worker.Name = "PythonScript";

                    initilized = true;
                }

                _timer.Stop();
                _initilized = initilized;
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
                    _worker.Join();
                    _worker = null;

                    PythonEngine.EndAllowThreads(_python_threads);
                    PythonEngine.Shutdown();

                    _script = null;

                    _initilized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Thread _worker = null;
            private IntPtr _python_threads;

            /// DEBUG
            private PythonScript _script = null;
        }
    }
}
