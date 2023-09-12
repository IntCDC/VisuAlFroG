using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using Visualizations.Varieties;
using Visualizations.Abstracts;



/*
 * SciChart parallel source interface
 * 
 */

namespace Visualizations
{
    namespace Data
    {
        public class DataInterfaceSciChartParallel<DataType> : AbstractDataInterface
            where DataType : class
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Set(object data_parent)
            {
                var parent = data_parent as SciChartParallelCoordinateSurface;
                if (parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Can not convert data parent parameter to required type");
                    return false;
                }

                var data = (ParallelCoordinateDataSource<DataType>)RequestDataCallback(typeof(ParallelCoordinateDataSource<DataType>));
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
