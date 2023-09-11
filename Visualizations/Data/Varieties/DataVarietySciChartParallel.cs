using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Data;
using Visualizations.Abstracts;
using SciChart.Charting.Visuals.RenderableSeries;
using Core.Utilities;



/*
 *  SciChart data variety for parallel coordinates 
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataVarietySciChartParallel<DataType> : AbstractDataVariety<DataInterfaceSciChartParallel<DataType>>
            where DataType : new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override DataDimensionality SupportedDimensionality { get { return (DataDimensionality.TwoDimensional); } }
            public sealed override List<Type> SupportedValueTypes { get { return new List<Type>() { typeof(double), typeof(int) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override void Update(ref GenericDataStructure data, DataDimensionality dimensionality, List<Type> value_types)
            {
                if (!CompatibleDimensionality(dimensionality))
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible data dimensionality");
                    return;
                }
                if (!CompatibleValueTypes(value_types))
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible data value types");
                    return;
                }

                if (dimensionality == DataDimensionality.TwoDimensional)
                {

                }
            }

            public override void UpdateEntryAtIndex(int index, GenericDataEntry updated_entry)
            {


            }
        }
    }
}
