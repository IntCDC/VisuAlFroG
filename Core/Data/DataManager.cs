using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Core.Abstracts;
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
            // public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate(GenericDataStructure ouput_data);

            /// <summary>
            /// 
            /// </summary>
            public delegate void SendOutputData_Callback();

            /// <summary>
            /// Callback for visualizations to request suitable data
            /// </summary>
            public delegate object RequestCallback_Delegate(Type data_type);

            /// <summary>
            /// Callback to register callback for getting notified on any data update
            /// </summary>
            public delegate void RegisterUpdateTypeCallback_Delegate(UpdateCallback_Delegate update_callback, Type data_type);
            public delegate void UnregisterUpdateCallback_Delegate(UpdateCallback_Delegate update_callback);

            /// <summary>
            /// Callback called on update data
            /// </summary>
            public delegate void UpdateCallback_Delegate(bool new_data);


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();


                _update_callbacks = new List<UpdateCallback_Delegate>();

                _data_library = new Dictionary<Type, IDataType>();
                var variety_generic = new DataTypeGeneric(event_metadata_changed);
                _data_library.Add(variety_generic.GetType(), variety_generic);

                bool initialized = true;


                _timer.Stop();
                _initialized = initialized;
                return _initialized;
            }

            public override bool Terminate()
            {
                bool terminated = true;
                if (_initialized)
                {
                    _outputdata_callback = null;

                    _data_library.Clear();
                    _data_library = null;

                    _update_callbacks.Clear();
                    _update_callbacks = null;

                    _initialized = false;
                }
                return terminated;
            }

            /// <summary>
            /// Callback to propagate new input data to the data manager.
            /// </summary>
            /// <param name="input_data">Reference to the new input data.</param>
            public void UpdateInputData(GenericDataStructure input_data)
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
                if (_data_validator.Convert(input_data, out GenericDataStructure validated_data))
                {
                    foreach (var pair in _data_library)
                    {
                        pair.Value.UpdateData(validated_data);
                    }
                    foreach (var update_callback in _update_callbacks)
                    {
                        update_callback(true);
                    }
                }
                Log.Default.Msg(Log.Level.Info, "...done.");

                _timer.Stop();
            }

            /// <summary>
            /// Set the callback to provide new output data to the interface.
            /// </summary>
            /// <param name="outputdata_callback"></param>
            public void SetOutputDataCallback(OutputData_Delegate outputdata_callback)
            {
                _outputdata_callback = outputdata_callback;
            }

            /// <summary>
            /// Register update callback of calling visualization.
            /// </summary>
            public void RegisterUpdateTypeCallback(UpdateCallback_Delegate update_callback, Type data_type)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }

                if (_update_callbacks.Contains(update_callback))
                {
                    Log.Default.Msg(Log.Level.Debug, "Callback for update data already registered");
                    return;
                }
                _update_callbacks.Add(update_callback);


                if (!_data_library.ContainsKey(data_type))
                {
                    var variety = (IDataType)Activator.CreateInstance(data_type, (PropertyChangedEventHandler)event_metadata_changed);
                    if (variety == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Expected data type of IDataVariety but received: " + data_type.FullName);
                        return;
                    }
                    _data_library.Add(variety.GetType(), variety);
                    Log.Default.Msg(Log.Level.Info, "Added new data type: " + data_type.FullName);

                    // Load data if available
                    var data = get_generic_data(true);
                    if (data != null)
                    {
                        variety.UpdateData(data);
                    }
                }
            }

            /// <summary>
            /// Unregister data update callback of calling visualization.
            /// XXX TODO Track used data types for being able to delete them is unused.
            /// </summary>
            public void UnregisterUpdateCallback(UpdateCallback_Delegate update_callback)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }

                if (_update_callbacks.Contains(update_callback))
                {
                    _update_callbacks.Remove(update_callback);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Callback for update data already removed");
                }
            }

            /// <summary>
            /// Return the data for the requested type.
            /// </summary>
            /// <param name="t">The type the data would be required.</param>
            /// <returns>The data as generic object. Cast to requested type manually.</returns>
            public object RequestDataCallback(Type data_type)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (!_data_library.ContainsKey(data_type))
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data type: " + data_type.FullName);
                    return null;

                }
                return _data_library[data_type]._Get;
            }

            /// <summary>
            /// Send changed output data to interface
            /// </summary>
            public void SendOutputData()
            {
                if (_outputdata_callback != null)
                {
                    var data = get_generic_data();
                    if (data != null)
                    {
                        var metadata_list = data.ListMetaData();
                        var out_data = new GenericDataStructure();
                        foreach (var meta_data in metadata_list)
                        {
                            if (meta_data._Selected)
                            {
                                var metadata_entry = new GenericDataEntry();
                                metadata_entry.AddValue(meta_data._Selected);
                                metadata_entry.AddValue(meta_data._Index);
                                out_data.AddEntry(metadata_entry);
                            }
                        }
                        _outputdata_callback(out_data);
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback for sending output data");
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Callback provided for getting notified on changed meta data 
            /// !!! This function is currently called for every single change !!!
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The property changed event arguments.</param>
            private void event_metadata_changed(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as IMetaData;
                if ((sender_selection == null) || (e.PropertyName != "_Selected"))
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown sender");
                    return;
                }

                try
                {
                    // Update meta data in all data series
                    foreach (var pair in _data_library)
                    {
                        pair.Value.UpdateMetaDataEntry(sender_selection);
                    }

                    // Notify visualizations via registered update callbacks
                    if (_update_callbacks == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "No callback list for registered update callbacks ");
                        return;
                    }
                    foreach (var update_callback in _update_callbacks)
                    {
                        update_callback(false);
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                    return;
                }
            }

            private GenericDataStructure get_generic_data(bool silent = false)
            {
                // Get current data from generic reference
                if (!_data_library.ContainsKey(typeof(DataTypeGeneric)))
                {
                    if (!silent)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing generic data");
                    }
                    return null;
                }
                var data = _data_library[typeof(DataTypeGeneric)]._Get as GenericDataStructure;
                // If there is already data present, create the data for the new type
                if (data == null)
                {
                    if (!silent)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing generic data");
                    }
                }
                return data;
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private Dictionary<Type, IDataType> _data_library = null;
            private OutputData_Delegate _outputdata_callback = null;
            private List<UpdateCallback_Delegate> _update_callbacks = null;

            private DataValidator _data_validator = new DataValidator();
        }
    }
}
