using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Ports;
using System.Linq;
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

            public delegate void TriggerSetDataCallback_Delegate();
            public delegate object GetDataCallback_Delegate(int data_uid);
            public delegate void SetDataCallback_Delegate(GenericDataStructure ouput_data);

            public delegate int RegisterDataCallback_Delegate(UpdateVisualizationCallback_Delegate update_callback, Type data_type);
            public delegate void UnregisterUpdateCallback_Delegate(int data_uid);

            /// <summary>
            /// Callback called on update data
            /// </summary>
            public delegate void UpdateVisualizationCallback_Delegate(bool new_data);


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();



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
                    _original_data = null;
                    _outputdata_callback = null;
                    _data_library.Clear();

                    _initialized = false;
                }
                return terminated;
            }

            /// <summary>
            /// Callback to propagate new input data to the data manager.
            /// </summary>
            /// <param name="input_data">The new input data.</param>
            public void UpdateData(GenericDataStructure input_data)
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
                    _original_data = validated_data;

                    /*
                    if (validated_data.Transpose(out GenericDataStructure tranposed_data)) {
                        validated_data = tranposed_data;
                    }
                    */
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
            /// Set the callback to provide new output data to the interface.
            /// </summary>
            /// <param name="outputdata_callback"></param>
            public void SetOutputDataCallback(SetDataCallback_Delegate outputdata_callback)
            {
                _outputdata_callback = outputdata_callback;
            }

            /// <summary>
            /// Register update callback of calling visualization.
            /// </summary>
            public int RegisterDataCallback(UpdateVisualizationCallback_Delegate update_callback, Type data_type)
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
                    var variety = (IDataType)Activator.CreateInstance(data_type, (PropertyChangedEventHandler)event_metadata_changed, (PropertyChangedEventHandler)event_data_changed);
                    if (variety == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Expected data type of IDataVariety but received: " + data_type.FullName);
                        return UniqueID.InvalidInt;
                    }

                    _data_library.Add(UniqueID.GenerateInt(), new DataDescription(update_callback, variety));
                    Log.Default.Msg(Log.Level.Info, "Added new data type: " + data_type.FullName);

                    // Load data if available
                    if (_original_data != null)
                    {
                        variety.UpdateData(_original_data);
                    }

                    return _data_library.Last().Key;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Omitted data creation for data type 'null'");
                    return UniqueID.InvalidInt;
                }
            }

            /// <summary>
            /// Unregister data update callback of calling visualization.
            /// XXX TODO Track used data types for being able to delete them is unused.
            /// </summary>
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
                else
                {
                    Log.Default.Msg(Log.Level.Debug, "Data already unregistered for data UID: " + data_uid.ToString());
                }
            }

            /// <summary>
            /// Return the data for the requested type.
            /// </summary>
            /// <param name="t">The type the data would be required.</param>
            /// <returns>The data as generic object. Cast to requested type manually.</returns>
            public object GetDataCallback(int data_uid)
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
                return _data_library[data_uid]._Data._Get;
            }

            /// <summary>
            /// 
            /// </summary>
            public bool SaveData()
            {
                /// TODO
                Log.Default.Msg(Log.Level.Warn, "Saving data from file is not yet implemented...");
                return true;

                string csv_data_string = CSV_DataConverter.ConvertToCSV(_original_data);
                FileDialogHelper.Save(csv_data_string, "Save Data", "CSV files (*.csv)|*.csv", ResourcePaths.CreateFileName("data", "csv"));
            }

            /// <summary>
            /// Load CSV formatted data from a file
            /// </summary>
            public bool LoadData()
            {
                /// TODO
                Log.Default.Msg(Log.Level.Warn, "Loading data from file is not yet implemented...");
                return true;


                string data_file = FileDialogHelper.Load("Load Data", "CSV files (*.csv)|*.csv", ResourcePaths.CreateFileName("data", "csv"));
                try
                {
                    var fileStream = new FileStream(data_file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        Log.Default.Msg(Log.Level.Info, "Loading data from file: '" + data_file + "'");

                        string csv_data = "";
                        var input_data = CSV_DataConverter.ConvertFromCSV(csv_data);
                        UpdateData(input_data);

                        return true;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return false;
            }

            /// <summary>
            /// Send changed output data to interface
            /// </summary>
            public bool SendData()
            {
                var out_data = new GenericDataStructure();
                if (_original_data != null)
                {
                    var metadata_list = _original_data.ListMetaData();
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

                }
                if (_outputdata_callback != null)
                {
                    _outputdata_callback(out_data);
                }
                else
                {
                    ///Log.Default.Msg(Log.Level.Warn, "Missing callback for sending output data");

                    /// TODO Ask to save output data to file
                    Log.Default.Msg(Log.Level.Warn, "Saving output data from file is not yet implemented...");
                    return false;

                    string csv_data_string = CSV_DataConverter.ConvertToCSV(out_data);
                    FileDialogHelper.Save(csv_data_string, "Save Output Data", "CSV files (*.csv)|*.csv", ResourcePaths.CreateFileName("output_data", "csv"));
                }
                return true;
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
                        pair.Value._Data.UpdateMetaDataEntry(sender_selection);

                        pair.Value._UpdateVisualization(false);
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                    return;
                }
            }

            /// <summary>
            /// Callback provided for getting notified on changed meta data 
            /// !!! This function is currently called for every single change !!!
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
                try
                {
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
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                    return;
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

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

            GenericDataStructure _original_data = null;
            private SetDataCallback_Delegate _outputdata_callback = null;

            private Dictionary<int, DataDescription> _data_library = new Dictionary<int, DataDescription>();
            private DataValidator _data_validator = new DataValidator();
        }
    }
}
