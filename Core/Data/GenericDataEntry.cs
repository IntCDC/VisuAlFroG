using System;
using System.Collections.Generic;



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
            #region public properties

            public List<object> _Values { get; private set; } = new List<object>();
            public GenericMetaData _Metadata { get; set; } = new GenericMetaData();
            public List<Type> _Types { get; private set; } = new List<Type>();

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public void AddValue<T>(T value)
            {
                _Values.Add(value);
                _Metadata._Dimension = (uint)_Values.Count;
                var type = value.GetType();
                if (!_Types.Contains(type))
                {
                    _Types.Add(value.GetType());
                }
            }

            public void AddValues<T>(List<T> values) where T : struct
            {
                _Values.Clear();
                foreach (var value in values)
                {
                    AddValue(value);
                }
            }

            public bool HasIndex(uint entry_index)
            {
                return (_Metadata._Index == entry_index);
            }

            public bool IsEmpty()
            {
                return (_Values.Count == 0);
            }
        }

        #endregion
    }
}
