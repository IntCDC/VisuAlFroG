using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

using Core.Data;
using Core.GUI;
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
            #region public properties

            public static uint MultiDimensional { get; } = uint.MaxValue;

            public object _Get { get { return _data; } }
            public uint _Dimension { get; protected set; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractDataType(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler)
            {
                _update_data_handler = update_data_handler;
                _update_metadata_handler = update_metadata_handler;
            }

            public abstract void UpdateData(GenericDataStructure data);

            public abstract void UpdateMetaDataEntry(IMetaData updated_meta_data);

            public abstract List<MenuItem> GetMenu();

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected DataType _data = default(DataType);
            protected bool _loaded = false;
            protected PropertyChangedEventHandler _update_data_handler = null;
            protected PropertyChangedEventHandler _update_metadata_handler = null;

            #endregion
        }
    }
}
