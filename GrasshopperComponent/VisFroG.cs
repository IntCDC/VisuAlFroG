using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using GrasshopperComponent.Utilities;
using Frontend.Application;
using Core.Utilities;
using System.Resources;
using System.Linq;
using static GH_IO.VersionNumber;
using System.Collections.Generic;



using AbstractData_Type = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using GHData_Type = Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo>;


/*
 * VisFrog Grasshopper Component
 * 
 */
namespace Interface
{
    namespace GrasshopperComponent
    {
        public class VisFroG : GH_Component
        {
            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Each implementation of GH_Component must provide a public 
            /// constructor without any arguments.
            /// Category represents the Tab in which the component will appear, 
            /// Subcategory the panel. If you use non-existing tab or panel names, 
            /// new tabs/panels will automatically be created.
            /// </summary>
            public VisFroG()
              : base("Visualization Framework for Grasshopper", "VisFroG",
                "Visualization framework providing the concept of visual analytics pipeline within grasshopper.",
                "Visual Analysis", "VisFroG")
            {
                _runtimemessages = new RuntimeMessages(this);
            }


            /// <summary>
            /// Registers all the input parameters for this component.
            /// </summary>
            protected override void RegisterInputParams(GH_InputParamManager pManager)
            {
                pManager.AddGenericParameter("Generic Input Data", "Input Data", "Generic input data for visualization.", GH_ParamAccess.tree);
                /// DEBUG
                pManager[0].Optional = true;
            }


            /// <summary>
            /// Registers all the output parameters for this component.
            /// </summary>
            protected override void RegisterOutputParams(GH_OutputParamManager pManager)
            {
                pManager.AddGenericParameter("Generic Output Data", "Output Data", "Generic output data from interaction.", GH_ParamAccess.tree);
            }


            /// <summary>
            /// This is the method that actually does the work.
            /// </summary>
            /// <param name="DA"> The DA object can be used to retrieve data from input parameters and 
            /// to store data in output parameters.</param>
            protected override void SolveInstance(IGH_DataAccess DA)
            {
                _timer.Start();

                // Lazy init required!
                if (_window == null)
                {
                    string app_name = base.Name + " (" + base.NickName + ")";
                    _window = new MainWindow(app_name, true);
                    _window.ReloadInterfaceCallback(reload_instance);
                    _window.OutputDataCallback(retrieve_output_data);
                }
                // Open or restore invisible window
                _window.Show();



                // Read input data
                var input_data = new GHData_Type();
                if (!DA.GetDataTree(0, out input_data))
                {
                    _runtimemessages.Add(Log.Level.Error, "Missing input data");
                    return;
                }
                _runtimemessages.Add(Log.Level.Debug, "Data Count: " + input_data.DataCount.ToString() + " | Type: " + input_data.GetType().FullName);
                // Convert and pass on input data 
                var input_data_converted = ConvertData.GH_to_List(ref input_data);
                _window.InputData(ref input_data_converted);



                // Write output data
                /// TODO Do not read input data if output data triggers reloading?
                if (output_data != null)
                {
                    DA.SetDataTree(0, output_data);
                    output_data = null;
                }



                // DEBUG
                _exec_count++;
                _runtimemessages.Add(Log.Level.Debug, "Executions: " + _exec_count);

                // Show runtime messages in Grasshopper
                _runtimemessages.Show();

                _timer.Stop();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            public void reload_instance()
            {
                ExpireSolution(true);
            }


            public void retrieve_output_data(ref AbstractData_Type ouput_data)
            {
                output_data = ConvertData.list_to_gh(ref ouput_data);
                reload_instance();
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private MainWindow _window = null;
            private GHData_Type output_data = null;
            private RuntimeMessages _runtimemessages = null;

            /// DEBUG
            private int _exec_count = 0;
            private TimeBenchmark _timer = new TimeBenchmark();


            /* ------------------------------------------------------------------*/
            // internal grasshopper stuff

            /// <summary>
            /// The Exposure property controls where in the panel a component icon 
            /// will appear. There are seven possible locations (primary to septenary), 
            /// each of which can be combined with the GH_Exposure.obscure flag, which 
            /// ensures the component will only be visible on panel dropdowns.
            /// </summary>
            public override GH_Exposure Exposure => GH_Exposure.primary;

            /// <summary>
            /// Provides an Icon for every component that will be visible in the User Interface.
            /// Icons need to be 24x24 pixels.
            /// You can add image files to your project resources and access them like this:
            /// return Resources.IconForThisComponent;
            /// </summary>
            protected override System.Drawing.Bitmap Icon => new System.Drawing.Bitmap("resources/logo24.png");

            /// <summary>
            /// Each component must have a unique Guid to identify it. 
            /// It is vital this Guid doesn't change otherwise old ghx files 
            /// that use the old ID will partially fail during loading.
            /// </summary>
            public override Guid ComponentGuid => new Guid("f1a3ed54-b664-439c-929f-a239c3f668cd");
        }
    }
}
