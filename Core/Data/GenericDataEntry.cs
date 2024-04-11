using System;
using System.Collections.Generic;
using Core.Abstracts;



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

            public List<object> Values { get { return _values; } }

            public MetaDataGeneric MetaData { get { return _metadata; } set { _metadata = value; } }

            public int Dimension { get { return _values.Count; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public void AddValue(object value) { _values.Add(value); }

            public void AddValues<T>(List<T> values)
            {
                _values.Clear();
                foreach (var value in values)
                {
                    _values.Add((object)value);
                }
            }

            public bool HasIndex(int entry_index)
            {
                return (_metadata.Index == entry_index);
            }

            public List<Type> ValueTypes()
            {
                var value_types = new List<Type>();
                foreach (var value in _values)
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
            // private variables

            private List<object> _values = new List<object>();
            private MetaDataGeneric _metadata = new MetaDataGeneric();
        }
    }
}
