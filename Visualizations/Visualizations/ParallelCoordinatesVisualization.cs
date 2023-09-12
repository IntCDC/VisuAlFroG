using System;
using Core.Abstracts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Visualizations.SciChartInterface;
using System.Windows;
using Core.Utilities;
using System.Runtime.Remoting.Contexts;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Model.DataSeries;
using SciChart.Drawing;
using SciChart.Core;
using SciChart.Data;
using System.Windows.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.RenderableSeries.Animations;
using SciChart.Charting.Model.ChartSeries;
using System.ComponentModel;
using System.Linq;
using SciChart.Charting.Visuals.PointMarkers;
using System.Windows.Input;
using Visualizations.Abstracts;
using SciChart.Charting.ChartModifiers;
using SciChart.Core.Utility.Mouse;
using Visualizations.Data;
using System.Dynamic;



/*
 * Visualization: Parallel Coordinates Plot (2D)
 * 
 */
namespace Visualizations
{
    namespace Varieties
    {
        public class ParallelCoordinatesVisualization : AbstractSciChartVisualization<SciChartParallelCoordinateSurface, DataInterfaceSciChartParallel<ExpandoObject>>
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Parallel Coordinates Plot (2D)"; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool ReCreate()
            {
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Re-creation of content should not be required");
                    return false;
                }
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (DataInterface.RequestDataCallback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();


                Content.ChartTitle = "Parallel Coordinates Plot";


                // Modifiers ---------------------------------------
                Content.ChartModifier = new SciChart.Charting.ChartModifiers.ModifierGroup(
                    new SciChart.Charting.ChartModifiers.RubberBandXyZoomModifier()
                    {
                        IsEnabled = false
                    },
                    new SciChart.Charting.ChartModifiers.ZoomExtentsModifier()
                    {
                        IsEnabled = false
                    },
                    new SciChart.Charting.ChartModifiers.ZoomPanModifier()
                    {
                        IsEnabled = true,
                        ExecuteOn = SciChart.Charting.ChartModifiers.ExecuteOn.MouseRightButton,
                        ClipModeX = SciChart.Charting.ClipMode.None
                    },
                    new SciChart.Charting.ChartModifiers.MouseWheelZoomModifier()
                    {
                        IsEnabled = true,
                        ActionType = SciChart.Charting.ActionType.Zoom,
                        XyDirection = SciChart.Charting.XyDirection.XYDirection
                    },
                    new SciChart.Charting.ChartModifiers.DataPointSelectionModifier()
                    {
                        IsEnabled = true
                    },
                    new SciChart.Charting.ChartModifiers.ParallelAxisReorderModifier()
                    {
                        IsEnabled = true,
                        //AxesReordered = on_axis_reordered
                    }
                    //<s:ParallelAxisReorderModifier AxesReordered = "OnAxisReordered" IsEnabled = "{Binding IsChecked, Mode=OneWay, ElementName=IsReorderEnabled}" />
                ) ;


                _timer.Stop();
                _created = true;
                return _created;
            }



            /* ------------------------------------------------------------------*/
            // private functions

            /*
            private void on_axis_reordered(object sender, ParallelAxisReorderArgs args)
            {
                _pcp_source?.ReorderItems(args.OldIndex, args.NewIndex);
            }
            */
        }
    }
}
