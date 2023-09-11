﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Abstracts;



/*
 * Generic data entry
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class GenericDataEntry
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public List<object> Values { get { return _values; } }

            public MetaData MetaData { get { return _metadata; } set { _metadata = value; } }

            public DataDimensionality Dimensionality
            {
                get
                {
                    var dim = DataDimensionality.None;
                    dim = (_values.Count == 1) ? (dim | DataDimensionality.Uniform) : (dim);
                    dim = (_values.Count == 2) ? (dim | DataDimensionality.TwoDimensional) : (dim);
                    dim = (_values.Count == 3) ? (dim | DataDimensionality.ThreeDimensional) : (dim);
                    dim = (_values.Count > 3) ? (dim | DataDimensionality.Multidimensional) : (dim);
                    return dim;
                }
            }


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
            private MetaData _metadata = new MetaData();
        }
    }
}
