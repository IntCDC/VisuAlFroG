using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        public abstract class AbstractDataType<DataType> : IDataType
            where DataType : class
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public object _Get { get { return _data; } }
            public abstract List<Dimension> _SupportedDimensions { get; }
            public abstract List<Type> _SupportedValueTypes { get; }


            /* ------------------------------------------------------------------*/
            // public functions

            public AbstractDataType(PropertyChangedEventHandler update_metadata_handler, PropertyChangedEventHandler update_data_handler) 
            {
                _update_metadata_handler = update_metadata_handler;
                _update_data_handler = update_data_handler;
            }

            /// <summary>
            /// TODO Use _initialized flag
            /// </summary>
            /// <param name="data"></param>
            public abstract void UpdateData(GenericDataStructure data);

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="updated_entry"></param>
            public abstract void UpdateMetaDataEntry(IMetaData updated_meta_data);

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="value_types"></param>
            /// <returns></returns>
            protected bool CompatibleTypes(List<Type> value_types)
            {
                bool incompatible = false;
                foreach (Type t in value_types)
                {
                    if (!_SupportedValueTypes.Contains(t))
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
            protected bool CompatibleDimensionality(uint data_dimension)
            {
                bool compatible = false;
                foreach (Dimension dim in _SupportedDimensions)
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
            // protected variables

            protected DataType _data = default(DataType);
            protected bool _loaded = false;
            protected PropertyChangedEventHandler _update_metadata_handler = null;
            protected PropertyChangedEventHandler _update_data_handler = null;
        }
    }
}
