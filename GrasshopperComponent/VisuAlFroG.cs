using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using GrasshopperComponent.Utilities;
using Frontend.Application;
using Core.Utilities;
using System.Resources;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Visualizations.Data;



/*
 * VisuAlFroG Grasshopper Component
 * 
 */
namespace Interface
{
    namespace GrasshopperComponent
    {
        public class VisuAlFroG : GH_Component
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
            public VisuAlFroG()
              : base("Visual Analytics Framework for Grasshopper", "VisuAlFroG",
                "Visual analytics framework providing the concept of visual analytics pipeline within grasshopper.",
                "Visual Analytics", "VisuAlFroG")
            {
                _runtimemessages = new RuntimeMessages(this);
                _timer = new TimeBenchmark();
                _exec_count = 0;
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
            protected override void SolveInstance(IGH_DataAccess DataAccess)
            {
                _timer.Start();

                // Window -----------------------------------------------------

                // Lazy init required!
                if (_window == null)
                {
                    string app_name = base.Name + " (" + base.NickName + ")";
                    _window = new MainWindow(app_name);
                    _window.SetReloadInterface(reload_instance);
                    _window.SetOutputDataCallback(retrieve_output_data);
                }
                // Open or restore invisible window
                _window.Show();


                // Data -------------------------------------------------------

                if (_output_data != null)
                {
                    // Write output data
                    DataAccess.SetDataTree(0, _output_data);
                    _output_data = null;
                }
                else
                {
                    // Read input data
                    var input_data = new GH_Structure<IGH_Goo>();
                    if (!DataAccess.GetDataTree(0, out input_data))
                    {
                        _runtimemessages.Add(Log.Level.Error, "Unable to read input data");
                        return;
                    }
                    _runtimemessages.Add(Log.Level.Debug, "Data Count: " + input_data.DataCount.ToString() + " | Type: " + input_data.GetType().FullName);
                     /// DEBUG Log.Default.Msg(Log.Level.Warn, input_data.DataDescription(true, true)); // -> Same as Grasshopper Panel output

                    if (!input_data.IsEmpty) {
                        // Convert and pass on input data 
                        var input_data_converted = DataConverter.ConvertFromGHStructure(ref input_data);
                        _window.UpdateInputData(ref input_data_converted);
                    } else 
                    {
                        _runtimemessages.Add(Log.Level.Info, "Skipping empty input data");
                    }
                }


                // Misc -------------------------------------------------------

                /*
                // Provide wrappers for DataAccess.S/GetDataTree, DataAccess.S/GetDataList, and DataAccess.S/GetData
                // Access all input parameters
                foreach (var input_param in Params.Input)
                {
                    _runtimemessages.Add(Log.Level.Warn, "Input Parameter Name: " + input_param.Name + " | Type: " + input_param.Type.ToString());
                    Tuple<DataType, Get-Delegate>
                }
                // Access all output parameters
                foreach (var output_param in Params.Output)
                {
                    _runtimemessages.Add(Log.Level.Warn, "Input Parameter Name: " + output_param.Name + " | Type: " + output_param.Type.ToString());
                    Tuple<DataType, Set-Delegate>
                }

                // string gh_document_filepath = OnPingDocument().FilePath;
                // OnPingDocument().FilePathChanged += this.FilePathChangedEvent
                */


                // Log --------------------------------------------------------

                _timer.Stop();

                // DEBUG
                _exec_count++;
                _runtimemessages.Add(Log.Level.Info, "Solution execution number: " + _exec_count);

                // Show runtime messages in Grasshopper
                _runtimemessages.Show();
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Callback to trigger reloading of the Grasshopper solution.
            /// </summary>
            public void reload_instance()
            {
                ExpireSolution(true);
            }

            /// <summary>
            /// Callback for retrieving new output data.
            /// </summary>
            /// <param name="ouput_data">Reference to the new output data.</param>
            public void retrieve_output_data(ref GenericDataStructure ouput_data)
            {
                _output_data = DataConverter.ConvertToGHStructure(ref ouput_data);
                reload_instance();
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private MainWindow _window = null;
            private GH_Structure<IGH_Goo> _output_data = null;
            private RuntimeMessages _runtimemessages = null;

            /// DEBUG
            private int _exec_count = 0;
            private TimeBenchmark _timer = null;


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