using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Visuals;
using System.Windows;
using Core.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.DataTypes;
using System.Dynamic;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace AbstractVisualizations
    {
        public abstract class AbstractSciChartParallel<SurfaceType, DataType> : AbstractSciChartVisualization<SurfaceType>
            where SurfaceType : SciChartSurface, new()
            where DataType : IDynamicMetaObjectProvider, new()
        {

            public override Type GetDataType()
            {
                return typeof(DataTypeSciChartParallel<DataType>);
            }

            public override bool GetData(object data_parent)
            {

                var parent = data_parent as SciChartParallelCoordinateSurface;
                var data = (ParallelCoordinateDataSource<DataType>)RequestDataCallback(GetDataType());
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                    return false;
                }

                parent.ParallelCoordinateDataSource = data;

                return true;
            }
        }
    }
}
