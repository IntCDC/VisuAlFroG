using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Model.DataSeries;
using Visualizations.Interaction;
using System.Windows.Markup;
using SciChart.Data.Model;
using System.Windows.Controls;
using System.Windows;
using Visualizations.Data;
using Visualizations.Abstracts;
using SciChart.Charting.Visuals;



/*
 * Data Manager
 * 
 */
namespace Visualizations
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
            public delegate void RegisterUpdateCallback_Delegate(UpdateCallback_Delegate update_callback);

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

                bool initialized = true;


                _updated_callbacks = new List<UpdateCallback_Delegate>();

                // Register all supported data varieties
                _data_library = new Dictionary<Type, IDataVariety>();

                var variety_generic = new DataVarietyGeneric();
                _data_library.Add(variety_generic.Variety, variety_generic);

                var variety_fastline = new DataVarietySciChartSeries<SciChart.Charting.Visuals.RenderableSeries.FastLineRenderableSeries>();
                _data_library.Add(variety_fastline.Variety, variety_fastline);

                var variety_fastcolumn = new DataVarietySciChartSeries<SciChart.Charting.Visuals.RenderableSeries.FastColumnRenderableSeries>();
                _data_library.Add(variety_fastcolumn.Variety, variety_fastcolumn);

                var variety_xyscatter = new DataVarietySciChartSeries<SciChart.Charting.Visuals.RenderableSeries.XyScatterRenderableSeries>();
                _data_library.Add(variety_xyscatter.Variety, variety_xyscatter);

                //var variety_parallel = new DataVarietySciChartParallel<T>();
                //_data_library.Add(variety_parallel.DataVariety, variety_parallel);

                /// TODO Add more library data formats here ...


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
                if (input_data == null)
                {
                    Log.Default.Msg(Log.Level.Warn, "No input data available");
                    return;
                }
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return;
                }
                _timer.Start();
                Log.Default.Msg(Log.Level.Info, "Reading input data ...");


                // Collect information on input data 
                var dimensionality = input_data.Dimensionality();
                var value_types = input_data.ValueTypes();
                // Initialize meta data
                int index = 0;
                init_metadata(input_data, ref index);

                // Update all data 
                foreach (var pair in _data_library)
                {
                    pair.Value.Update(ref input_data, dimensionality, value_types);
                }


                // Notify visualizations via registered update callbacks
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
            public void RegisterUpdateCallback(UpdateCallback_Delegate update_callback)
            {
                if (_updated_callbacks.Contains(update_callback))
                {
                    Log.Default.Msg(Log.Level.Debug, "callback for updated data already registered");
                    return;
                }
                _updated_callbacks.Add(update_callback);
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
                if (_data_library.ContainsKey(data_type))
                {
                    return _data_library[data_type].Get;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data type: " + data_type.FullName);
                }
                return null;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Callback provided for getting notified on changed meta data 
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The property changed event arguments.</param>
            private void metadata_changed(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as MetaData;
                if ((sender_selection == null) || (e.PropertyName != "IsSelected"))
                {
                    Log.Default.Msg(Log.Level.Error, "Unknown sender");
                    return;
                }
                int index = sender_selection.Index;

                // Use GenericDataStructure as reference ...
                GenericDataStructure data = null;
                GenericDataEntry entry = null;
                try
                {
                    data = _data_library[typeof(GenericDataStructure)].Get as GenericDataStructure;
                    entry = data.EntryAtIndex(index);
                    if (data == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing data");
                        return;
                    }
                    if (entry == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Missing data entry");
                        return;
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                    return;
                }
                //...to update meta data:
                foreach (var pair in _data_library)
                {
                    pair.Value.UpdateEntryAtIndex(index, entry);
                }

                // Notify visualizations via registered update callbacks
                foreach (var updated_callback in _updated_callbacks)
                {
                    updated_callback(false);
                }

                // Send changed output data
                /*
                if (_outputdata_callback != null)
                {
                    var metadata_list = data.ListMetaData();

                    var out_data = new GenericDataStructure();
                    foreach (var meta_data in metadata_list)
                    {
                        var metadata_entry = new GenericDataEntry();
                        metadata_entry.AddValue(meta_data.IsSelected);
                        metadata_entry.AddValue(meta_data.Index);

                        out_data.AddEntry(metadata_entry);
                    }
                    _outputdata_callback(ref out_data);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Missing callback to propagate updated output data.");
                }
                */
            }

            /// <summary>
            /// Recursively initialize meta data.
            /// </summary>
            /// <param name="data">The data branch.</param>
            /// <param name="index">The entry index</param>
            private void init_metadata(GenericDataStructure data, ref int entry_index)
            {
                foreach (var entry in data.Entries)
                {
                    entry.MetaData.IsSelected = false;
                    entry.MetaData.Index = entry_index;
                    entry.MetaData.PropertyChanged += metadata_changed;

                    entry_index++;
                }
                foreach (var branch in data.Branches)
                {
                    init_metadata(branch, ref entry_index);
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Dictionary<Type, IDataVariety> _data_library = null;

            private OutputData_Delegate _outputdata_callback = null;
            private List<UpdateCallback_Delegate> _updated_callbacks = null;
        }
    }
}
