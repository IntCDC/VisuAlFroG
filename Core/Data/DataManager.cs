using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Core.Abstracts;
using Core.GUI;
using Core.Utilities;



/*
 * Data Manager
 * 
 */
namespace Core
{
    namespace Data
    {
        public class DataManager : AbstractService
        {
            /* ------------------------------------------------------------------*/
            #region public delegates

            public delegate object GetSpecificDataCallback_Delegate(int data_uid);
            public delegate List<System.Windows.Controls.Control> GetDataMenuCallback_Delegate(int data_uid);

            public delegate void GetSendOutputCallback_Delegate(int data_uid, bool only_selected);
            public delegate void SetDataCallback_Delegate(GenericDataStructure ouput_data);

            public delegate GenericDataStructure GetGenericDataCallback_Delegate(int data_uid);
            public delegate void UpdateFilteredDataCallback_Delegate(int data_uid, GenericDataStructure input_data);

            public delegate int RegisterDataCallback_Delegate(Type data_type, UpdateVisualizationCallback_Delegate update_callback);
            public delegate void UnregisterUpdateCallback_Delegate(int data_uid);

            public delegate void UpdateVisualizationCallback_Delegate(bool new_data);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public virtual bool Initialize(PropertyChangedEventHandler event_entry_selection_changed)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _event_metadata_changed = event_entry_selection_changed;

                // Add copy of data that is kept for reference as the original data unmodified
                var variety = (IDataType)Activator.CreateInstance(typeof(DataTypeGeneric),
                    (PropertyChangedEventHandler)event_data_changed,
                    (PropertyChangedEventHandler)_event_metadata_changed,
                    ((_outputdata_callback != null) ? ((GetSendOutputCallback_Delegate)callback_send_output) : (null)));

                _data_library.Add(UniqueID.GenerateInt(), new DataDescription(((bool new_data) => { }), variety));
                _original_data_hash = _data_library.Last().Key;


                _timer.Stop();
                _initialized = true;
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    UnregisterDataCallback(_original_data_hash);
                    _original_data_hash = UniqueID.InvalidInt;

                    _outputdata_callback = null;
                    _data_library.Clear();

                    _initialized = false;
                }
                return true;
            }

            /// <summary>
            /// Callback to propagate new input data to the data manager.
            /// </summary>
            /// <param name="input_data">The new input data.</param>
            public void UpdateAllDataCallback(GenericDataStructure input_data)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }
                if (input_data == null)
                {
                    Log.Default.Msg(Log.Level.Warn, "No input data available");
                    return;
                }
                _timer.Start();

                Log.Default.Msg(Log.Level.Info, "Processing new input data...");
                if (DataValidator.Convert(input_data, out GenericDataStructure validated_data))
                {
                    foreach (var pair in _data_library)
                    {
                        pair.Value._Data.UpdateData(validated_data);
                        pair.Value._UpdateVisualization(true);
                    }
                }
                Log.Default.Msg(Log.Level.Info, "...done.");

                _timer.Stop();
            }

            /// <summary>
            /// Callback to propagate new input data to selected data types in the data manager.
            /// </summary>
            /// <param name="input_data">The new input data.</param>
            public void UpdateFilteredDataCallback(int data_uid, GenericDataStructure input_data)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }
                if (input_data == null)
                {
                    Log.Default.Msg(Log.Level.Warn, "No input data available");
                    return;
                }

                if (!_data_library.ContainsKey(data_uid))
                {
                    Log.Default.Msg(Log.Level.Error, "Received unknown data UID");
                }
                _data_library[data_uid]._Data.UpdateData(input_data);
                _data_library[data_uid]._UpdateVisualization(true);
            }

            /// <summary>
            /// Set the callback to provide new output data to the interface.
            /// </summary>
            /// <param name="outputdata_callback"></param>
            public void SetOutputDataCallback(SetDataCallback_Delegate outputdata_callback)
            {
                _outputdata_callback = outputdata_callback;
            }

            /// <summary>
            /// Register data of calling visualization.
            /// </summary>
            public int RegisterDataCallback(Type data_type, UpdateVisualizationCallback_Delegate update_callback)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return UniqueID.InvalidInt;
                }

                if (update_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Update callback for visualization is NULL.");
                    return UniqueID.InvalidInt;
                }

                if (data_type != null)
                {
                    var variety = (IDataType)Activator.CreateInstance(data_type,
                        (PropertyChangedEventHandler)event_data_changed,
                        (PropertyChangedEventHandler)_event_metadata_changed,
                        ((_outputdata_callback != null) ? ((GetSendOutputCallback_Delegate)callback_send_output) : (null)));

                    if (variety == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Expected data type of IDataVariety but received: " + data_type.FullName);
                        return UniqueID.InvalidInt;
                    }

                    _data_library.Add(variety._UID, new DataDescription(update_callback, variety));
                    Log.Default.Msg(Log.Level.Info, "Added new data type: " + data_type.FullName);

                    // Load original data if available
                    var original_data = (GenericDataStructure)GetSpecificDataCallback(_original_data_hash);
                    if ((original_data != null) && (!original_data.IsEmpty()))
                    {
                        variety.UpdateData(original_data);
                    }

                    return variety._UID;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Omitted data creation for data type 'null'");
                    return UniqueID.InvalidInt;
                }
            }

            /// <summary>
            /// Unregister data of calling visualization.
            /// </summary>
            /// <param name="data_uid">The UID of the data to be deleted.</param>
            public void UnregisterDataCallback(int data_uid)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }

                if (_data_library.ContainsKey(data_uid))
                {
                    _data_library.Remove(data_uid);
                    Log.Default.Msg(Log.Level.Debug, "Unregistered data for UID: " + data_uid.ToString());
                }
                else if (data_uid == UniqueID.InvalidInt)
                {
                    /// do nothing...
                }
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Data already unregistered for data UID: " + data_uid.ToString());
                }
            }

            /// <summary>
            /// Return the data type specific data for the requested type.
            /// </summary>
            /// <param name="data_uid">The UID of the requested data.</param>
            /// <returns>The data as generic object. Cast to requested type manually.</returns>
            public object GetSpecificDataCallback(int data_uid)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (data_uid == UniqueID.InvalidInt)
                {
                    // Silently continue
                    Log.Default.Msg(Log.Level.Debug, "Requested data for invalid UID.");
                    return null;
                }

                if (!_data_library.ContainsKey(data_uid))
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data UID: " + data_uid.ToString());
                    return null;

                }
                return _data_library[data_uid]._Data._Specific;
            }

            public GenericDataStructure GetGenericDataCallback(int data_uid)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (data_uid == UniqueID.InvalidInt)
                {
                    // Silently continue
                    Log.Default.Msg(Log.Level.Debug, "Requested data for invalid UID.");
                    return null;
                }

                if (!_data_library.ContainsKey(data_uid))
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data UID: " + data_uid.ToString());
                    return null;

                }
                return _data_library[data_uid]._Data._Generic;
            }

            /// <summary>
            /// Return the data menu for the requested type
            /// </summary>
            /// <param name="data_uid"></param>
            /// <returns></returns>
            public List<System.Windows.Controls.Control> GetDataMenuCallback(int data_uid)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (!_data_library.ContainsKey(data_uid))
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data UID: " + data_uid.ToString());
                    return null;

                }
                return _data_library[data_uid]._Data.GetDataMenu();
            }

            public override void AttachMenu(MenubarMain menu_bar)
            {
                var save_menu_item = MenubarMain.GetDefaultMenuItem("Save (.csv)");
                save_menu_item.Click += (object sender, RoutedEventArgs e) =>
                {
                    var sender_content = sender as System.Windows.Controls.MenuItem;
                    if (sender_content == null)
                    {
                        return;
                    }
                    CSV_DataHandling.SaveToFile((GenericDataStructure)GetSpecificDataCallback(_original_data_hash));
                };
                menu_bar.AddMenu(MenubarMain.PredefinedMenuOption.DATA, save_menu_item);

                var load_menu_item = MenubarMain.GetDefaultMenuItem("Load (.csv)");
                load_menu_item.Click += (object sender, RoutedEventArgs e) =>
                {
                    var sender_content = sender as System.Windows.Controls.MenuItem;
                    if (sender_content == null)
                    {
                        return;
                    }
                    if (CSV_DataHandling.LoadFromFile(out GenericDataStructure data))
                    {
                        UpdateAllDataCallback(data);
                    }
                };
                menu_bar.AddMenu(MenubarMain.PredefinedMenuOption.DATA, load_menu_item);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            /// <summary>
            /// Send changed output data to interface
            /// </summary>
            public void callback_send_output(int data_uid, bool only_selected)
            {
                if (!_data_library.ContainsKey(data_uid))
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data menu not available for given data UID: " + data_uid.ToString());
                    return;

                }
                var out_data = _data_library[data_uid]._Data._Generic;
                if ((out_data != null) && (!out_data.IsEmpty()))
                {
                    if (only_selected)
                    {
                        var out_data_selected = new GenericDataStructure();

                        /// TODO

                        out_data = out_data_selected;
                    }
                    if (_outputdata_callback != null)
                    {
                        _outputdata_callback(out_data);
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Warn, "No callback available to send the output data.");
                    }
                }
            }

            /// <summary>
            /// Callback provided for getting notified on changes that should be applied globally
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The property changed event arguments.</param>
            private void event_data_changed(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as DataModifier;
                if (sender_selection == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown sender");
                    return;
                }
                // Update data entry in all data series
                foreach (var pair in _data_library)
                {
                    /// TODO 
                    switch (e.PropertyName)
                    {
                        case (DataModifier.ADD):

                            break;
                        case (DataModifier.DELETE):

                            break;
                        case (DataModifier.CHANGE):

                            break;
                        default: break;

                    }
                    pair.Value._UpdateVisualization(false);
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private struct DataDescription
            {
                public DataDescription(UpdateVisualizationCallback_Delegate update_vis_callback, IDataType data)
                {
                    _UpdateVisualization = update_vis_callback;
                    _Data = data;
                }
                public UpdateVisualizationCallback_Delegate _UpdateVisualization { get; private set; }
                public IDataType _Data { get; private set; }
            }

            private int _original_data_hash = UniqueID.InvalidInt;
            private SetDataCallback_Delegate _outputdata_callback = null;
            private Dictionary<int, DataDescription> _data_library = new Dictionary<int, DataDescription>();

            private PropertyChangedEventHandler _event_metadata_changed = null;

            #endregion
        }
    }
}
