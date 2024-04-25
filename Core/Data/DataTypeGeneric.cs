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

            public sealed override List<Dimension> _SupportedDimensions { get; }
                = new List<Dimension>() { Dimension.Uniform, Dimension.TwoDimensional, Dimension.ThreeDimensional, Dimension.Multidimensional };

            public sealed override List<Type> _SupportedValueTypes { get; }
                = new List<Type>() { typeof(string), typeof(double), typeof(float), typeof(int), typeof(uint), typeof(long), typeof(ulong) };


            /* ------------------------------------------------------------------*/
            // public functions

            public DataTypeGeneric(PropertyChangedEventHandler meta_data_update_handler) : base(meta_data_update_handler) { }

            public override void UpdateData(GenericDataStructure data)
            {
                _loaded = false;
                _data = null;

                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }

                if (!CompatibleDimensionality(data.DataDimension()) || !CompatibleValueTypes(data.ValueTypes()))
                {
                    return;
                }
                _data = data;

                int index = 0;
                init_metadata(_data, ref index);

                _loaded = true;
            }

            public override void UpdateMetaDataEntry(IMetaData updated_meta_data)
            {
                if (!_loaded)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }
                var entry = _data.EntryAtIndex(updated_meta_data._Index);
                if (entry != null)
                {
                    entry._Metadata._Selected = updated_meta_data._Selected;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Can not find data entry at index: " + updated_meta_data._Index.ToString());
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
                foreach (var entry in data._Entries)
                {
                    entry._Metadata._Selected = false;
                    entry._Metadata._Index = index;
                    entry._Metadata.PropertyChanged += _meta_data_update_handler;

                    index++;
                }
                foreach (var branch in data._Branches)
                {
                    init_metadata(branch, ref index);
                }
            }
        }
    }
}
