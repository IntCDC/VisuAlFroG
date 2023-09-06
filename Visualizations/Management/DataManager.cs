using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Model.DataSeries;
using Visualizations.Interaction;
using Visualizations.SciChartInterface;
using Visualizations.WebAPI;
using Visualizations.PythonInterface;
using System.Windows.Markup;
using SciChart.Data.Model;
using SciChart.Charting3D.Primitives;



/*
 * Data Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {

        // Supported Data Types
        public class SciChartUniformData_Type : SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double> { }
        public class SciChartData_Type : SciChart.Charting.Model.DataSeries.XyDataSeries<double, double> { }


        public class DataManager : AbstractService
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the data manager for passing on the input data to the visualizations
            /// </summary>
            public delegate void InputData_Delegate(ref DefaultData_Type input_data);

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate(ref DefaultData_Type ouput_data);

            /// <summary>
            /// Callback for visualizations to request suitable data
            /// </summary>
            public delegate object RequestDataCallback_Delegate(Type t);

            /// <summary>
            /// TODO
            /// </summary>
            public delegate void RegisterUpdatedDataCallback_Delegate(UpdatedDataCallback_Delegate update_callback);

            /// <summary>
            /// TODO
            /// </summary>
            public delegate object UpdatedDataCallback_Delegate();


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                bool initilized = true;

                _data_x = new List<double>();
                _data_y = new List<double>();
                _data_meta = new List<MetaData>();

                // Register all supported data types
                _library_data = new Dictionary<Type, object>();
                _updated_callbacks = new List<UpdatedDataCallback_Delegate>();

                // DefaultData_Type
                _library_data.Add(typeof(DefaultData_Type), new DefaultData_Type());

                // SciChartUniformData_Type
                var data_scichart_uniform = new SciChartUniformData_Type();
                data_scichart_uniform.SeriesName = "SciChartUniformData_Type";
                _library_data.Add(typeof(SciChartUniformData_Type), data_scichart_uniform);

                // SciChartData_Type
                var data_scichart = new SciChartData_Type();
                data_scichart.SeriesName = "SciChartData_Type";
                _library_data.Add(typeof(SciChartData_Type), data_scichart);

                /// TODO Add more library data formats here ...


                _timer.Stop();
                _initilized = initilized;
                return _initilized;
            }

            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    _outputdata_callback = null;

                    foreach (var data_type in _library_data)
                    {
                        if (data_type.Key == typeof(DefaultData_Type))
                        {
                            ((DefaultData_Type)data_type.Value).Clear();
                        }
                        else if (data_type.Key == typeof(SciChartUniformData_Type))
                        {
                            ((SciChartUniformData_Type)data_type.Value).Clear();
                        }
                        else if (data_type.Key == typeof(SciChartData_Type))
                        {
                            ((SciChartData_Type)data_type.Value).Clear();
                        }
                        else
                        {
                            /// TODO Add more library data formats here ...
                            Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                        }
                    }
                    _library_data.Clear();
                    _library_data = null;
                    _data_x.Clear();
                    _data_x = null;
                    _data_y.Clear();
                    _data_y = null;
                    _data_meta.Clear();
                    _data_meta = null;
                    _updated_callbacks.Clear();
                    _updated_callbacks = null;

                    _initilized = false;
                }
                return terminated;
            }

            /// <summary>
            /// Callback for new input data.
            /// </summary>
            /// <param name="input_data">Reference to the new input data.</param>
            public void UpdateInputData(ref DefaultData_Type input_data)
            {
                _data_x.Clear();
                _data_y.Clear();
                _data_meta.Clear();
                if (input_data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Warn, "No input data available");
                    return;
                }
                _timer.Start();
                Log.Default.Msg(Log.Level.Debug, "Reading input data");

                // Copy input data

                //for (int x = 0; x < count_x; x++)
                //{
                /// DEBUG Taking only first value row
                int x = 0;
                var count_y = input_data[x].Count;
                for (int y = 0; y < count_y; y++)
                {
                    _data_x.Add(y);
                    _data_y.Add(input_data[x][y]);

                    var meta_data = new MetaData() { Index = y, IsSelected = false };
                    meta_data.PropertyChanged += metadata_changed;
                    _data_meta.Add(meta_data);
                }
                //}

                if ((_data_x.Count != _data_y.Count) || (_data_x.Count != _data_meta.Count) || (_data_y.Count != _data_meta.Count))
                {
                    Log.Default.Msg(Log.Level.Warn, "Data count does not match");
                    return;
                }

                // Create library dependent data
                foreach (var data_type in _library_data)
                {
                    Log.Default.Msg(Log.Level.Debug, data_type.Key.FullName);

                    if (data_type.Key == typeof(DefaultData_Type))
                    {
                        var data_series = (DefaultData_Type)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < _data_y.Count; i++)
                        {
                            data_series.Add(new List<double>() { _data_y[i] });
                        }
                    }
                    else if (data_type.Key == typeof(SciChartUniformData_Type))
                    {
                        var data_series = (SciChartUniformData_Type)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < _data_x.Count; i++)
                        {
                            data_series.Append(_data_y[i], _data_meta[i]);
                        }
                    }
                    else if (data_type.Key == typeof(SciChartData_Type))
                    {
                        var data_series = (SciChartData_Type)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < _data_x.Count; i++)
                        {
                            data_series.Append(_data_x[i], _data_y[i], _data_meta[i]);
                        }
                    }
                    else
                    {
                        /// TODO Add more library data formats here ...
                        Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                    }
                }

                // Notify registered update callbacks on new input data
                foreach (var updated_callback in _updated_callbacks)
                {
                    updated_callback();
                }

                _timer.Stop();
                Log.Default.Msg(Log.Level.Debug, "... done.");
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
            public void RegisterUpdatedDataCallback(UpdatedDataCallback_Delegate update_callback)
            {
                if (_updated_callbacks.Contains(update_callback))
                {
                    Log.Default.Msg(Log.Level.Debug, "callback for updated data already registered");
                    return;
                }
                _updated_callbacks.Add(update_callback);
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Return the data for the requested type.
            /// </summary>
            /// <param name="t">The type the data would be required.</param>
            /// <returns>The data as generic object. Cast to requested type manually.</returns>
            public object RequestDataCallback(Type t)
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (_library_data.ContainsKey(t))
                {
                    return _library_data[t];
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data type: " + t.FullName);
                }
                return null;
            }

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
                int i = sender_selection.Index;

                foreach (var data_type in _library_data)
                {
                    if (data_type.Key == typeof(DefaultData_Type))
                    {
                        var dataseries = (DefaultData_Type)_library_data[data_type.Key];
                        /// TODO
                    }
                    else if (data_type.Key == typeof(SciChartUniformData_Type))
                    {
                        var dataseries = (SciChartUniformData_Type)_library_data[data_type.Key];
                        using (dataseries.SuspendUpdates())
                        {
                            dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                        }
                    }
                    else if (data_type.Key == typeof(SciChartData_Type))
                    {
                        var dataseries = (SciChartData_Type)_library_data[data_type.Key];
                        using (dataseries.SuspendUpdates())
                        {
                            dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                        }
                    }
                    else
                    {
                        /// TODO Add more library data formats here ...
                        Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                    }
                }

                // Send changed output data
                if (_outputdata_callback != null)
                {
                    var out_data = new DefaultData_Type();
                    var list = new List<double>();
                    foreach (var metadata in _data_meta)
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
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private OutputData_Delegate _outputdata_callback = null;
            private List<double> _data_x = null;
            private List<double> _data_y = null;
            private List<MetaData> _data_meta = null;
            private Dictionary<Type, object> _library_data = null;
            private List<UpdatedDataCallback_Delegate> _updated_callbacks = null;
        }
    }
}
