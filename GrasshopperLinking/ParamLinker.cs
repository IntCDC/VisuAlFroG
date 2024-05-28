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



/*
 * Parameter linking
 * 
* * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum***
 * 
 */
namespace GrasshopperLinking
{
    public class ParamLinker<LinkedParamType> : AbstractParamLinker
        where LinkedParamType : IGH_Param
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public ParamLinker(ParamType param_type, string caption) : base(param_type, caption) { }

        public override bool AttachNewInteraction(GH_Canvas canvas, GH_CanvasMouseEvent canvas_mouse_event, LinkingComponent owner)
        {
            _interaction = new ParamLinkerInteraction<LinkedParamType>(canvas, canvas_mouse_event, owner, _GuidValueMap, _last_grip);
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
