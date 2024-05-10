using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GrasshopperInterface.Utilities;
using Frontend.Application;
using Core.Utilities;
using Core.Data;
using Core.Abstracts;



/*
 * VisuAlFroG Grasshopper Component
 * 
 */

namespace GrasshopperInterface
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
          : base("Visual Analytics Framework for Grasshopper",
                "VisuAlFroG",
                "Visual analytics framework providing the concept of visual analytics pipeline within grasshopper.",
                "Visual Analytics",
                "Frameworks")
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
            pManager[0].Optional = true;

            pManager.AddGenericParameter("Command Line Arguments", "Arguments", "Provide command line arguments as text.", GH_ParamAccess.item);
            pManager[1].Optional = true;
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
        /// <param name="DataAccess"> The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DataAccess)
        {
            _timer.Start();

            // Window -----------------------------------------------------

            // Lazy init required
            if (_window == null)
            {
                string app_name = base.Name + " (" + base.NickName + ")";
                _window = new MainWindow(app_name);
                _window.SetOutputDataCallback(retrieve_output_data);
            }
            // Open or restore invisible window
            _window.Show();


            // Parse and evaluate command line arguments provided as text input
            string arguments = "";
            if (DataAccess.GetData<string>(1, ref arguments))
            {
                if (_arguments != arguments)
                {
                    _window.Arguments(arguments);
                    _arguments = arguments;
                }
            }


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
                if (!DataAccess.GetDataTree(0, out GH_Structure<IGH_Goo> input_data))
                {
                    _runtimemessages.Add(Log.Level.Error, "Unable to read input data");
                    return;
                }
                if (!input_data.IsEmpty)
                {
                    _runtimemessages.Add(Log.Level.Debug, "Data Count: " + input_data.DataCount.ToString() + " | Type: " + input_data.GetType().FullName);
                    /// DEBUG Log.Default.Msg(Log.Level.Warn, input_data.DataDescription(true, true)); // -> Same as Grasshopper Panel output

                    // Convert and pass on input data 
                    var input_data_converted = DataConverter.ConvertFromGHStructure(input_data);
                    _window.UpdateInputData(input_data_converted);

                }
                else
                {
                    _runtimemessages.Add(Log.Level.Info, "Skipping empty input data");
                }
            }


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
        /// Callback for retrieving new output data.
        /// </summary>
        /// <param name="ouput_data">Reference to the new output data.</param>
        public void retrieve_output_data(GenericDataStructure ouput_data)
        {
            _output_data = DataConverter.ConvertToGHStructure(ouput_data);
            ExpireSolution(true);
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private MainWindow _window = null;
        private GH_Structure<IGH_Goo> _output_data = null;
        private RuntimeMessages _runtimemessages = null;
        private string _arguments = "";

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
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("GrasshopperInterface.resources.logo.logo24.png");
                return new System.Drawing.Bitmap(stream);

            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f1a3ed54-b664-439c-929f-a239c3f668cd");
    }
}
