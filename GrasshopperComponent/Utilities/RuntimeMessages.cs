using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;



/*
 * Grasshopper Runtime Messages
 * 
 */
namespace GrasshopperComponent.Utilities
{

    public enum MessageLevel
    {
        Error,
        Warning,
        Info
    }


    public class RuntimeMessages
    {

        /* ------------------------------------------------------------------*/
        // public functions
        public RuntimeMessages(GH_Component ghcomponent)
        {
            this.parent = ghcomponent;
            this.messages = new List<(GH_RuntimeMessageLevel, string)>();
        }


        public void Add(MessageLevel l, string m)
        {
            switch(l)
            {
                case (MessageLevel.Error):
                    this.messages.Add((GH_RuntimeMessageLevel.Error, m));
                    break;
                case (MessageLevel.Warning):
                    this.messages.Add((GH_RuntimeMessageLevel.Warning, m)); 
                    break;
                case (MessageLevel.Info):
                    this.messages.Add((GH_RuntimeMessageLevel.Remark, m)); 
                    break;
            }
        }


        public void Show()
        {
            this.parent.ClearRuntimeMessages();
            if (this.messages.Count != 0)
            {
                foreach ((Grasshopper.Kernel.GH_RuntimeMessageLevel, string) m in this.messages)
                {
                    this.parent.AddRuntimeMessage(m.Item1, m.Item2);
                }
                this.messages.Clear();
            }
        }


        /* ------------------------------------------------------------------*/
        // local variables

        private GH_Component parent;
        private List<(Grasshopper.Kernel.GH_RuntimeMessageLevel, string)> messages;

        /* ------------------------------------------------------------------*/
        // local functions


    }
}
