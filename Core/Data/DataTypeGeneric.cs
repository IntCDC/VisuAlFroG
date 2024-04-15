using System;
using System.Collections.Generic;
using Core.Utilities;
using Core.Abstracts;
using System.ComponentModel;



/*
 *  Generic data variety
 * 
 */
namespace Core
{
    namespace Data
    {
        public class DataTypeGeneric : AbstractDataType<GenericDataStructure>
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

            public DataTypeGeneric(PropertyChangedEventHandler meta_data_update_handler) : base(meta_data_update_handler) { }

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
                int index = 0;
                init_metadata(_data, ref index);

                _created = true;
            }

            public override void UpdateMetaData(IMetaData updated_meta_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }
                var entry = _data.EntryAtIndex(updated_meta_data.Index);
                if (entry != null)
                {
                    entry.MetaData.IsSelected = updated_meta_data.IsSelected;
                }
                else 
                {
                    Log.Default.Msg(Log.Level.Debug, "Can not find data entry at index: " + updated_meta_data.Index.ToString());
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Recursively initialize meta data.
            /// </summary>
            /// <param name="data">The data branch.</param>
            /// <param name="index">The entry index</param>
            private void init_metadata(GenericDataStructure data, ref int index)
            {
                foreach (var entry in data.Entries)
                {
                    entry.MetaData.IsSelected = false;
                    entry.MetaData.Index = index;
                    entry.MetaData.PropertyChanged += _meta_data_update_handler;

                    index++;
                }
                foreach (var branch in data.Branches)
                {
                    init_metadata(branch, ref index);
                }
            }

        }
    }
}
