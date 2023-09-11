using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Core.Utilities;
using SciChart.Charting.Visuals.RenderableSeries;

using Visualizations.Abstracts;



/*
 * Generic data structure interface
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataInterfaceGeneric<DataType> : AbstractDataInterface 
            where DataType : GenericDataStructure
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Set(ref GenericDataStructure data_parent)
            {
                var data = (DataType)RequestDataCallback(typeof(DataType));
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                    return false;
                }

                data_parent = data;

                return true;
            }
        }
    }
}
