using System;
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

            public DataTypeGeneric(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler, DataManager.GetSendOutputCallback_Delegate send_output_callback)
                : base(update_data_handler, update_metadata_handler, send_output_callback) { }

            public override void UpdateData(GenericDataStructure data)
            {
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return;
                }

                _data_generic = null;
                _data_specific = null;
                _loaded = false;

                _Dimension = data.GetDimension();
                _data_generic = data.DeepCopy();
                _data_specific = _data_generic;
                init_metadata(_data_specific);

                _loaded = true;
            }

            public override List<Control> GetDataMenu()
            {
                return base.GetDataMenu();
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
