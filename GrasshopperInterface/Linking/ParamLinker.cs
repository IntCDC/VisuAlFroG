using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Drawing;
using System.Windows.Forms;
using Rhino.Resources;
using Grasshopper.Kernel.Special;
using GrasshopperInterface.Utilities;



/*
 * Parameter linking
 * 
 * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum ***
 * 
 */
namespace GrasshopperInterface
{
    public class ParamLinker<LinkedParamType> : AbstractParamLinker
        where LinkedParamType : IGH_Param
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public ParamLinker(ParamType param_type, string caption, RuntimeMessages runtimemessages) : base(param_type, caption, runtimemessages) { }

        public override bool AttachNewInteraction(GH_Canvas canvas, GH_CanvasMouseEvent canvas_mouse_event, VisuAlFroG owner)
        {
            _interaction = new ParamLinkerInteraction<LinkedParamType>(canvas, canvas_mouse_event, owner, _GuidValueMap, _last_grip, _runtimemessages);
            canvas.ActiveInteraction = _interaction;
            return true;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        ParamLinkerInteraction<LinkedParamType> _interaction = null;

        #endregion
    }
}
