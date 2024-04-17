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
            /// Function provided by the data manager for passing on the input data to the visualizations
            /// </summary>
            public delegate void InputData_Delegate(ref GenericDataStructure input_data);

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate(ref GenericDataStructure ouput_data);

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
            /// Callback called on updated data
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


                _updated_callbacks = new List<UpdateCallback_Delegate>();
                _data_library = new Dictionary<Type, IDataType>();
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

                    _updated_callbacks.Clear();
                    _updated_callbacks = null;

                    _initialized = false;
                }
                return terminated;
            }

            /// <summary>
            /// Callback to propagate new input data to the data manager.
            /// </summary>
            /// <param name="input_data">Reference to the new input data.</param>
            public void GetInputDataCallback(ref GenericDataStructure input_data)
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
                Log.Default.Msg(Log.Level.Info, "Reading input data ...");

                // Update generic data type...
                /// -> Initializes meta data!
                if (!_data_library.ContainsKey(typeof(DataTypeGeneric)))
                {
                    var variety_generic = new DataTypeGeneric(event_metadata_changed);
                    _data_library.Add(variety_generic.GetType(), variety_generic);
                }
                _data_library[typeof(DataTypeGeneric)].Initialize(ref input_data, input_data.DataDimension(), input_data.ValueTypes());
                var data = get_generic_data();

                // ...then update all other data types
                foreach (var pair in _data_library)
                {
                    if (pair.Key != typeof(DataTypeGeneric))
                    {
                        pair.Value.Initialize(ref input_data, data.DataDimension(), data.ValueTypes());
                    }
                }

                // Notify visualizations via registered update callbacks
                if (_updated_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "No callback list for registered update callbacks");
                    return;
                }
                foreach (var updated_callback in _updated_callbacks)
                {
                    updated_callback(true);
                }

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
            /// Notify registered callers on updated input data. Called by visualizations.
            /// </summary>
            public void RegisterUpdateTypeCallback(UpdateCallback_Delegate update_callback, Type data_type)
            {
                // Register update callback of calling visualization
                if (_updated_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "No callback list for registered update callbacks");
                    return;
                }
                if (_updated_callbacks.Contains(update_callback))
                {
                    Log.Default.Msg(Log.Level.Debug, "Callback for updated data already registered");
                    return;
                }
                _updated_callbacks.Add(update_callback);

                // Register data type of calling visualization
                if (!_data_library.ContainsKey(data_type))
                {
                    // Create new data variety
                    var variety = (IDataType)Activator.CreateInstance(data_type, (PropertyChangedEventHandler)event_metadata_changed);
                    if (variety == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Expected data type of IDataVariety but received: " + data_type.FullName);
                        return;
                    }
                    _data_library.Add(variety.GetType(), variety);

                    var data = get_generic_data();
                    if (data != null)
                    {
                        variety.Initialize(ref data, data.DataDimension(), data.ValueTypes());
                        Log.Default.Msg(Log.Level.Info, "Added new data type: " + data_type.FullName);
                    }
                }
            }

            /// <summary>
            /// Notify registered callers on updated input data. Called by visualizations.
            /// </summary>
            public void UnregisterUpdateCallback(UpdateCallback_Delegate update_callback)
            {
                // Unregister data update callback of calling visualization
                if (_updated_callbacks == null)
                {
                    Log.Default.Msg(Log.Level.Error, "No callback list for registered update callbacks");
                    return;
                }
                if (_updated_callbacks.Contains(update_callback))
                {
                    _updated_callbacks.Remove(update_callback);
                    return;
                }
                Log.Default.Msg(Log.Level.Debug, "Callback for updated data already removed");

                /// XXX Do not unregister data type since it might be used by other visualization(s) or later when visualization is created again...
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
                return _data_library[data_type].Get;
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
                if ((sender_selection == null) || (e.PropertyName != "IsSelected"))
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown sender");
                    return;
                }
                int index = sender_selection.Index;

                try
                {
                    // Update meta data in all data series
                    foreach (var pair in _data_library)
                    {
                        pair.Value.UpdateMetaDataEntry(sender_selection);
                    }

                    // Notify visualizations via registered update callbacks
                    if (_updated_callbacks == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "No callback list for registered update callbacks ");
                        return;
                    }
                    foreach (var updated_callback in _updated_callbacks)
                    {
                        updated_callback(false);
                    }





                    // Send changed output data to interface ---------------------
                    /// TODO XXX Call only once per selection
                    if (_outputdata_callback != null)
                    {
                        var data = get_generic_data();
                        if (data != null)
                        {
                            var metadata_list = data.ListMetaData();
                            var out_data = new GenericDataStructure();
                            foreach (var meta_data in metadata_list)
                            {
                                if (meta_data.IsSelected)
                                {
                                    var metadata_entry = new GenericDataEntry();
                                    metadata_entry.AddValue(meta_data.IsSelected);
                                    metadata_entry.AddValue(meta_data.Index);
                                    out_data.AddEntry(metadata_entry);
                                }
                            }
                            _outputdata_callback(ref out_data);
                        }
                    }
                    // ---------------------------------------------------------
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                    return;
                }
            }

            private GenericDataStructure get_generic_data()
            {
                // Get current data from generic reference
                if (!_data_library.ContainsKey(typeof(DataTypeGeneric)))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing generic data");
                    return null;
                }
                var data = _data_library[typeof(DataTypeGeneric)].Get as GenericDataStructure;
                // If there is already data present, create the data for the new type
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing generic data");
                }
                return data;
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private Dictionary<Type, IDataType> _data_library = null;

            private OutputData_Delegate _outputdata_callback = null;
            private List<UpdateCallback_Delegate> _updated_callbacks = null;
        }
    }
}
