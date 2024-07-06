using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
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

            public int _UID { get; } = UniqueID.GenerateInt();
            public object _Specific { get { return _data_specific; } }
            public GenericDataStructure _Generic { get { return _data_generic; } }
            public uint _Dimension { get; protected set; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractDataType(PropertyChangedEventHandler update_data_handler, PropertyChangedEventHandler update_metadata_handler, DataManager.GetSendOutputCallback_Delegate send_output_callback)
            {
                if ((update_data_handler == null) || (update_metadata_handler == null)) // ||(send_output_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing handler(s) or callback(s)");
                    return;
                }
                _update_data_handler = update_data_handler;
                _update_metadata_handler = update_metadata_handler;
                _send_output_callback = send_output_callback;
            }

            public abstract void UpdateData(GenericDataStructure data);

            public abstract void UpdateMetaDataEntry(IMetaData updated_meta_data);

            public virtual List<Control> GetMenu()
            {
                var menu_items = new List<Control>();
                var send_menu_item = MenubarWindow.GetDefaultMenuItem("Output to External Interface ", menu_ouput_interface);
                send_menu_item.IsEnabled = (_send_output_callback != null);
                menu_items.Add(send_menu_item);

                var save_menu_item = MenubarWindow.GetDefaultMenuItem("Save (.csv) ", menu_save_data);
                menu_items.Add(save_menu_item);

                return menu_items;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions 

            private bool menu_ouput_interface()
            {
                if (_send_output_callback != null)
                {
                    _send_output_callback(_UID, false);
                    return true;
                }
                return false;
            }

            private bool menu_save_data()
            {
                if (CSV_DataHandling.ConvertToCSV(_data_generic, out string csv_data_string))
                {
                    return FileDialogHelper.Save(csv_data_string, "Save Output Data", "CSV files (*.csv)|*.csv", ResourcePaths.CreateFileName("output_data", "csv"));
                }
                return false;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected DataType _data_specific = default(DataType);
            // Keeping local copy of generic data version for sending to output, manipulation, and filtering
            protected GenericDataStructure _data_generic = null;

            protected bool _loaded = false;
            protected PropertyChangedEventHandler _update_data_handler = null;
            protected PropertyChangedEventHandler _update_metadata_handler = null;
            protected DataManager.GetSendOutputCallback_Delegate _send_output_callback = null;

            #endregion
        }
    }
}
