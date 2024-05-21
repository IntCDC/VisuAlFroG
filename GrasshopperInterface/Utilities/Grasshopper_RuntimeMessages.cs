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
namespace GrasshopperInterface
{
    namespace Utilities
    {
        public class Grasshopper_RuntimeMessages
        {

            /* ------------------------------------------------------------------*/
#region public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="ghcomponent">The parent Grasshopper component to access the runtime message interface.</param>
            public Grasshopper_RuntimeMessages(GH_Component ghcomponent)
            {
                _parent = ghcomponent;
                _messages = new List<(GH_RuntimeMessageLevel, string)>();
            }

            /// <summary>
            /// Add new message to the Grasshopper specific log.
            /// </summary>
            /// <param name="level">The log level.</param>
            /// <param name="message">The actual log message.</param>
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
                /// TODO XXX StackTrace is not working for Grasshopper component
                Log.Default.Msg(level, message, new StackTrace(true));
            }

            /// <summary>
            /// Send all messages to the Grasshopper component.
            /// </summary>
            public void Show()
            {
                if (_parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing parent grasshopper component", new StackTrace(true));
                    return;
                }

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

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private GH_Component _parent = null;
            private List<(Grasshopper.Kernel.GH_RuntimeMessageLevel, string)> _messages = null;

            #endregion
        }
    }
}
