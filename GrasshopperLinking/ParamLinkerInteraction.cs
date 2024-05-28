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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Grasshopper.Documentation;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Grasshopper.Kernel.Special;
using Core.Data;



/*
 * Grasshopper parameter linking interaction
 * 
 */
namespace GrasshopperLinking
{
    public class ParamLinkerInteraction<LinkedParamType> : GH_AbstractInteraction
        where LinkedParamType : IGH_Param
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public ParamLinkerInteraction(GH_Canvas canvas, GH_CanvasMouseEvent canvas_mouse_event, GH_Component owner, Dictionary<Guid, Tuple<string, double>> guid_value_map, PointF grip) : base(canvas, canvas_mouse_event)
        {
            _target = null;
            _owner = owner;
            _mode = LinkMode.REPLACE;
            _guid_value_map = guid_value_map;
            _grip = grip;

            Instances.CursorServer.AttachCursor(Canvas, "GH_NewWire");
            canvas.StartAutoPan();
            canvas.CanvasPostPaintObjects += canvas_post_paint_objects;
        }

        public override void Destroy()
        {
            m_canvas.CanvasPostPaintObjects -= canvas_post_paint_objects;
            m_canvas.StopAutoPan();
            base.Destroy();
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            base.RespondToMouseMove(sender, e);
            GH_Document document = sender.Document;
            if (!m_active || (document == null))
            {
                return GH_ObjectResponse.Ignore;
            }
            try_link(document, e.CanvasLocation);

            _point = e.CanvasLocation;

            sender.Refresh();
            return GH_ObjectResponse.Handled;
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_Document document = sender.Document;
            if (document == null)
            {
                return GH_ObjectResponse.Ignore;
            }
            link(document);
            Instances.CursorServer.ResetCursor(sender);
            return GH_ObjectResponse.Release;
        }

        public override GH_ObjectResponse RespondToKeyUp(GH_Canvas sender, KeyEventArgs e)
        {
            adjust_courser(e);
            return GH_ObjectResponse.Ignore;
        }

        public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Cancel:
                    _owner = null;
                    _target = null;
                    Instances.CursorServer.ResetCursor(sender);
                    return GH_ObjectResponse.Release;
                default:
                    adjust_courser(e);
                    return GH_ObjectResponse.Ignore;
            }
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        private void try_link(GH_Document doc, PointF point)
        {
            _target = null;
            GH_DocumentObject docobj = (GH_DocumentObject)doc.FindObject(point, 3f);
            if (docobj is LinkedParamType)
            {
                _target = (IGH_Param)docobj;
            }
        }

        private void link(GH_Document doc)
        {
            if (_target != null)
            {
                double param_value = 0.0f;
                var param = (LinkedParamType)Activator.CreateInstance(typeof(LinkedParamType));
                var input = (GH_DocumentObject)doc.FindObject(_target.InstanceGuid, true);
                if (input != null)
                {
                    if (input.ComponentGuid == param.ComponentGuid)
                    {
                        /// TODO ...
                        if (typeof(LinkedParamType) == typeof(GH_NumberSlider))
                        {
                            param_value = ((double)((GH_NumberSlider)input).CurrentValue);
                        }
                    }
                    else
                    {
                        /// XXXX Unknown parameter type
                    }
                }
                var param_name = (_target.NickName == "") ? (_target.Name) : (_target.NickName);

                switch (_mode)
                {
                    case LinkMode.ADD:
                        {
                            if (!_guid_value_map.ContainsKey(_target.InstanceGuid))
                            {
                                _owner.RecordUndoEvent("UndoEvents_AddParamterLink");
                                _guid_value_map.Add(_target.InstanceGuid, new Tuple<string, double>(param_name, param_value));
                            }
                            break;
                        }
                    case LinkMode.REMOVE:
                        {
                            if (_guid_value_map.ContainsKey(_target.InstanceGuid))
                            {
                                _owner.RecordUndoEvent("UndoEvents_RemoveParamterLink");
                                _guid_value_map.Remove(_target.InstanceGuid);
                            }
                            break;
                        }
                    case LinkMode.REPLACE:
                    default:
                        {
                            if (_guid_value_map.Count == 1 && _guid_value_map.ContainsKey(_target.InstanceGuid))
                            {
                                break;
                            }
                            _owner.RecordUndoEvent("UndoEvents_SetParamterLink");
                            _guid_value_map.Clear();
                            _guid_value_map.Add(_target.InstanceGuid, new Tuple<string, double>(param_name, param_value));
                            break;
                        }
                }
            }
            if (_owner != null)
            {
                _owner.ExpireSolution(true);
            }
        }


        private void canvas_post_paint_objects(GH_Canvas sender)
        {
            try
            {
                if (float.IsNaN(_point.X) || float.IsNaN(_point.Y))
                {
                    return;
                }
                sender.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                RectangleF target = new RectangleF(_point, new SizeF(2f, 2f));
                if (_target != null)
                {
                    target = _target.Attributes.Bounds;
                    target.Inflate(0.0f, 0.0f);
                }
                var path = DrawingHelpers.WirePath(_grip, target);
                var pen = DrawingHelpers.WirePen(Color.MediumVioletRed, true);
                Canvas.Graphics.DrawPath(pen, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void adjust_courser(KeyEventArgs e)
        {
            LinkMode prev_mode = _mode;
            _mode = LinkMode.REPLACE;

            if (e.Control)
            {
                _mode = LinkMode.REMOVE;
            }
            else if (e.Shift)
            {
                _mode = LinkMode.ADD;
            }

            if (_mode != prev_mode)
            {
                switch (_mode)
                {
                    case LinkMode.ADD:
                        Instances.CursorServer.AttachCursor(Canvas, "GH_AddWire");
                        break;
                    case LinkMode.REMOVE:
                        Instances.CursorServer.AttachCursor(Canvas, "GH_RemoveWire");
                        break;
                    case LinkMode.REPLACE:
                        Instances.CursorServer.AttachCursor(Canvas, "GH_NewWire");
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables 

        private enum LinkMode
        {
            ADD,
            REMOVE,
            REPLACE
        }

        private GH_Component _owner = null;
        private LinkMode _mode = LinkMode.REPLACE;
        private IGH_Param _target = null;
        private PointF _point = new PointF(float.NaN, float.NaN);
        private PointF _grip = new PointF(float.NaN, float.NaN);
        private Dictionary<Guid, Tuple<string, double>> _guid_value_map = null;


        #endregion
    }
}
