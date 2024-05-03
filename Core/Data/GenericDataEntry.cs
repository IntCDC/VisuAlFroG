using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;



/*
 * Generic data entry
 * 
 */
namespace Core
{
    namespace Data
    {
        public class GenericDataEntry
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public List<object> _Values { get; private set; } = new List<object>();
            public MetaDataGeneric _Metadata { get; set; } = new MetaDataGeneric();


            /* ------------------------------------------------------------------*/
            // public functions

            public void AddValue(object value)
            {
                _Values.Add(value);
                update_metadata(value);
            }

            public void AddValues<T>(List<T> values) where T : struct
            {
                _Values.Clear();
                foreach (var value in values)
                {
                    _Values.Add((object)value);
                    update_metadata(value);
                }
            }

            public bool HasIndex(uint entry_index)
            {
                return (_Metadata._Index == entry_index);
            }

            public List<Type> ValueTypes()
            {
                var value_types = new List<Type>();
                foreach (var value in _Values)
                {
                    var type = value.GetType();
                    if (!value_types.Contains(type))
                    {
                        value_types.Add(value.GetType());
                    }
                }
                return value_types;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Re-evaluate statistics for provided value
            /// </summary>
            /// <param name="value"></param>
            private void update_metadata(object value)
            {
                _Metadata._Dimension = (uint)_Values.Count;
            }
        }
    }
}
