using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;



/*
 * Drawing helpers for Grasshopper
 * 
* * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum***
 * 
 */
namespace GrasshopperInterface
{
    public class gh_DrawingHelpers
    {
        /* ------------------------------------------------------------------*/
        #region static functions

        /// <summary>
        /// [STATIC] This contains the definition of the arrow drawn at the tip of the wire when drawing wires dragging from the different inputs and outputs to other objects in Grasshopper.
        /// </summary>
        /// <returns>A <see cref="CustomLineCap"/> for use in the different pens for the different wires.</returns>
        public static CustomLineCap ArrowCap()
        {
            float arrow_size = 2f;
            GraphicsPath stroke_path = new GraphicsPath();
            PointF[] points = new PointF[3]
            {
                new PointF(-1f * arrow_size, -1f * arrow_size),
                new PointF(0.0f, 0.0f),
                new PointF(1f * arrow_size, -1f * arrow_size)
            };
            stroke_path.AddLines(points);
            CustomLineCap custom_line_cap = new CustomLineCap(null, stroke_path, LineCap.ArrowAnchor);
            custom_line_cap.SetStrokeCaps(LineCap.Flat,LineCap.Flat);
            stroke_path.Dispose();

            return custom_line_cap;
        }

        #endregion
    }
}
