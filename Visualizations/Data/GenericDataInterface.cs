using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;

using SciChart.Charting.Visuals.RenderableSeries;



/*
 * Generic Data Structure Interface
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class GenericDataInterface<DataType> : IDataInterface 
            where DataType : GenericDataStructure, new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public DataManager.RequestCallback_Delegate RequestDataCallback { get; set; }


            /* ------------------------------------------------------------------*/
            // public functions

            public GenericDataInterface()
            {
                _data = new DataType();
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            public bool Set(object data_parent)
            {
                var parent = data_parent as GenericDataStructure;
                if (parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data parent");
                    return false;
                }

                _data = (DataType)RequestDataCallback(typeof(DataType));
                if (_data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data");
                    return false;
                }

                parent = _data;

                /// TODO Set style

                return true;

            }

            public void SetDataStyle(DataStyles style)
            {
                /// TODO
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private DataType _data = null;
        }
    }
}
