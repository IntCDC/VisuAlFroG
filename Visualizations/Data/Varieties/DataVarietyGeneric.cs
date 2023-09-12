using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Data;
using Visualizations.Abstracts;
using Core.Utilities;


/*
 *  Generic data variety
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataVarietyGeneric : AbstractDataVariety<GenericDataStructure>
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public sealed override List<Dimension> SupportedDimensions
            {
                get
                {
                    return new List<Dimension>() { Dimension.Uniform, Dimension.TwoDimensional, Dimension.ThreeDimensional, Dimension.Multidimensional };
                }
            }

            public sealed override List<Type> SupportedValueTypes
            {
                get
                {
                    return new List<Type>() { typeof(string), typeof(double), typeof(float), typeof(int), typeof(uint), typeof(long), typeof(ulong) };
                }
            }


            /* ------------------------------------------------------------------*/
            // public functions

            public override void Create(ref GenericDataStructure data, int data_dimension, List<Type> value_types)
            {
                _created = false;
                if (!CompatibleDimensionality(data_dimension) || !CompatibleValueTypes(value_types)) {
                    return;
                }
                if (data == null)
                {
                    return;
                }

                _data = data;
                _created = true;
            }

            public override void UpdateEntryAtIndex(GenericDataEntry updated_entry)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }
                /* Not required for meta data update
                var entry = _data.EntryAtIndex(index);
                if (entry != null)
                {
                    entry.MetaData = updated_entry.MetaData;
                }
                else 
                {
                Log.Default.Msg(Log.Level.Debug, "Can not find data entry at index: " + index.ToString());
                }
                */
                }
        }
    }
}
