using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;
using System.Diagnostics;
using Grasshopper;
using Grasshopper.Kernel;



/*
 * Grasshopper Runtime Messages
 * 
 */
namespace GrasshopperComponent
{
    namespace Utilities
    {
        public class RuntimeMessages
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public RuntimeMessages(GH_Component ghcomponent)
            {
                _parent = ghcomponent;
            }


            public void Add(Log.Level level, string message)
            {
                switch (level)
                {
                    case (Log.Level.Error):
                        _messages.Add((GH_RuntimeMessageLevel.Error, message));
                        break;
                    case (Log.Level.Warn):
                        _messages.Add((GH_RuntimeMessageLevel.Warning, message));
                        break;
                    case (Log.Level.Info):
                        _messages.Add((GH_RuntimeMessageLevel.Remark, message));
                        break;
                    case (Log.Level.Debug):
                        _messages.Add((GH_RuntimeMessageLevel.Blank, message));
                        break;
                }
                Log.Default.Msg(level, message, new StackTrace(true));
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
}
