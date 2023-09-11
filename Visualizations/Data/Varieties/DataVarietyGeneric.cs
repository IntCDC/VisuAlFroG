using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Data;
using Visualizations.Abstracts;



/*
 *  Generic data variety
 * 
 */
using DataType = Visualizations.Data.GenericDataStructure;
using Core.Utilities;

namespace Visualizations
{
    namespace Data
    {
        public class DataVarietyGeneric : AbstractDataVariety<DataType>
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override DataDimensionality SupportedDimensionality { get { return (DataDimensionality.All); } }
            public sealed override List<Type> SupportedValueTypes { get { return new List<Type>() { typeof(string), typeof(double), typeof(int) }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override void Update(ref GenericDataStructure data, DataDimensionality dimensionality, List<Type> value_types)
            {
                Log.Default.Msg(Log.Level.Debug, "Dimensionality Flag: " + Convert.ToString((int)SupportedDimensionality, 2));
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
                _data = data;
            }

            public override void UpdateEntryAtIndex(int index, GenericDataEntry updated_entry)
            {
                /*
                var entry = _data.EntryAtIndex(index);
                if (entry != null)
                {
                    entry.MetaData = updated_entry.MetaData;
                }
                Log.Default.Msg(Log.Level.Debug, "Can not find data entry at index: " + index.ToString());
                */
            }
        }
    }
}
