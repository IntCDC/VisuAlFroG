using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Python.Runtime;
using Microsoft.Scripting.Runtime;
using Core.Management;



/*
 * Python.NET API
 * 
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

                if (_script.Initialize())
                {
                    _script._StringID = DateTime.Now.ToLongTimeString();
                    _worker = new Thread(_script.Execute);
                    _worker.Name = "PythonScript";

                    _initilized = true;
                }

                _timer.Stop(this.GetType().FullName);
                return _initilized;
            }


            public override bool Execute()
            {
                if (!_initilized)
                {
                    return false;
                }
                _timer.Start();

                _worker.Start();

                _timer.Stop(this.GetType().FullName);
                return true;
            }


            public override bool Terminate()
            {
                _worker.Join();
                PythonEngine.EndAllowThreads(_python_threads);
                PythonEngine.Shutdown();

                _initilized = false;
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private PythonScript _script = new PythonScript();
            private Thread _worker;
            private IntPtr _python_threads;

        }
    }
}
