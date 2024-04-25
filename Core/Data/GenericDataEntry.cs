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

            public bool HasIndex(int entry_index)
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
                var type = value.GetType();
                if ((type == typeof(double)) || (type == typeof(int)) || (type == typeof(uint)) || (type == typeof(long)) || (type == typeof(ulong)) || (type == typeof(float)))
                {
                    _Metadata._Min = Math.Min(_Metadata._Min, Convert.ToDouble(value));
                    _Metadata._Max = Math.Max(_Metadata._Max, Convert.ToDouble(value));
                }

                _Metadata._Dimension = (uint)_Values.Count;
            }
        }
    }
}
