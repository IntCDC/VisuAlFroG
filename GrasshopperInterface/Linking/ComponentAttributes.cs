using System;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI;
using Grasshopper;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel.Special;
using System.Linq;
using GrasshopperInterface.Utilities;



/*
 * Grasshopper linking component attributes
 * 
 * *** CODE is mainly based on \Opossum\Opossum2_0_Proto_A\OptComponentAttributes.cs on https://github.tik.uni-stuttgart.de/icd/Opossum ***
 * 
 */
namespace GrasshopperInterface
{
    public class ComponentAttributes : GH_ComponentAttributes
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public ComponentAttributes(VisuAlFroG component_owner, RuntimeMessages runtimemessages) : base(component_owner)
        {
            _owner = component_owner;
            _runtimemessages = runtimemessages;

            /// _runtimemessages.Add(Log.Level.Warn, "[ComponentAttributes] ...");

            _linker_list = new List<AbstractParamLinker>() {
                new ParamLinker<GH_NumberSlider>(AbstractParamLinker.ParamType.IN, "Variables", runtimemessages),
                new ParamLinker<GH_NumberSlider>(AbstractParamLinker.ParamType.IN, "Objectives", runtimemessages),
                // new ParamLinker<GH_NumberSlider>(ParamType.OUT, "Results"),
            };


            float in_count = 0.0f;
            float out_count = 0.0f;
            foreach (var linker in _linker_list)
            {
                in_count += (linker._ParamType == AbstractParamLinker.ParamType.IN) ? (1.0f) : (0.0f);
                out_count += (linker._ParamType == AbstractParamLinker.ParamType.OUT) ? (1.0f) : (0.0f);
            }
            float linker_idx = 1.0f;
            foreach (var linker in _linker_list)
            {
                var count = (linker._ParamType == AbstractParamLinker.ParamType.IN) ? (in_count) : (out_count);
                linker._Fraction = linker_idx / (count + 1.0F); ;
                linker_idx++;
            }
        }

        public List<Tuple<Guid, string, double>> OutputValues()
        {
            var values_list = new List<Tuple<Guid, string, double>>();
            foreach (var linker in _linker_list)
            {
                foreach (var map in linker._GuidValueMap)
                {
                    values_list.Add(new Tuple<Guid, string, double>(map.Key, map.Value.Item1, map.Value.Item2));
                }
            }
            return values_list;
        }

        /// <summary>
        /// Overrides functionality for when a mouse click event is handled by the Canvas. 
        /// <seealso cref="IsPickRegion"/> needs to return true for this event to be handled by the component itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (var linker in _linker_list)
                {
                    if (linker.IsGripWithinReach(Bounds, e.CanvasLocation))
                    {
                        if (linker.AttachNewInteraction(sender, e, _owner))
                        {
                            return GH_ObjectResponse.Handled;
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                foreach (var linker in _linker_list)
                {
                    if (linker.ParamTextBox(Bounds).Contains(e.CanvasLocation))
                    {
                        linker.ShowContextMenu(e.ControlLocation);
                        return GH_ObjectResponse.Handled;
                    }
                }
            }
            return base.RespondToMouseDown(sender, e);
        }

        /// <summary>
        /// Overrides functionality for when a mouse click event is handled by the Canvas. 
        /// <seealso cref="IsPickRegion"/> needs to return true for this event to be handled by the component itself.
        /// </summary>
        /// <param name="sender">Canvas which picked up the Mouse event.</param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            // return GH_ObjectResponse.Handled;
            return base.RespondToMouseUp(sender, e);
        }

        /// <summary>
        /// Overrides functionality for when a mouse click event is handled by the Canvas. 
        /// <seealso cref="IsPickRegion"/> needs to return true for this event to be handled by the component itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            _owner.CreateWindow();
            return GH_ObjectResponse.Handled; // base.RespondToMouseDoubleClick(sender, e);
        }


        /// <summary>
        /// Overrides functionality for when a mouse click event is handled by the Canvas. 
        /// <seealso cref="IsPickRegion"/> needs to return true for this event to be handled by the component itself.
        /// </summary>
        /// <param name="sender">Canvas which picked up the Mouse event.</param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (IsParamRegion(e.CanvasLocation))
            {
                switch (Control.ModifierKeys)
                {
                    case Keys.Control | Keys.Shift:
                        Instances.CursorServer.AttachCursor(sender, "GH_Rewire");
                        break;
                    case Keys.Control:
                        Instances.CursorServer.AttachCursor(sender, "GH_RemoveWire");
                        break;
                    case Keys.Shift:
                        Instances.CursorServer.AttachCursor(sender, "GH_AddWire");
                        break;
                    default:
                        Instances.CursorServer.AttachCursor(sender, "GH_NewWire");
                        break;
                }
                return GH_ObjectResponse.Handled;
            }

            return base.RespondToMouseMove(sender, e);
        }

        public override bool IsTooltipRegion(PointF point)
        {
            _tooltip_linker = null;
            foreach (var linker in _linker_list)
            {
                if (linker.ParamTextBox(Bounds).Contains(point))
                {
                    _tooltip_linker = linker;
                    return true;
                }
            }
            return base.IsTooltipRegion(point);
        }

        public override void SetupTooltip(PointF point, GH_TooltipDisplayEventArgs e)
        {
            if (_tooltip_linker != null)
            {
                _tooltip_linker.SetToolTip(e);
            }
            base.SetupTooltip(point, e);
        }

        /// <summary>
        /// Tells the <see cref="GH_Canvas"/> if the cursor should consider activating the component.
        /// </summary>
        /// <param name="point">The cursor's position in the canvas.</param>
        /// <returns>True if the cursor is within the component or near to the input or output grips; false otherwise.</returns>
        public override bool IsPickRegion(PointF point)
        {
            return Bounds.Contains(point) || IsParamRegion(point);
        }

        public bool IsParamRegion(PointF point)
        {
            foreach (var linker in _linker_list)
            {
                if (linker.IsGripWithinReach(Bounds, point))
                {
                    return true;
                }
            }
            return false;
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
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            GH_Document doc = Owner.OnPingDocument();
            if (doc == null)
            {
                return;
            }

            switch (channel)
            {
                case GH_CanvasChannel.First: // == Groups
                    foreach (var linker in _linker_list)
                    {
                        linker.RenderRemoteBox(graphics, doc);
                    }
                    break;

                case GH_CanvasChannel.Wires:
                    foreach (var linker in _linker_list)
                    {
                        linker.RenderWire(graphics, doc, Bounds, Selected);
                    }
                    break;

                case GH_CanvasChannel.Objects:
                    render_component_capsule(canvas, graphics);
                    break;

                case GH_CanvasChannel.Overlay: // == Last
                    break;

                default:
                    base.Render(canvas, graphics, channel);
                    break;
            }
        }

        /// <summary>
        /// Here the layout of the component is overridden. The bounds for the component are determined here.
        /// </summary>
        protected override void Layout()
        {
            //This property calculates the new bounds from the changing Pivot:
            m_innerBounds = _inner_bounds;
            //Inherited from GH_ComponentAttributes. These created a LinkedParamAttributes for each Param and change their Pivots and Bounds accordingly:
            LayoutInputParams(Owner, m_innerBounds);
            // Inherited from GH_ComponentAttributes. Ditto but for Output params:
            LayoutOutputParams(Owner, m_innerBounds);
            // Unions the bounds to any other attributes (input and output) and then inflates it by 2f on all sides:
            Bounds = LayoutBounds(Owner, m_innerBounds);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        private bool render_component_capsule(GH_Canvas canvas, System.Drawing.Graphics graphics)
        {
            canvas.SetSmartTextRenderingHint();
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Blue, 3, 6);
            capsule.SetJaggedEdges(false, false);
            capsule.Font = GH_FontServer.Large;
            capsule.TextOrientation = GH_Orientation.horizontal_near;

            foreach (var linker in _linker_list)
            {
                // [capsule] Add grip for parameters
                var param_grip = linker.ParamGrip(Bounds);
                if (linker._ParamType == AbstractParamLinker.ParamType.IN)
                {
                    capsule.AddInputGrip(param_grip.X, param_grip.Y);
                }
                else if (linker._ParamType == AbstractParamLinker.ParamType.OUT)
                {
                    capsule.AddOutputGrip(param_grip.X, param_grip.Y);
                }
            }

            /// OUTPUT
            capsule.AddOutputGrip(Owner.Params.First().Attributes.OutputGrip.Y);

            var implied_style = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Blue, Selected, Owner.Locked, Owner.Hidden);
            capsule.Render(graphics, implied_style);

            // Handle parameters
            foreach (var linker in _linker_list)
            {
                // [graphics] Draw text for parameters
                var param_box = linker.ParamTextBox(Bounds);
                graphics.DrawString(linker._Caption, GH_FontServer.Standard, Brushes.Black, param_box, GH_TextRenderingConstants.NearCenter);
            }

            /// OUTPUT
            string caption = "Output";
            SizeF string_size = GH_FontServer.MeasureString(caption, GH_FontServer.Large);
            RectangleF result_box = new RectangleF
            {
                Width = string_size.Width,
                Height = string_size.Height
            };
            result_box.X = Bounds.Right - result_box.Width;
            result_box.Y = Bounds.Top + (Bounds.Height * 0.5f) - (result_box.Height / 2.0f);
            graphics.DrawString(caption, GH_FontServer.Standard, Brushes.Black, result_box, GH_TextRenderingConstants.NearCenter);

            using (var icon = get_icon())
            {
                float size = 32.0f;
                RectangleF icon_bounds = new RectangleF
                {
                    X = Bounds.Left + Bounds.Width - (Bounds.Width / 2.0F) - (size / 2.0f),
                    Y = Bounds.Top + (Bounds.Height / 2.0f) - (size / 2.0f),
                    Height = size,
                    Width = size
                };
                capsule.RenderEngine.RenderIcon(graphics, icon, icon_bounds, 0, 0);
            }

            if (Owner.Obsolete && CentralSettings.CanvasObsoleteTags && (canvas.DrawingMode == GH_CanvasMode.Control))
            {
                GH_GraphicsUtil.RenderObjectOverlay(graphics, Owner, Bounds);
            }
            capsule.Dispose();

            return true;
        }

        private System.Drawing.Bitmap get_icon()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("GrasshopperInterface.resources.logo.logo32.png");
            return (stream != null) ? (new System.Drawing.Bitmap(stream)) : (null);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        private VisuAlFroG _owner = null; // only keep pointer
        private List<AbstractParamLinker> _linker_list = null; // set in ctor
        private AbstractParamLinker _tooltip_linker = null; // only keep pointer
        private RuntimeMessages _runtimemessages = null; // only keep pointer

        private RectangleF _inner_bounds
        {
            get { return new RectangleF(Pivot, new SizeF(140f, 70f)); }
        }

        #endregion
    }
}
