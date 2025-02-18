using System;
using System.Collections.Generic;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.PointMarkers;
using Core.GUI;
using System.Windows;
using SciChart.Charting.ChartModifiers;
using System.Windows.Media;
using Core.Data;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartSeries<SurfaceType, DataType> : AbstractSciChartVisualization<SurfaceType>
            where SurfaceType : SciChartSurface, new()
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            #region public properties

            public sealed override Type _RequiredDataType { get; } = typeof(DataTypeSciChartSeries<DataType>);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractSciChartSeries(int uid) : base(uid) { }

            public override bool CreateUI()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    // Log Console does not depend on data
                    Log.Default.Msg(Log.Level.Warn, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();


                // Axis --------------------------------------------
                var xAxis = new NumericAxis()
                {
                    AxisTitle = "X Axis",
                    DrawMajorBands = false,
                };
                _Content.XAxis = xAxis;

                var yAxis = new NumericAxis()
                {
                    AxisTitle = "Y Axis",
                    GrowBy = new SciChart.Data.Model.DoubleRange(0.2, 0.2),
                    DrawMajorBands = false,
                };
                _Content.YAxis = yAxis;


                // Interaction -------------------------------------

                /// DEBUG 
                _Content.MouseLeftButtonUp += data_point_mouse_event;


                // Modifiers ---------------------------------------

                var modifier_selection_point = new DataPointSelectionModifier();
                modifier_selection_point.IsEnabled = true;
                modifier_selection_point.AllowsMultiSelection = true;

                var modifier_selection_series = new SeriesSelectionModifier();
                modifier_selection_series.IsEnabled = true;
                modifier_selection_series.SelectionChanged += event_series_selection_changed;

                _Content.ChartModifier = new ModifierGroup(
                    modifier_selection_point,
                    modifier_selection_series,
                    new RubberBandXyZoomModifier()
                    {
                        IsEnabled = false
                    },
                    new ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    },
                    new ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None
                    },
                    new MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
                    },
                    new RolloverModifier()
                    {
                        IsEnabled = true,
                        ShowTooltipOn = ShowTooltipOptions.MouseOver,
                        HoverDelay = 0.0,
                        ShowAxisLabels = false,
                    },
                    new LegendModifier()
                    {
                        IsEnabled = false,
                        ShowLegend = true,
                    }
                );

                _Content.ZoomExtents();

                _timer.Stop();
                _created = true;
                return _created;
            }

            public override void UpdateEntrySelection(IMetaData updated_meta_data)
            {
                foreach (var data_series in _Content.RenderableSeries)
                {
                    using (data_series.DataSeries.SuspendUpdates())
                    {
                        int values_count = data_series.DataSeries.Count;
                        for (int i = 0; i < values_count; i++)
                        {
                            if (updated_meta_data._Index == ((SciChartMetaData)data_series.DataSeries.Metadata[i])._Index)
                            {
                                ((SciChartMetaData)data_series.DataSeries.Metadata[i])._Selected = updated_meta_data._Selected;
                            }
                        }
                    }
                    var typed_series = data_series as DataType;
                    if (typed_series == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Failed to convert to typed series");
                        break;
                    }
                    typed_series.InvalidateVisual();
                }
            }

            public override void AttachMenu(MenubarWindow menubar)
            {
                base.AttachMenu(menubar);

                var option_hint = MenubarMain.GetDefaultMenuItem("Interaction");

                var clue_item = MenubarMain.GetDefaultMenuItem("Select Series | Drag & Drop Axes");
                clue_item.InputGestureText = "Left Mouse";
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                clue_item = MenubarMain.GetDefaultMenuItem("Zoom");
                clue_item.InputGestureText = "Mouse Wheel";
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                clue_item = MenubarMain.GetDefaultMenuItem("Pan");
                clue_item.InputGestureText = "Right Mouse";
                clue_item.IsEnabled = false;
                option_hint.Items.Add(clue_item);
                menubar.AddMenu(MenubarWindow.PredefinedMenuOption.VIEW, option_hint);
            }
            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// </summary>
            /// <param name="data_parent"></param>
            /// <returns></returns>
            protected override bool apply_data(SciChartSurface data_parent)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                foreach (var data_series in _Content.RenderableSeries)
                {
                    data_series.DataSeries.Clear();
                }
                data_parent.RenderableSeries.Clear();

                if (this._RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                try
                {
                    var data = (List<DataType>)_RequestDataCallback(_DataUID);
                    if (data != null)
                    {
                        double color_idx = 0.0;
                        foreach (var data_series in data)
                        {
                            double palette_index = color_idx / (double)data.Count;
                            data_series.AntiAliasing = true;
                            data_series.Style = get_series_style(palette_index);
                            data_parent.RenderableSeries.Add(data_series);

                            color_idx++;
                        }
                        return true;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                ///Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(List<DataType>).FullName);
                return false;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private System.Windows.Style get_series_style(double palette_index)
            {
                const int stroke_thickness = 2;
                const double marker_size = 8.0;

                var series_style = new System.Windows.Style();
                series_style.TargetType = typeof(DataType);

                var random_color = ColorTheme.PaletteColorViridis(palette_index); // ColorTheme.RandomColor();

                if (typeof(DataType) == typeof(FastColumnRenderableSeries))
                {
                    // SELECTION STYLE
                    Setter setter_stroke = new Setter();
                    setter_stroke.Property = FastColumnRenderableSeries.PaletteProviderProperty;
                    setter_stroke.Value = new SciChart_StrokePalette();
                    series_style.Setters.Add(setter_stroke);
                    // SELECTION STYLE

                    Setter setter_gradient = new Setter();
                    setter_gradient.Property = FastColumnRenderableSeries.FillProperty;
                    setter_gradient.Value = new SolidColorBrush(random_color);
                    series_style.Setters.Add(setter_gradient);
                }
                else
                {
                    Setter setter_stroke = new Setter();
                    setter_stroke.Property = BaseRenderableSeries.StrokeProperty;
                    setter_stroke.Value = random_color;
                    series_style.Setters.Add(setter_stroke);

                    Setter setter_thickness = new Setter();
                    setter_thickness.Property = BaseRenderableSeries.StrokeThicknessProperty;
                    setter_thickness.Value = stroke_thickness;
                    series_style.Setters.Add(setter_thickness);

                    // SELECTION STYLE
                    Trigger trigger = new Trigger();
                    trigger.Property = BaseRenderableSeries.IsSelectedProperty;
                    trigger.Value = true;
                    Setter setter_trigger = new Setter();
                    setter_trigger.Property = BaseRenderableSeries.StrokeProperty;
                    setter_trigger.Value = new DynamicResourceExtension("Color_StrokeSelected");
                    trigger.Setters.Add(setter_trigger);
                    series_style.Triggers.Add(trigger);

                    var pointmarker_default = new EllipsePointMarker()
                    {
                        StrokeThickness = 0,
                        Fill = random_color,
                        Width = marker_size,
                        Height = marker_size,
                        AntiAliasing = true,
                    };
                    Setter setter_point = new Setter();
                    setter_point.Property = BaseRenderableSeries.PointMarkerProperty;
                    setter_point.Value = pointmarker_default;
                    series_style.Setters.Add(setter_point);

                    var pointmarker_selected = new EllipsePointMarker()
                    {
                        StrokeThickness = 0,
                        Width = marker_size,
                        Height = marker_size,
                        AntiAliasing = true,
                    };
                    pointmarker_selected.SetResourceReference(EllipsePointMarker.FillProperty, "Color_StrokeSelected");
                    Setter setter_point_selected = new Setter();
                    setter_point_selected.Property = BaseRenderableSeries.SelectedPointMarkerProperty;
                    setter_point_selected.Value = pointmarker_selected;
                    series_style.Setters.Add(setter_point_selected);
                    // SELECTION STYLE
                }

                return series_style;
            }

            #endregion
        }
    }
}
