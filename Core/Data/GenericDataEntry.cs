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

            public List<object> Values { get { return _values; } }
            public MetaDataGeneric MetaData { get { return _metadata; } set { _metadata = value; } }
            public uint Dimension { get { return (uint)_values.Count; } }

            public double Min { get { return _min; } }
            public double Max { get { return _max; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public void AddValue(object value)
            {
                _values.Add(value);
                calculate_statistics(value);
            }

            public void AddValues<T>(List<T> values) where T : struct
            {
                _values.Clear();
                foreach (var value in values)
                {
                    _values.Add((object)value);
                    calculate_statistics(value);
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
            // private functions

            /// <summary>
            /// Re-evaluate statistics for provided value
            /// </summary>
            /// <param name="value"></param>
            private void calculate_statistics(object value)
            {
                var type = value.GetType();
                if ((type == typeof(double)) || (type == typeof(int)) || (type == typeof(uint)) || (type == typeof(long)) || (type == typeof(ulong)) || (type == typeof(float)))
                {
                    _min = Math.Min(_min, Convert.ToDouble(value));
                    _max = Math.Max(_max, Convert.ToDouble(value));
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private List<object> _values = new List<object>();
            private MetaDataGeneric _metadata = new MetaDataGeneric();

            private double _min = double.PositiveInfinity;
            private double _max = double.NegativeInfinity;

        }
    }
}
