using System;
using System.Collections.Generic;
using Core.Data;
using Core.Utilities;



/*
 *  Abstract data variety
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractDataType<DataType> : IDataVariety
            where DataType : class
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public Type Variety { get { return typeof(DataType); } }
            public object Get { get { return _data; } }

            public abstract List<Dimension> SupportedDimensions { get; }
            public abstract List<Type> SupportedValueTypes { get; }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// TODO Use _created flag
            /// </summary>
            /// <param name="data"></param>
            /// <param name="data_dimension"></param>
            /// <param name="value_types"></param>
            public abstract void Create(ref GenericDataStructure data, int data_dimension, List<Type> value_types);

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="updated_entry"></param>
            public abstract void UpdateEntryAtIndex(GenericDataEntry updated_entry);

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
                if (incompatible)
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible data value types");
                }
                return !incompatible;
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="dimensionality"></param>
            /// <returns></returns>
            protected bool CompatibleDimensionality(int data_dimension)
            {
                bool compatible = false;
                foreach (var dim in SupportedDimensions)
                {
                    switch (dim)
                    {
                        case (Dimension.Uniform):
                            compatible |= (data_dimension == 1);
                            break;
                        case (Dimension.TwoDimensional):
                            compatible |= (data_dimension == 2);
                            break;
                        case (Dimension.ThreeDimensional):
                            compatible |= (data_dimension == 3);
                            break;
                        case (Dimension.Multidimensional):
                            compatible |= (data_dimension > 0);
                            break;
                        default: break;
                    }
                }
                if (!compatible)
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible data dimension");
                }
                return compatible;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            protected DataType _data = default(DataType);
            protected bool _created = false;
        }
    }
}
