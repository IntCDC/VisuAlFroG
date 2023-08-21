using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using GrasshopperComponent.Utilities;
using Frontend.Application;
using Core.Utilities;
using System.Resources;



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
                pManager.AddGenericParameter("Generic Output Data", "Output Data", "Generic output data from interaction.", GH_ParamAccess.list);
                pManager.AddTextParameter("Debug Output", "Debug", "Output of debug information.", GH_ParamAccess.item);
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


                // Access input paramter
                foreach (var input_param in Params.Input)
                {
                    _runtimemessages.Add(MessageLevel.Warning, "Input Paramter Name: " + input_param.Name + " | Type: " + input_param.Type.ToString());
                }
                var input_data = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo>();
                if (!DA.GetDataTree(0, out input_data))
                {
                    _runtimemessages.Add(MessageLevel.Error, "Can not get input data");
                    return;
                }
                _runtimemessages.Add(MessageLevel.Info, "Data Count: " + input_data.DataCount.ToString() + " | Type: " + input_data.GetType().ToString());



                /// TODO
                _window.InputData();




                // Debug stuff
                _exec_count++;
                DA.SetData(1, "<DEBUG> Executions: " + _exec_count);

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
            private TimeBenchmark _timer = new TimeBenchmark();

            /// DEBUG
            private int _exec_count = 0;


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
            protected override System.Drawing.Bitmap Icon => new System.Drawing.Bitmap("resources/logo32.png");

            /// <summary>
            /// Each component must have a unique Guid to identify it. 
            /// It is vital this Guid doesn't change otherwise old ghx files 
            /// that use the old ID will partially fail during loading.
            /// </summary>
            public override Guid ComponentGuid => new Guid("f1a3ed54-b664-439c-929f-a239c3f668cd");
        }
    }
}
