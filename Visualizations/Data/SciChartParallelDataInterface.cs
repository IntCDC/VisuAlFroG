using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;

using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.RenderableSeries;
using Visualizations.Types;



/*
 * SciChart Parallel Source Interface
 * 
 */

namespace Visualizations
{
    namespace Data
    {
        public class SciChartParallelDataInterface<DataType> : IDataInterface
            where DataType : new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public DataManager.RequestCallback_Delegate RequestDataCallback { get; set; }

            /* ------------------------------------------------------------------*/
            // public functions

            public SciChartParallelDataInterface()
            {
                _data = new ParallelCoordinateDataSource<DataType>();
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            public bool Set(object data_parent)
            {
                var parent = data_parent as SciChartParallelCoordinateSurface;
                if (parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data parent");
                    return false;
                }

                _data = (ParallelCoordinateDataSource<DataType>)RequestDataCallback(typeof(ParallelCoordinateDataSource<DataType>));
                if (_data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data");
                    return false;
                }

                parent.ParallelCoordinateDataSource = _data;

                /// TODO Set style

                return true;
            }

            public void SetDataStyle(DataStyles style)
            {
                /// TODO
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ParallelCoordinateDataSource<DataType> _data = null;
        }
    }
}
