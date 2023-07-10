using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GrasshopperComponent.Utilities;
using VisFroG_WPF;


/*
 * VisFrog Grasshopper Component
 * 
 */
namespace GrasshopperComponent
{
    public class VisFroG : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public VisFroG()
          : base("VISualization FRamework fOr Grasshopper", "VisFroG",
            "Visualization framework providing the concept of visual analytics pipeline within grasshopper.",
            "Visual Analysis", "VisFroG")
        {
            runtimemessages = new RuntimeMessages(this);

            exec_count = 0;
            triggered_data_update = false;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Generic Input Data", "Input Data", "Generic input data for visualization.", GH_ParamAccess.tree);
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
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // Create new window if not created yet or closed previously
            if (window == null || !window.IsLoaded)
            {
                window = new VisFroG_WPF.MainWindow();
                window.ReloadComponentFunction(this.ReloadInstance);
            }
            // Open window
            window.Show();

            // Do not update 'some buffer' data if new data has been set by this component 
            if (triggered_data_update)
            {
                triggered_data_update = false;
                /* 
                if (!DA.GetData(1, ...))
                {
                    this.runtimemessages.Add(Utils.MessageLevel.Warning, "Unable to retrieve 'some data'.");
                    return;
                }
                */
            }


            foreach (var input_param in Params.Input)
            {
                this.runtimemessages.Add(Utilities.MessageLevel.Warning, "Input Paramter Name: " + input_param.Name + " | Type: " + input_param.TypeName);

            }

            var input_data = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo>();
            DA.GetDataTree(0, out input_data);

            this.runtimemessages.Add(Utilities.MessageLevel.Info, "Data Count: " + input_data.DataCount.ToString());

            // Show runtime messages
            runtimemessages.Show();


            exec_count++;
            DA.SetData(1, "<DEBUG> Executions: " + exec_count);
        }

        /* ------------------------------------------------------------------*/
        // local functions

        public void ReloadInstance()
        {
            try
            {
                /*
                SomeComponent sc = null;
                if (this.uniqueDepenedConnectedComponent<SomeComponent>(ref fbc))
                {
                    fbc.SetSomeBuffer();
                    triggered_data_update = true;
                    runtimemessages.Add(MessageLevel.Remark, "Found connected 'some buffer' and updated interaction.");
                }
                else
                {
                    runtimemessages.Add(MessageLevel.Error, "Missing connected 'some buffer' component for update of interaction.");
                }
                */
            }
            catch
            {
                runtimemessages.Add(MessageLevel.Error, "Missing Component.");
            }
            ExpireSolution(true);
        }

        /* ------------------------------------------------------------------*/
        // local functions


        private bool uniqueDepenedConnectedComponent<T>(ref T found_object)
        {

            foreach (var doc_obj in OnPingDocument().Objects)
            {
                try
                {
                    T component = (T)doc_obj;
                    if (component != null)
                    {
                        if (DependsOn((IGH_ActiveObject)doc_obj))
                        {
                            found_object = component;
                            return true;
                        }
                    }
                }
                catch (System.InvalidCastException)
                {
                    // runtimemessages.Add(MessageLevel.Error, "Cannot convert to: " + typeof(T).Name);
                }
            }
            return false;
        }


        /* ------------------------------------------------------------------*/
        // local variables

        private VisFroG_WPF.MainWindow window;
        private Utilities.RuntimeMessages runtimemessages;

        private int exec_count;
        private bool triggered_data_update;

        /* ------------------------------------------------------------------*/
        // Internal grasshopper stuff

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
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f1a3ed54-b664-439c-929f-a239c3f668cd");
    }
}