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
                    _window.OutputDataCallback(output_data);
                }
                // Open or restore invisible window
                _window.Show();


                string gh_document_filepath = OnPingDocument().FilePath;
                /// TODO Callback: OnPingDocument().FilePathChanged += this.FilePathChangedEvent


                // Access all input paramters
                foreach (var input_param in Params.Input)
                {
                    _runtimemessages.Add(Log.Level.Warn, "Input Paramter Name: " + input_param.Name + " | Type: " + input_param.Type.ToString());
                }

                // Read input paramter
                var input_data = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo>();
                if (!DA.GetDataTree(0, out input_data))
                {
                    _runtimemessages.Add(Log.Level.Error, "Missing input data");
                    return;
                }
                _runtimemessages.Add(Log.Level.Info, "Data Count: " + input_data.DataCount.ToString() + " | Type: " + input_data.GetType().FullName);

/*
                int i = 0;
                foreach (var branch in input_data.Branches)
                {
                    Log.Default.Msg(Log.Level.Error, i.ToString() + " Branch Type: " + branch.GetType().FullName);

                    foreach (var leaf in branch)
                    {
                        var type = leaf.GetType();
                        //Log.Default.Msg(Log.Level.Error, i.ToString() + " Leaf Type: " + type.FullName);

                        string data_s;
                        if (leaf.CastTo<string>(out data_s))
                        {
                            //Log.Default.Msg(Log.Level.Warn, i.ToString() + " Data String: " + data_s);
                        }
                        else {
                            Log.Default.Msg(Log.Level.Error, i.ToString() + " Data String: " + data_s);
                        }

                        i++;
                    }
                }
*/
                //input_data.Flatten();
                var flatten_data = input_data.FlattenData();
                _runtimemessages.Add(Log.Level.Info, " Flatten Type: " + flatten_data.GetType().FullName + " - Count: " + flatten_data.Count().ToString());
                


                /// TODO
                /// Convert data
                _window.InputData();









                // DEBUG
                _exec_count++;
                _runtimemessages.Add(Log.Level.Info, "<DEBUG> Executions: " + _exec_count);

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

            public void output_data()
            {


                /// TODO


            }


            /* ------------------------------------------------------------------*/
            // private variables

            private MainWindow _window = null;
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
