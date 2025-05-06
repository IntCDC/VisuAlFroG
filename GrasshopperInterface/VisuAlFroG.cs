using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GrasshopperInterface.Utilities;
using Frontend.Application;
using Core.Utilities;
using Core.Data;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;


/*
 * VisuAlFroG Grasshopper component
 * 
 */
namespace GrasshopperInterface
{
    public class VisuAlFroG : GH_Component
    {
        /* ------------------------------------------------------------------*/
        #region public functions

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
            /// XXX Prevent multiple instances or is it OK?
            NewInstanceGuid();

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

            pManager.AddGenericParameter("Input Sample Files Path", "Samples Path", "Input absolut path of folder of sample file(s) as string.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Output Data", "Output", "Output of data.", GH_ParamAccess.list);
        }


        /// <summary>
        /// Creates custom attributes for the component.
        /// This method is overridden to provide a custom UI for the component.
        /// </summary>
        //public override void CreateAttributes()
        //{
            //m_attributes = new ComponentAttributes(this, _runtimemessages);
        //}


    
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DataAccess"> The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DataAccess)
        {
            _timer.Start();

            // Initialize generic_data
            GenericDataStructure generic_data = new GenericDataStructure();

            var atr = m_attributes as ComponentAttributes;
            //if (atr == null)
            //{
            //    _runtimemessages.Add(Log.Level.Info, "Unable to get output data from attribute.");
            //}
            //else
            //{
            //    List<Tuple<Guid, string, double>> values = atr.OutputValues();
            //    DataAccess.SetDataList(0, values);
            //}

            //if (_window == null)
            //{
            //    // Skip if window is not yet created
            //    _runtimemessages.Add(Log.Level.Warn, "Create window with double-click on component.");
            //}
            //else
            //{

            // Window -----------------------------------------------------

            CreateWindow();

            //// Parse and evaluate command line arguments provided as text input
            //string arguments = "";
            //if (DataAccess.GetData<string>(1, ref arguments))
            //{
            //    if (_arguments != arguments)
            //    {
            //        _window.Arguments(arguments);
            //        _arguments = arguments;
            //    }
            //}
                       
            // GH Structure Data ------------------------------------------

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

                    // Convert GH input data to GenericDataStructure
                    generic_data = DataConverter.ConvertFromGHStructure(input_data, generic_data);

                    // Update the window with the converted data
                    if (_window != null)
                        _window.UpdateInputData(generic_data);
                }
            }

            // Json Sample Files ------------------------------------------

            generic_data = GetSamples(DataAccess, generic_data);

            if (generic_data != null && _window != null)
            {
                // Update the window with the updated generic_data
                _window.UpdateInputData(generic_data);
            }

            // Log --------------------------------------------------------

            // DEBUG
            //_exec_count++;
            //_runtimemessages.Add(Log.Level.Info, "Solution execution number: " + _exec_count);

            _timer.Stop();

            // Show runtime messages in Grasshopper
            _runtimemessages.Show();
        }

        public void CreateWindow()
        {
            // Lazy init required
            if (_window == null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                _window = new MainWindow(true);
                _window.SetOutputDataCallback(retrieve_output_data);
            }
            else
            {
                // Open or restore invisible window
                _window.Show();
            }

            //ExpireSolution(true);
        }

        /// <summary>
        /// Retrieves and processes sample JSON files from a user-provided directory path input.
        /// </summary>
        /// <param name="DataAccess">The Grasshopper data access interface for retrieving input data.</param>
        private GenericDataStructure GetSamples(IGH_DataAccess DataAccess, GenericDataStructure generic_data)
        {
            string input_sample_path = "";

            if (!DataAccess.GetData(2, ref input_sample_path))
            {
                // If the input is missing or null, log an error and exit early.
                //_runtimemessages.Add(Log.Level.Info, "No input directory path provided.");
                return null;
            }

            // Normalize and clean the input directory path
            string dirPath = input_sample_path.ToString().Trim().Replace('/', '\\');

            if (string.IsNullOrWhiteSpace(dirPath) || !System.IO.Directory.Exists(dirPath))
            {
                _runtimemessages.Add(Log.Level.Error, $"Invalid directory path: {dirPath}");
                return null;
            }

            try
            {
                // Debug the directory path and files before loading them
                _runtimemessages.Add(Log.Level.Info, $"Loading JSON files from directory: {dirPath}");

                // Attempt to load JSON sample files from the directory using a helper utility.
                List<Dictionary<string, object>> json_samples = JsonSampleLoader.LoadJsonSamples(dirPath);

                // Log how many JSON files were successfully loaded
                _runtimemessages.Add(Log.Level.Info, $"Loaded {json_samples.Count} JSON file(s).");

                // Convert the loaded JSON samples to a generic data structure
                generic_data = DataConverter.ConvertJsonSamplesToGenericData(json_samples, generic_data);

                // Retrieve the list of metadata from the generic data structure
                List<GenericMetaData> metadata_list = generic_data.GetListMetaData();

                // Convert the metadata to readable strings for debugging or output
                List<string> metadataStrings = metadata_list.Select(metadata =>
                    $"Index: {metadata._Index}, Label: {metadata._Label}, Dimension: {metadata._Dimension}, Selected: {metadata._Selected}"
                ).ToList();

                // Combine the JSON sample data into readable strings for Grasshopper output
                List<string> jsonStrings = json_samples.Select(dict =>
                    string.Join(", ", dict.Select(kvp => $"{kvp.Key}: {kvp.Value}"))
                ).ToList();

                // Combine metadata and JSON strings into one list to be output
                List<string> outputStrings = metadataStrings.Concat(jsonStrings).ToList();

                ////// Convert dictionaries to readable strings for Grasshopper output
                //List<string> outputStrings = json_samples.Select(
                //    dict => string.Join(", ", dict.Select(kvp => $"{kvp.Key}: {kvp.Value}"))
                //).ToList();

                //// Output the loaded JSON samples to Grasshopper output parameter
                DataAccess.SetDataList(0, outputStrings);

                return generic_data;
            }
            catch (Exception ex)
            {
                _runtimemessages.Add(Log.Level.Error, $"Failed to load JSON files: {ex.Message}");
                _runtimemessages.Add(Log.Level.Error, $"Stack Trace: {ex.StackTrace}");
                // Return the original generic_data if error occurs
                return generic_data;
            }
        }


        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        /// <summary>
        /// Callback for retrieving new output data.
        /// </summary>
        /// <param name="ouput_data">Reference to the new output data.</param>
        public void retrieve_output_data(GenericDataStructure ouput_data)
        {
            _output_data = DataConverter.ConvertToGHStructure(ouput_data);
            ExpireSolution(true);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        private MainWindow _window = null; // set in ctor
        private GH_Structure<IGH_Goo> _output_data = null; // only keep pointer
        private RuntimeMessages _runtimemessages = null; // set in ctor
        private string _arguments = "";

        /// DEBUG
        private int _exec_count = 0;
        private TimeBenchmark _timer = null; // set in ctor

        #endregion

        /* ------------------------------------------------------------------*/
        #region internal grasshopper stuff

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

        #endregion
    }
}
