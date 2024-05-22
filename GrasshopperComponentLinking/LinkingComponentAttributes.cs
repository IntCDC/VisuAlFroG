using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Core.Utilities;
using Core.Data;
using Core.Abstracts;
using GH_IO.Serialization;
using Grasshopper.GUI.Canvas;
using System.Threading;
using System.Globalization;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI;
using Grasshopper;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;



/*
 * Grasshopper linking component attributes
 * 
 */
namespace GrasshopperLinking
{
    public class LinkingComponentGAttributes : GH_ComponentAttributes
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public LinkingComponentGAttributes(LinkingComponent component_owner) : base(component_owner)
        {
            _owner = component_owner;
        }


        /// <summary>
        /// Open WPF window on double-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Ignore;
        }

        #endregion


        /* ------------------------------------------------------------------*/
        #region protected functions

        /// <summary>
        /// The rendering of the attribute is overriden here. The wires, capsule, grips, icons and highlighted boxes are all drawn in this method.
        /// </summary>
        /// <param name="canvas">The canvas which calls this method. See <see cref="GH_Canvas"/>.</param>
        /// <param name="graphics">The graphics that comes with canvas.</param>
        /// <param name="channel">The channel that the rendering is being called in. Canvas rendering takes place in different channels. See <see cref="GH_CanvasChannel"/>.</param>
        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            switch (channel)
            {
                case GH_CanvasChannel.First: // == Groups

                    break;

                case GH_CanvasChannel.Wires:

                    break;

                case GH_CanvasChannel.Objects:
                    break;

                case GH_CanvasChannel.Overlay: // == Last
                    break;

                default:
                    base.Render(canvas, graphics, channel);
                    break;
            }
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        private System.Drawing.Bitmap get_icon()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("GrasshopperLinking.resources.logo.logo24.png");
            return new System.Drawing.Bitmap(stream);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        private LinkingComponent _owner = null;

        #endregion
    }
}
