using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.NetworkInformation;
using Grasshopper.Kernel;
using Grasshopper.GUI;



/*
 * Drawing helpers for Grasshopper
 * 
* * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum***
 * 
 */
namespace GrasshopperLinking
{
    public class DrawingHelpers
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
            custom_line_cap.SetStrokeCaps(LineCap.Flat, LineCap.Flat);
            stroke_path.Dispose();

            return custom_line_cap;
        }

        /// <summary>
        /// [STATIC]
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static GraphicsPath WirePath(PointF anchor, RectangleF target)
        {
            PointF b = GH_GraphicsUtil.BoxClosestPoint(anchor, target);
            float num = GH_GraphicsUtil.Distance(anchor, b);
            GraphicsPath graphicsPath = new GraphicsPath();
            PointF pointF1 = new PointF(anchor.X - 0.5f * num, anchor.Y);
            PointF pt1 = anchor;
            PointF pt2 = pointF1;
            PointF pointF2 = b;
            graphicsPath.AddBezier(pt1, pt2, pointF2, pointF2);
            return graphicsPath;
        }

        /// <summary>
        /// [STATIC]
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Pen WirePen(Color color, bool highlighted)
        {
            if (highlighted)
            {
                return new Pen(color, 3.0f)
                {
                    StartCap = LineCap.Round,
                    CustomEndCap = DrawingHelpers.ArrowCap(),
                    //EndCap = LineCap.ArrowAnchor,
                };
            }
            else
            {
                return new Pen(Color.FromArgb(60, color), 2.0f)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round,
                };
            }
        }

        #endregion
    }
}
