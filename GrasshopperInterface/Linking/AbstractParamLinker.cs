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
using System.Drawing.Drawing2D;
using GrasshopperInterface.Utilities;
using Core.Utilities;



/*
 * Parameter linking
 * 
 * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum ***
 * 
 */
namespace GrasshopperInterface
{
    public abstract class AbstractParamLinker
    {
        /* ------------------------------------------------------------------*/
        #region public enum

        public enum ParamType
        {
            IN,
            OUT,
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region public properties

        public string _Caption { get; private set; } = "";
        public ParamType _ParamType { get; private set; } = ParamType.IN;

        // Required to be adjusted by class creating this object
        public float _Fraction { get; set; } = 0.5f;

        #endregion

        /* ------------------------------------------------------------------*/
        #region protected properties

        public Dictionary<Guid, Tuple<string, double>> _GuidValueMap { get; private set; } = new Dictionary<Guid, Tuple<string, double>>();

        #endregion

        /* ------------------------------------------------------------------*/
        #region abstract functions

        public abstract bool AttachNewInteraction(GH_Canvas canvas, GH_CanvasMouseEvent canvas_mouse_event, VisuAlFroG owner);

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public AbstractParamLinker(ParamType param_type, string caption, RuntimeMessages runtimemessages)
        {
            _Caption = caption;
            _ParamType = param_type;
            _runtimemessages = runtimemessages;

            /// _runtimemessages.Add(Log.Level.Warn, "[AbstractParamLinker] ...");
        }

        public RectangleF ParamTextBox(RectangleF bounds)
        {
            SizeF string_size = GH_FontServer.MeasureString(_Caption, GH_FontServer.Large);
            RectangleF param_box = new RectangleF
            {
                Width = string_size.Width,
                Height = string_size.Height
            };
            if (_ParamType == ParamType.IN)
            {
                param_box.X = bounds.Left + 5f;
            }
            else
            {
                param_box.X = bounds.Right - param_box.Width - 5f;
            }
            param_box.Y = bounds.Top + (bounds.Height * _Fraction) - (param_box.Height / 2.0f);
            return param_box;
        }

        public PointF ParamGrip(RectangleF bounds)
        {
            var param_box = ParamTextBox(bounds);
            if (_ParamType == ParamType.IN)
            {
                _last_grip = new PointF(bounds.Left, param_box.Y + (0.5f * param_box.Height));
            }
            else
            {
                _last_grip = new PointF(bounds.Right, param_box.Y + (0.5f * param_box.Height));
            }
            return _last_grip;
        }

        public bool IsGripWithinReach(RectangleF bounds, PointF point, double radius = 12.0)
        {
            return (GH_GraphicsUtil.Distance(point, ParamGrip(bounds)) <= radius);
        }

        public void RenderRemoteBox(Graphics graphics, GH_Document doc)
        {
            foreach (var map in _GuidValueMap)
            {
                IGH_DocumentObject obj = doc.FindObject(map.Key, true);
                if (obj == null)
                {
                    return;
                }
                Rectangle rectangle = GH_Convert.ToRectangle(obj.Attributes.Bounds);
                rectangle.Inflate(3, 3);
                var brush = new SolidBrush(Color.FromArgb(100, Color.MediumVioletRed));
                var pen = new Pen(brush);

                graphics.FillRectangle(brush, rectangle);
                graphics.DrawRectangle(pen, rectangle);
            }
        }

        public void RenderWire(Graphics graphics, GH_Document doc, RectangleF bounds, bool selected)
        {
            foreach (var map in _GuidValueMap)
            {
                IGH_DocumentObject obj = doc.FindObject(map.Key, true);
                if (obj == null)
                {
                    return;
                }
                PointF source = ParamGrip(bounds);
                Rectangle target = GH_Convert.ToRectangle(obj.Attributes.Bounds);

                /*
                if (selected)
                {
                    target.Inflate(5, 5);
                }
                PointF p0 = source;
                PointF p3 = new PointF(target.Right, target.Top + (target.Height / 2.0f));
                float dist = GH_GraphicsUtil.Distance(source, p3);
                if (dist < 5.0f)
                {
                    dist = 5.0f;
                }
                PointF p1 = new PointF(p0.X, p0.Y + dist / 2f);
                PointF p2 = new PointF(p3.X + dist / 2f, p3.Y);
                GraphicsPath path = new GraphicsPath();
                path.AddBezier(p0, p1, p2, p3);
                */

                PointF closest_point = GH_GraphicsUtil.BoxClosestPoint(source, target);
                float distance = GH_GraphicsUtil.Distance(source, closest_point);
                PointF p0 = source;
                PointF p1 = new PointF(source.X - 0.5f * distance, source.Y);
                PointF p2 = closest_point;
                PointF p3 = closest_point;
                GraphicsPath path = new GraphicsPath();
                path.AddBezier(p0, p1, p2, p3);

                var pen = DrawingHelpers.WirePen(Color.MediumVioletRed, selected);

                graphics.DrawPath(pen, path);
            }
        }

        public void ShowContextMenu(System.Drawing.Point point)
        {
            /// TODO
            /*
            ToolStripDropDownMenu menu = new ToolStripDropDownMenu
            {
                ShowItemToolTips = true,
                ShowImageMargin = false
            };
            ToolStripMenuItem menu_item = GH_DocumentObject.Menu_AppendItem(menu, "MENU ITEM", ... );
            menu.Show(Instances.ActiveCanvas, point);
            Instances.ActiveCanvas.CanvasPostPaintObjects += MenuCanvasPostPaintEventHandler; // Draw rectangle around highlighted menu item
            */
        }

        public void SetToolTip(GH_TooltipDisplayEventArgs e)
        {
            /// TODO
            /* 
            e.Title       = "TODO";
            e.Text        = "TODO";
            e.Description = "TODO";
            */
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region protected variables

        protected PointF _last_grip = new PointF(float.NaN, float.NaN);
        protected RuntimeMessages _runtimemessages = null; // only keep pointer

        #endregion
    }
}
