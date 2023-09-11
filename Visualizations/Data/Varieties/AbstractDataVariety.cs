using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Core.Utilities;

using Visualizations.Data;



/*
 *  Abstract data variety
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractDataVariety<DataType> : IDataVariety
            where DataType : new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public Type Variety { get { return typeof(DataType); } }
            public object Get { get { return _data; } }

            public abstract DataDimensionality SupportedDimensionality { get; }
            public abstract List<Type> SupportedValueTypes { get; }


            /* ------------------------------------------------------------------*/
            // public functions

            public abstract void Update(ref GenericDataStructure data, DataDimensionality dimensionality, List<Type> value_types);

            public abstract void UpdateEntryAtIndex(int index, GenericDataEntry updated_entry);

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="value_types"></param>
            /// <returns></returns>
            protected bool CompatibleValueTypes(List<Type> value_types)
            {
                bool incompatible = false;
                foreach (var t in value_types)
                {
                    if (!SupportedValueTypes.Contains(t))
                    {
                        incompatible = true;
                    }
                }
                return !incompatible;
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="dimensionality"></param>
            /// <returns></returns>
            protected bool CompatibleDimensionality(DataDimensionality dimensionality)
            {
                return ((dimensionality & SupportedDimensionality) == dimensionality);
            }


            /* ------------------------------------------------------------------*/
            // private variables

            protected DataType _data = new DataType();
        }
    }
}
