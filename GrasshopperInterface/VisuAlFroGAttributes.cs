using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GrasshopperInterface.Utilities;
using Frontend.Application;
using Core.Utilities;
using Core.Data;
using Core.Abstracts;
using GH_IO.Serialization;
using Grasshopper.GUI.Canvas;
using System.Threading;
using System.Globalization;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI;



/*
 * VisuAlFroG Grasshopper Component Attributes
 * 
 */
namespace GrasshopperInterface
{
    public class VisuAlFroGAttributes : GH_ComponentAttributes
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public VisuAlFroGAttributes(VisuAlFroG component_owner) : base(component_owner)
        {
            _component_owner = component_owner;
        }

        /// <summary>
        /// Open WPF window on double-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            _component_owner.CreateWindow();
            return GH_ObjectResponse.Handled;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        private VisuAlFroG _component_owner = null;

        #endregion
    }
}
