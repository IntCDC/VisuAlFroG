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
            public delegate object RequestCallback_Delegate(Type t);

            /// <summary>
            /// Callback to register callback for getting notified on any data update
            /// </summary>
            public delegate void RegisterUpdateCallback_Delegate(UpdateCallback_Delegate update_callback);

            /// <summary>
            /// Callback called on updated data
            /// </summary>
            public delegate void UpdateCallback_Delegate(); 


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                bool initilized = true;

                _updated_callbacks = new List<UpdateCallback_Delegate>();

/*
                // Register all supported data types
                _library_data = new Dictionary<Type, Tuple<int, object>>();
                // GenericDataBranch (mandatory)
                _library_data.Add(typeof(GenericDataStructure), new Tuple<int, object>(intmax, new GenericDataStructure()));

                // SciChartUniformData (optional)
                var data_scichart_uniform = new SciChartUniformData();
                data_scichart_uniform.SeriesName = "SciChartUniformData";
                _library_data.Add(typeof(SciChartUniformData), data_scichart_uniform);

                // SciChartData (optional)
                var data_scichart = new SciChartData();
                data_scichart.SeriesName = "SciChartData";
                _library_data.Add(typeof(SciChartData), data_scichart);

                /// TODO Add more library data formats here ...
*/

                _timer.Stop();
                _initialized = initilized;
                return _initialized;
            }

            public override bool Terminate()
            {
                bool terminated = true;
                if (_initialized)
                {
                    _outputdata_callback = null;

                    /*
                    foreach (var data_type in _library_data)
                    {
                        if (data_type.Key == typeof(GenericDataStructure))
                        {
                            /// Nothing to do
                        }
                        else if (data_type.Key == typeof(SciChartUniformData))
                        {
                            ((SciChartUniformData)data_type.Value).Clear();
                        }
                        else if (data_type.Key == typeof(SciChartData))
                        {
                            ((SciChartData)data_type.Value).Clear();
                        }
                        else
                        {
                            /// TODO Add more library data formats here ...
                            Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                        }
                    }
                    _library_data.Clear();
                    _library_data = null;
                    */
                    _updated_callbacks.Clear();
                    _updated_callbacks = null;

                    _initialized = false;
                }
                return terminated;
            }

            /// <summary>
            /// Callback for new input data.
            /// </summary>
            /// <param name="input_data">Reference to the new input data.</param>
            public void UpdateInputData(ref GenericDataStructure input_data)
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

                /*
                Log.Default.Msg(Log.Level.Debug, "Reading input data");
                if (!_library_data.ContainsKey(typeof(GenericDataStructure)))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing GenericDataBranch in library");
                    return;
                }
                _library_data[typeof(GenericDataStructure)] = input_data;

                int index = 0;
                init_metadata(input_data, ref index);

                DataDimensionality data_dim = new DataDimensionality();
                check_data_type(input_data, data_dim);

                if (data_dim.Uniform) 
                {

                }
                else if (data_dim.XY) 
                {


                }
                else if (data_dim.XYZ)
                {


                }
                else if (data_dim.Multivariate) 
                {


                }
                */

                /// DEBUG
                /*
                var debug_branch = input_data.Branches[0];
                foreach (var data_type in _library_data)
                {
                    Log.Default.Msg(Log.Level.Debug, data_type.Key.FullName);

                    if (data_type.Key == typeof(GenericDataBranch))
                    {
                        /// Nothing to do ...
                    }
                    else if (data_type.Key == typeof(SciChartUniformData))
                    {
                        var data_series = (SciChartUniformData)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < debug_branch.Entries.Count; i++)
                        {
                            data_series.Append((double)debug_branch.Entries[i].Values[0], debug_branch.Entries[i].MetaData);
                        }
                    }
                    else if (data_type.Key == typeof(SciChartData))
                    {
                        var data_series = (SciChartData)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < debug_branch.Entries.Count; i++)
                        {
                            data_series.Append((double)i, (double)debug_branch.Entries[i].Values[0], debug_branch.Entries[i].MetaData);
                        }
                    }
                    else
                    {
                        /// TODO Add more library data formats here ...
                        Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                    }
                }
                */


                // Notify registered update callbacks on new input data
                foreach (var updated_callback in _updated_callbacks)
                {
                    updated_callback();
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
            public object RequestDataCallback(Type t)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }
                /*
                if (_library_data.ContainsKey(t))
                {
                    return _library_data[t];
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data type: " + t.FullName);
                }
                */
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
                    return;
                }
                int index = sender_selection.Index;

                /*
                var entry = _library_data[SciChartUniformData].EntryAtIndex(index);
                var dim = entry.Dimension;

                foreach (var data_type in _library_data)
                {
                    if (data_type.Key == typeof(GenericDataStructure))
                    {
                        // Nothing to do since local data lists are linked per reference
                        /// var data_series = (GenericData_Type)_library_data[data_type.Key];
                    }
                    else if (data_type.Key == typeof(SciChartUniformData))
                    {
                        var data_series = (SciChartUniformData)_library_data[data_type.Key];
                        using (data_series.SuspendUpdates())
                        {
                            /// TODO fix me
                            /// data_series.Update(i, _data_y[i], data_series.Metadata[i]);
                        }
                    }
                    else if (data_type.Key == typeof(SciChartData))
                    {
                        var data_series = (SciChartData)_library_data[data_type.Key];
                        using (data_series.SuspendUpdates())
                        {
                            /// TODO fix me
                            /// data_series.Update(i, _data_y[i], data_series.Metadata[i]);
                        }
                    }
                    else
                    {
                        /// TODO Add more library data formats here ...
                        Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                    }
                }
                */
                // Send changed output data
                /* DEBUG
                if (_outputdata_callback != null)
                {
                    var out_data = new DefaultData_Type();
                    var list = new List<double>();
                    foreach (var meta_data in _data_meta)
                    {
                        list.Add(metadata.IsSelected ? 1.0 : 0.0);
                    }
                    out_data.Add(list);
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

            //private Dictionary<Type, List<Type>> _data_compatibility = null;


            private OutputData_Delegate _outputdata_callback = null;
            private List<UpdateCallback_Delegate> _updated_callbacks = null;
        }
    }
}
