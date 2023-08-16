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
            _parent = ghcomponent;
        }


        public void Add(MessageLevel l, string m)
        {
            switch(l)
            {
                case (MessageLevel.Error):
                    _messages.Add((GH_RuntimeMessageLevel.Error, m));
                    break;
                case (MessageLevel.Warning):
                    _messages.Add((GH_RuntimeMessageLevel.Warning, m)); 
                    break;
                case (MessageLevel.Info):
                    _messages.Add((GH_RuntimeMessageLevel.Remark, m)); 
                    break;
            }
        }


        public void Show()
        {
            _parent.ClearRuntimeMessages();
            if (_messages.Count != 0)
            {
                foreach ((Grasshopper.Kernel.GH_RuntimeMessageLevel, string) m in _messages)
                {
                    _parent.AddRuntimeMessage(m.Item1, m.Item2);
                }
                _messages.Clear();
            }
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private GH_Component _parent;
        private List<(Grasshopper.Kernel.GH_RuntimeMessageLevel, string)> _messages = new List<(GH_RuntimeMessageLevel, string)>();
    }
}
