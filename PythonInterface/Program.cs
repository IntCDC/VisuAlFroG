using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.IO;

using System.Threading;
using Python.Runtime;
using Microsoft.Scripting.Runtime;


/*
 * Python.NET API
 * 
 */
namespace PythonInterface { 

    public class Script
    {
        public string ID { get; set; }

        /*

         from bokeh.layouts import column
        from bokeh.models import ColumnDataSource, CustomJS, Slider
        from bokeh.plotting import Figure, output_file, show

        output_file(""js_on_change.html"")

        x = [x*0.005 for x in range(0, 200)]
        y = x

        source = ColumnDataSource(data=dict(x=x, y=y))

        plot = Figure(width=400, height=400)
        plot.line('x', 'y', source=source, line_width=3, line_alpha=0.6)

        callback = CustomJS(args=dict(source=source), code=""""""
            console.log('JS handler called ...');
            const data = source.data;
            const f = cb_obj.value
            const x = data['x']
            const y = data['y']
            for (let i = 0; i < x.length; i++) {
                y[i] = Math.pow(x[i], f)
            }
            source.change.emit();
        """""")

        slider = Slider(start=0.1, end=4, value=1, step=.1, title=""power"")
        slider.js_on_change('value', callback)

        layout = column(slider, plot)

        show(layout)

         */


        private string _source = @"#
#-----------------------------------------------------------------

### Required to prevent error message 'Python.Runtime.PythonException: 'unknown locale: en-US'' when starting VisFrog via Grasshopper
import locale
locale.setlocale(locale.LC_ALL, ""C"")

import clr
clr.AddReference('PythonInterface')
from PythonInterface import PythonCallBack

def Start():
    PythonCallBack.Call('hello')

import sys
print(""Python Version: "" + sys.version)

#from bokeh.plotting import figure, output_file, show 
#graph = figure(title = 'Bokeh Line Graph') 
#x = [1, 2, 3, 4, 5] 
#y = [5, 4, 3, 2, 1] 
#graph.line(x, y) 
#show(graph)

import os
from bokeh.models import ColumnDataSource, Patches, CustomJS
from bokeh.plotting import figure, show
from bokeh.layouts import row
from bokeh.io import output_file, curdoc
import pandas as pd

output_file(""js_on_change.html"")

x = [[1,2,4], [3,5,6], [7,9,7], [5,7,6]]
y = [[4,2,1], [6,5,8], [3,9,6], [2,2,1]]
group = ['A', 'A', 'B', 'B']
id = [0,1,2,3]

df = pd.DataFrame(data=dict(x=x, y=y, group=group, id=id))
source = ColumnDataSource(df)

p = figure(tools=""tap"")

renderer = p.patches('x', 'y', source=source)

def my_tap_handler(attr,old,new):
    print(""Python Handler Called ..."")
    indices = source.selected.indices
    if len(indices) == 1:
        group = source.data[""group""][indices[0]]
        new_indices = [i for i, g in enumerate(source.data[""group""]) if g == group]
        if new_indices != indices:
            source.selected = Selection(indices=new_indices)


callback = CustomJS(args=dict(source=source), code=""""""
    console.log('JavaScript Handler Called ...');
    const data = source.data;
    const idcs = cb_obj.indices;
    console.log(typeof idcs);
    if (idcs.length == 1) {
        const group = source.data[""group""][idcs[0]];
        var new_indices = [];
        for (const [index, element] of source.data[""group""].entries()) {
            if (element == group) {
                new_indices.push(index);
            }
        }
        if (new_indices != idcs) {
            console.log(new_indices);
            source.selected.indices = new_indices;
        }
    }

    source.change.emit();
"""""")

selected_patches = Patches(fill_color=""#a6cee3"")
renderer.selection_glyph = selected_patches
source.selected.js_on_change('indices', callback)

print(""Show Plot ..."")

show(p, width=800)

#-----------------------------------------------------------------
";

        public Script()
        {
            //Setup Python Environment
            Runtime.PythonDLL = @"C:\ProgramData\Anaconda3\python38.dll";
            var pathToVirtualEnv = @"C:\ProgramData\Anaconda3";
            Console.WriteLine(pathToVirtualEnv);

            //var path = Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
            //path = string.IsNullOrEmpty(path) ? pathToVirtualEnv : path + ";" + pathToVirtualEnv;
            //Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("PATH", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("PYTHONPATH", python_path, EnvironmentVariableTarget.Process);
            string python_path = $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib;{pathToVirtualEnv}\\DLLs";

            PythonEngine.PythonHome = pathToVirtualEnv;
            PythonEngine.PythonPath = python_path; // Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

            Console.WriteLine(Runtime.PythonDLL);
            Console.WriteLine(PythonEngine.Platform);
            Console.WriteLine(PythonEngine.MinSupportedVersion);
            Console.WriteLine(PythonEngine.MaxSupportedVersion);
            Console.WriteLine(PythonEngine.BuildInfo);
            Console.WriteLine(PythonEngine.PythonPath);

            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();

            /*
            Runtime.PythonDLL = @"C:\Users\<username>\AppData\Local\Programs\Python\Python310\python310.dll";
            var pathToVirtualEnv = @"path\to\env";

            // be sure not to overwrite your existing "PATH" environmental variable.
            var path = Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
            path = string.IsNullOrEmpty(path) ? pathToVirtualEnv : path + ";" + pathToVirtualEnv;
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PATH", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            // Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib", EnvironmentVariableTarget.Process);

            PythonEngine.Initialize();

            PythonEngine.PythonHome = pathToVirtualEnv;
            PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);



            string pathToVirtualEnv = "/path/to/venv/";

            Environment.SetEnvironmentVariable("PATH", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib", EnvironmentVariableTarget.Process);


            PythonEngine.PythonHome = pathToVirtualEnv;
            PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);
            */


            //var searchPaths = this._engine.GetSearchPaths();
            //foreach (var s in searchPaths)
            //    Console.WriteLine(s);

            //ICollection<string> searchPaths = new List<string>();

            /*
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result
            // This will get the current PROJECT bin directory (ie ../bin/)
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName
            */
            /*
            string workingDirectory = Environment.CurrentDirectory;
            string basedir = Directory.GetParent(workingDirectory).Parent.FullName; // AppDomain.CurrentDomain.BaseDirectory;
            string libpath = Path.Combine(basedir, "lib");
            //searchPaths.Add(libpath);
            //Console.WriteLine(libpath);



            //searchPaths.Add(@"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Anaconda3_64\Lib");
            //searchPaths.Add(@"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Anaconda3_64\Lib\site-packages");
            searchPaths.Add(@"C:\Program Files\Python34\Lib");
            searchPaths.Add(@"C:\Program Files\Python34\Lib\site-packages");
            this._engine.SetSearchPaths(searchPaths);
            */

        }

        public void Execute()
        {

            Console.WriteLine("Execute Start ...");

            using (Py.GIL())
            {

                Console.WriteLine("Execute GIL ...");

                //dynamic clr = Py.Import("clr"); 
                //dynamic bp = Py.Import("bokeh.plotting");

                // create a Python scope
                using (PyModule scope = Py.CreateScope())
                {

                    Console.WriteLine("Execute Scope ...");

                    // convert the Person object to a PyObject
                    //PyObject pyPerson = person.ToPython();
                    // create a Python variable "person"
                    //scope.Set("person", pyPerson);

                    // the person object may now be used in Python

                    scope.Exec(this._source);
                    dynamic start = scope.Get("Start");
                    start();
                }
            }
            Console.WriteLine(this.ID);

            /*
            this._script.Execute(this._scope);
            dynamic start = this._scope.GetVariable("Start");
            start();
            */
        }


    }

    public static class PythonCallBack
    {

        static PythonCallBack()
        {

        }

        public static void Call(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class Program
    {
        public void Main()
        {

            Script _script = new Script();
            _script.ID = "313";

            Thread _worker;
            _worker = new Thread(_script.Execute);
            _worker.Name = "Test";
            _worker.Start();


            //_script.Execute();


            Console.WriteLine("The end ...");
            Console.ReadLine();
            _worker.Join();

            Console.WriteLine("Press Enter to quit.");
            Console.ReadLine();
        }
    }
}
