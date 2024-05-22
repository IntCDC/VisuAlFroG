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


/*
 * Abstract class for component linking
 * 
* * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum***
 * 
 */
namespace GrasshopperInterface
{
    public class gh_AbstractParamLinker : GH_AbstractInteraction
    {

        /* ------------------------------------------------------------------*/
        #region public functions

        public gh_AbstractParamLinker(GH_Canvas canvas, GH_CanvasMouseEvent canvas_mouse_event, GH_Component owner) : base(canvas, canvas_mouse_event)
        {



        }



        #endregion

        /* ------------------------------------------------------------------*/
        #region protected functions



        #endregion

        /* ------------------------------------------------------------------*/
        #region static functions

        private Dictionary<GH_Guid, double> _guid_value_map = new Dictionary<GH_Guid, double>();


        #endregion
    }
}
