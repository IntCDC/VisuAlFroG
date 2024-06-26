﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;
using System.ComponentModel;
using Core.GUI;



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
            #region public functions

            public DataTypeGeneric(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler)
                : base(update_data_handler, update_metadata_handler) { }

            public override void UpdateData(GenericDataStructure data)
            {
                _loaded = false;
                _data = null;

                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }
                _Dimension = data.GetDimension();

                _data = data.DeepCopy(_update_metadata_handler);
                init_metadata(_data);

                _loaded = true;
            }

            public override void UpdateMetaDataEntry(IMetaData updated_meta_data)
            {
                if (!_loaded)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of data required prior to execution");
                    return;
                }
                var entry = _data.GetEntryAtIndex(updated_meta_data._Index);
                if (entry != null)
                {
                    entry._Metadata._Selected = updated_meta_data._Selected;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Can not find data entry at index: " + updated_meta_data._Index.ToString());
                }
            }

            public override List<MenuItem> GetMenu()
            {
                /// XXX TODO Only for testing - this should go or be set by DataFilter

                var menu_list = new List<MenuItem>();
                if (_data != null)
                {
                    var transpose_item = ContentMenuBar.GetDefaultMenuItem("Transpose", _data.Transpose);
                    menu_list.Add(transpose_item);
                }
                return menu_list;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void init_metadata(GenericDataStructure data)
            {
                foreach (var entry in data._Entries)
                {
                    entry._Metadata.PropertyChanged += _update_metadata_handler;
                }
                foreach (var branch in data._Branches)
                {
                    init_metadata(branch);
                }
            }

            #endregion
        }
    }
}
