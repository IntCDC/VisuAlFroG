using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Visuals;
using System.Windows;
using Core.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Visualizations
    {
        public abstract class AbstractSciChartSeries<SurfaceType, DataType> : AbstractSciChartVisualization<SurfaceType>
            where SurfaceType : SciChartSurface, new()
            where DataType : BaseRenderableSeries, new()
        {
            public override Type GetDataType()
            {
                return typeof(DataTypeSciChartSeries<DataType>);
            }

            public override bool GetData(object data_parent)
            {

                var parent = data_parent as SciChartSurface;
                var data = (List<DataType>)RequestDataCallback(GetDataType());
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                    return false;
                }
                if (data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return false;
                }

                foreach (var data_series in data)
                {
                    data_series.Name = UniqueID.Generate();
                    data_series.SelectionChanged += event_selection_changed;
                    parent.RenderableSeries.Add(data_series);
                }
                return true;
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            /// <exception cref="NotImplementedException"></exception>
            private void event_selection_changed(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
